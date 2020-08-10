using System.Text;
using System.Security.Cryptography;

using Autofac;

using Server.Services;
using Server.API.LiqPay;
using Server.DataAccess;
using Server.DataAccess.Entities;

namespace Server
{
    public class DependencyHolder
    {
        private static IContainer serviceDependencies = null;
        public static IContainer ServiceDependencies
        {
            get
            {
                if (serviceDependencies == null)
                    serviceDependencies = BuildServiceDependencies();
                return serviceDependencies;
            }
        }

        private static IContainer repoDependencies = null;
        public static IContainer RepoDependencies
        {
            get
            {
                if (repoDependencies == null)
                    repoDependencies = BuildRepoDependencies();
                return repoDependencies;
            }
        }

        private DependencyHolder() { }

        private static IContainer BuildServiceDependencies()
        {
            ContainerBuilder builder = new ContainerBuilder();

            TypedParameter hasher = new TypedParameter(typeof(HashAlgorithm), SHA256.Create());
            TypedParameter encoding = new TypedParameter(typeof(Encoding), Encoding.UTF8);

            builder.RegisterType<HashingService>()
                   .UsingConstructor(typeof(HashAlgorithm), typeof(Encoding))
                   .WithParameters(new TypedParameter[] { hasher, encoding })
                   .AsSelf().SingleInstance();

            builder.RegisterType<SessionService>().AsSelf().SingleInstance();

            builder.RegisterType<AccountService>().AsSelf().SingleInstance();
            builder.RegisterType<OrderService>().AsSelf().SingleInstance();
            builder.RegisterType<PayService>().AsSelf().SingleInstance();
            builder.RegisterType<PasswordService>().AsSelf().SingleInstance();

            builder.RegisterType<LiqPay>().AsSelf().SingleInstance();

            return builder.Build();
        }

        private const string CONNECTION_STRING = "Server=courierbot.mssql.somee.com; Database=courierbot; User Id=CourierBot2020_SQLLogin_1; Password=yoveguqpjw";

        private static IContainer BuildRepoDependencies()
        {
            ContainerBuilder builder = new ContainerBuilder();

            RegisterRepo<AccountRepo, AccountEntity>(builder, CONNECTION_STRING);
            RegisterRepo<OrderRepo, OrderEntity>(builder, CONNECTION_STRING);
            RegisterRepo<PasswordRepo, PasswordEntity>(builder, CONNECTION_STRING);

            return builder.Build();
        }

        private static void RegisterRepo<TRepo, TEntity>(ContainerBuilder builder, string connectionString) 
            where TRepo : RepoBase<TEntity>
            where TEntity : class, IEntity, new()
        {
            TypedParameter connString = new TypedParameter(
                typeof(string), connectionString
            );

            builder.RegisterType<TRepo>()
                  .UsingConstructor(typeof(string))
                  .WithParameter(connString)
                  .AsSelf().SingleInstance();
        }
    }
}