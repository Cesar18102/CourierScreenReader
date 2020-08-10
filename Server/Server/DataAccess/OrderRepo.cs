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