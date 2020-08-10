using System;

namespace Server.API.LiqPay.Models
{
    public class PaymentPrepareModel
    {
        public float Amount { get; private set; }
        public string OrderId { get; private set; }
        public string CallbackUrl { get; set; }
        public string Description { get; set; }

        public PaymentPrepareModel(string orderId, float amount, string callback)
        {
            OrderId = orderId;
            Amount = amount;
            CallbackUrl = callback;
        }
    }
}