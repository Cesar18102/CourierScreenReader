using System.Threading.Tasks;
using System.Collections.Generic;

using Autofac;

using Server.DataAccess.Entities;

namespace Server.DataAccess
{
    public class OrderRepo : RepoBase<OrderEntity>
    {
        private AccountRepo AccountRepo = DependencyHolder.RepoDependencies.Resolve<AccountRepo>();

        public OrderRepo(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<OrderEntity>> GetByTakerLogin(string login)
        {
            AccountEntity account = await AccountRepo.GetByLogin(login);
            return await Get(order => order.AccountId, account.Id);
        }

        public async Task<OrderEntity> MarkPaid(int orderId)
        {
            OrderEntity stored = await FirstOrDefault(order => order.Id, orderId);

            if (stored == null)
                return null;

            stored.Paid = true;
            return await Update(stored);
        }
    }
}