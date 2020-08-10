using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Autofac;

using Server.API.LiqPay;
using Server.API.LiqPay.Dto;
using Server.API.LiqPay.Models;

using Server.Models.Exceptions;

using Server.DataAccess;
using Server.DataAccess.Entities;

namespace Server.Services
{
    public class PayService
    {
        private const int MIN_PAY = 150; //uah
        private const int PAY_PERCENT = 30;
        private const int MAX_UPNPAYED_TIME = 8 * 60;// 960; //in minutes

        private const string CALLBACK_URL = "http://37.229.145.236:5000/api/pay/HandlePaymentCallback";

        private static readonly LiqPay LiqPay = DependencyHolder.ServiceDependencies.Resolve<LiqPay>();
        private static readonly OrderRepo OrderRepo = DependencyHolder.RepoDependencies.Resolve<OrderRepo>();

        public class PendingPayment
        {
            public string Id { get; private set; }
            public SignedPaymentInfo PaymentInfo { get; private set; }
            public IEnumerable<OrderEntity> PaidOrders { get; private set; }

            public PendingPayment(string id, SignedPaymentInfo paymentInfo, IEnumerable<OrderEntity> paidOrders)
            {
                Id = id;
                PaymentInfo = paymentInfo;
                PaidOrders = paidOrders;
            }
        }

        private IDictionary<string, PendingPayment> PendingPayments = new Dictionary<string, PendingPayment>();

        private async Task<IEnumerable<OrderEntity>> GetOldUnpayedOrders(string login)
        {
            IEnumerable<OrderEntity> orders = await OrderRepo.GetByTakerLogin(login);
            return orders.Where(order => !order.Paid && (DateTime.Now - order.TakeDateTime).TotalMinutes > MAX_UPNPAYED_TIME);
        }

        private int GetTaxAmount(IEnumerable<OrderEntity> orders)
        {
            float totalGain = orders.Sum(order => order.Gain);
            return (int)Math.Max(MIN_PAY, totalGain * PAY_PERCENT / 100);
        }

        public async Task CheckPaymentRequired(string login)
        {
            IEnumerable<OrderEntity> oldUnpayedOrders = await GetOldUnpayedOrders(login);

            if (oldUnpayedOrders.Count() != 0)
                throw new PaymentRequiredException(GetTaxAmount(oldUnpayedOrders));
        }

        public async Task<SignedPaymentInfo> GetPaymentForm(string login)
        {
            IEnumerable<OrderEntity> oldUnpayedOrders = await GetOldUnpayedOrders(login);

            if (oldUnpayedOrders.Count() == 0)
                throw new NotFoundException("orders to pay for");

            int tax = GetTaxAmount(oldUnpayedOrders);
            string id = Guid.NewGuid().ToString();

            PaymentPrepareModel paymentPrepare = new PaymentPrepareModel(id, tax, CALLBACK_URL);
            SignedPaymentInfo signedPayment = LiqPay.CreatePayment(paymentPrepare);
            PendingPayment pending = new PendingPayment(id, signedPayment, oldUnpayedOrders);

            PendingPayments.Add(id, pending);
            return signedPayment;
        }

        public async Task<PendingPayment> HandlePaymentCallback(PaymentConfirmDto dto)
        {
            SignedPaymentInfo signed = new SignedPaymentInfo(dto.data, dto.signature);
            string id = LiqPay.GetOrderId(signed);

            if (!PendingPayments.ContainsKey(id))
                throw new NotFoundException("payment with such id");

            if (!LiqPay.IsSucceed(signed))
                throw new ValidationException();

            PendingPayment payment = PendingPayments[id];
            if (!LiqPay.IsPaymentAuthorized(signed, payment.PaymentInfo))
                throw new UnauthorizedAccessException();

            foreach (OrderEntity order in payment.PaidOrders)
                await OrderRepo.MarkPaid(order.Id);

            return payment;
        }
    }
}