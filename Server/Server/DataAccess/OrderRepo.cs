using System.Threading.Tasks;
using System.Collections.Generic;

using Server.DataAccess.Entities;

namespace Server.DataAccess
{
    public class OrderRepo : RepoBase<OrderEntity>
    {
        public OrderRepo(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<OrderEntity>> GetByTakerId(int takerId)
        {
            return await Get(order => order.AccountId, takerId);
        }
    }
}