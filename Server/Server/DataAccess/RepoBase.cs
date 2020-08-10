using System;
using System.Reflection;
using System.Data.SqlClient;
using System.Threading.Tasks;

using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using Server.Models.Exceptions;
using Server.DataAccess.Entities;
using Server.DataAccess.Attributes;

namespace Server.DataAccess
{
    public class RepoBase<TEntity> where TEntity : class, IEntity, new()
    {
        protected string ConnectionString { get; set; }

        protected string TableName { get; set; }

        protected string KeyColumnName { get; set; }
        protected PropertyInfo KeyProperty { get; set; }

        protected IEnumerable<string> Columns { get; set; }
        protected IEnumerable<PropertyInfo> Properties { get; set; }

        protected IEnumerable<string> ColumnsExceptKey { get; set; }
        protected IEnumerable<PropertyInfo> PropertiesExceptKey { get; set; }

        public RepoBase(string connectionString)
        {
            InitMetaData();
            ConnectionString = connectionString;
        }

        private void InitMetaData()
        {
            KeyProperty = GetKeyProperty();
            KeyColumnName = GetColumnByProperty(KeyProperty);

            Properties = typeof(TEntity).GetProperties();
            Columns = Properties.Select(property => GetColumnByProperty(property)).ToList();

            PropertiesExceptKey = Properties.Where(property => property.Name != KeyProperty.Name).ToList();
            ColumnsExceptKey = Columns.Where(column => column != KeyColumnName).ToList();

            TableName = typeof(TEntity).GetCustomAttribute<TableAttribute>()?.Name ?? nameof(TEntity).ToLower();
        }

        private PropertyInfo GetKeyProperty()
        {
            return typeof(TEntity).GetProperties().FirstOrDefault(
                property => property.Name.ToLower() == "id" || property.GetCustomAttribute<KeyAttribute>() != null
            );
        }

        private IDictionary<string, object> GetParametersValues(IEnumerable<string> columns, TEntity entity)
        {
            return columns.ToDictionary(
                column => $"@{column}", 
                column => GetPropertyByColumn(column).GetValue(entity)
            );
        }

        private string GetColumnByProperty(PropertyInfo property)
        {
            if (property == null)
                return null;

            return property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name.ToLower();
        }

        private PropertyInfo GetPropertyByColumn(string column)
        {
            PropertyInfo[] properties = typeof(TEntity).GetProperties();

            return properties.FirstOrDefault(property => property.GetCustomAttribute<ColumnAttribute>()?.Name == column) ??
                   properties.FirstOrDefault(property => property.Name.ToLower() == column.ToLower());
        }

        private TEntity Fetch(SqlDataReader reader)
        {
            TEntity entity = new TEntity();

            for (int i = 0; i < reader.FieldCount; ++i)
            {
                string name = reader.GetName(i);
                PropertyInfo property = GetPropertyByColumn(name);

                if (property != null)
                {
                    object converted = Convert.ChangeType(reader[name], property.PropertyType);
                    property.SetValue(entity, converted);
                }
            }

            return entity;
        }

        private async Task<IEnumerable<TEntity>> FetchList(SqlDataReader reader)
        {
            List<TEntity> entities = new List<TEntity>();

            while(await reader.ReadAsync())
            {
                TEntity entity = Fetch(reader);
                entities.Add(entity);
            }

            return entities;
        }

        private string GetColumnByExpression(Expression<Func<TEntity, object>> expression)
        {
            MemberExpression memberExpression = null;

            if (expression.Body.NodeType == ExpressionType.MemberAccess)
                memberExpression = expression.Body as MemberExpression;
            else if (expression.Body.NodeType == ExpressionType.Convert)
                memberExpression = (expression.Body as UnaryExpression)?.Operand as MemberExpression;

            if (memberExpression == null)
                return null;

            return GetColumnByProperty(memberExpression.Member as PropertyInfo);
        }

