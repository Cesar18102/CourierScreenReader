using System;
using System.Threading.Tasks;

using Autofac;

using Server.Dto;
using Server.Models;
using Server.DataAccess;
using Server.DataAccess.Entities;

namespace Server.Services
{
    public class OrderService
    {
        private static readonly OrderRepo OrderRepo = DependencyHolder.RepoDependencies.Resolve<OrderRepo>();
        private static readonly AccountRepo AccountRepo = DependencyHolder.RepoDependencies.Resolve<AccountRepo>();

        private static readonly SessionService SessionService = DependencyHolder.ServiceDependencies.Resolve<SessionService>();

        public async Task<Order> TakeOrder(TakeOrderDto dto)
        {
            SessionService.CheckSession(dto.Session);

            OrderEntity constructedEntity = new OrderEntity()
            {
                AccountId = dto.Session.UserId.Value,
                FromA = dto.Order.FromA,
                ToB = dto.Order.ToB,
                TakeDateTime = DateTime.Now,
                Gain = dto.Order.Gain
            };

            int orderId = await OrderRepo.Insert(constructedEntity);
            OrderEntity insertedEntity = await OrderRepo.FirstOrDefault(order => order.Id, orderId);
            AccountEntity takerAccountEntity = await AccountRepo.GetById(insertedEntity.AccountId);

            Account takerModel = new Account()
            {
                Id = takerAccountEntity.Id,
                Login = takerAccountEntity.Login
            };

            return new Order()
            {
                Id = insertedEntity.Id,
                Taker = takerModel,
                FromA = insertedEntity.FromA,
                ToB = insertedEntity.ToB,
                TakeDateTime = insertedEntity.TakeDateTime,
                Gain = insertedEntity.Gain,
                Paid = insertedEntity.Paid
            };
        }
    }
}