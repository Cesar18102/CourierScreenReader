using System;

namespace Server.Models.Exceptions
{
    public class PaymentRequiredException : Exception
    {
        public float Amount { get; private set; }

        public PaymentRequiredException(float amount)
        {
            Amount = amount;
        }

        public override string Message => $"You must pay {Amount}";
    }
}