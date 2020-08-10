using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Autofac;

using Server.Models.Exceptions;

using Server.DataAccess;
using Server.DataAccess.Entities;

namespace Server.Services
{
    public class PayService
    {
        private const int MIN_PAY = 150; //uah
        private const int PAY_PERCENT = 30;
        private const int MAX_UPNPAYED_TIME = 960; //in minutes

        private static readonly OrderRepo OrderRepo = DependencyHolder.RepoDependencies.Resolve<OrderRepo>();

        public async Task CheckPaymentRequired(int userId)
        {
            IEnumerable<OrderEntity> orders = await OrderRepo.GetByTakerId(userId);
            IEnumerable<OrderEntity> oldUnpayedOrders = orders.Where(
                order => !order.Paid && (DateTime.Now - order.TakeDateTime).TotalMinutes > MAX_UPNPAYED_TIME
            );

            if (oldUnpayedOrders.Count() != 0)
            {
                float totalGain = oldUnpayedOrders.Sum(order => order.Gain);
                int ourTax = (int)Math.Max(MIN_PAY, totalGain * PAY_PERCENT / 100);
                throw new PaymentRequiredException(ourTax);
            }
        }

        public async Task GetPaymentForm(int userId)
        {
            //TODO
        }

        public async Task HandlePaymentCallback()
        {
            //TODO
        }
    }
}