        private string GetLimitString(int limit)
        {
            return limit > 0 ? $" TOP {limit}" : "";
        }

        private async Task<IEnumerable<TEntity>> GetLimited(Expression<Func<TEntity, object>> expression, object value, int limit)
        {
            string column = GetColumnByExpression(expression);
            string selectText = $"SELECT {GetLimitString(limit)} * FROM {TableName} WHERE {column} = @value;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand selector = new SqlCommand(selectText, connection))
            {
                selector.Parameters.AddWithValue("@value", value);
                connection.Open();
                using (SqlDataReader reader = await selector.ExecuteReaderAsync())
                    return await FetchList(reader);
            }
        }

        private async Task<IEnumerable<TEntity>> GetLimited(int limit)
        {
            string selectText = $"SELECT {GetLimitString(limit)} * FROM {TableName};";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand selector = new SqlCommand(selectText, connection))
            {
                connection.Open();
                using (SqlDataReader reader = await selector.ExecuteReaderAsync())
                    return await FetchList(reader);
            }
        }

        public async Task<IEnumerable<TEntity>> Get(int limit)
        {
            return await GetLimited(limit);
        }

        public async Task<IEnumerable<TEntity>> Get()
        {
            return await GetLimited(-1);
        }

        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, object>> expression, object value, int limit)
        {
            return await GetLimited(expression, value, limit);
        }

        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, object>> expression, object value)
        {
            return await GetLimited(expression, value, -1);
        }

        public async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, object>> expression, object value)
        {
            IEnumerable<TEntity> list = await GetLimited(expression, value, 1);
            return list.FirstOrDefault();
        }

        private class InsertOutputEntity : IEntity
        {
            public int Id { get; private set; }
        }

        public async Task<int> Insert(TEntity entity)
        {
            string columns = string.Join(",", ColumnsExceptKey.Select(column => $"\"{column}\""));

            IDictionary<string, object> parametersValues = GetParametersValues(ColumnsExceptKey, entity);
            string parametersString = string.Join(",", parametersValues.Keys);

            string insertText = $"INSERT INTO {TableName}({columns})\n" +
                                $"OUTPUT INSERTED.ID\n" +
                                $"VALUES({parametersString});";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand inserter = new SqlCommand(insertText, connection))
            {
                foreach (KeyValuePair<string, object> parameterValue in parametersValues)
                    inserter.Parameters.AddWithValue(parameterValue.Key, parameterValue.Value);

                connection.Open();
                return (int)await inserter.ExecuteScalarAsync();
            }
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            int id = (int)KeyProperty.GetValue(entity);
            TEntity current = await FirstOrDefault(stored => stored.Id, id);

            if (current == null)
                throw new NotFoundException("entity with such id");

            IDictionary<string, object> changedColumns = new Dictionary<string, object>();
            foreach(PropertyInfo property in PropertiesExceptKey)
            {
                object storedValue = property.GetValue(current);
                object updatedValue = property.GetValue(entity);

                if (storedValue.Equals(updatedValue))
                    continue;

                changedColumns.Add(GetColumnByProperty(property), updatedValue);
            }

            if (changedColumns.Count == 0)
                return current;

            IEnumerable<string> changeQueryItems = changedColumns.Select(change => $"{change.Key} = @{change.Key}");
            string setString = string.Join(", ", changeQueryItems);

            string updateText = $"UPDATE {TableName}\n" +
                                $"SET {setString}\n" +
                                $"WHERE {KeyColumnName} = {id};";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand updater = new SqlCommand(updateText, connection))
            {
                foreach (KeyValuePair<string, object> changedColumn in changedColumns)
                    updater.Parameters.AddWithValue($"@{changedColumn.Key}", changedColumn.Value);

                connection.Open();
                await updater.ExecuteNonQueryAsync();
            }

            return await FirstOrDefault(updated => updated.Id, id);
        }
    }
}