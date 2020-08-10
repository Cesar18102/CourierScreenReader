using System;

namespace ScreenReaderService.Data.Exceptions
{
    public class PaymentRequiredException : Exception
    {
        public float Amount { get; private set; }

        public PaymentRequiredException(float amount)
        {
            Amount = amount;
            message = $"You must pay {Amount}";
        }

        public PaymentRequiredException(string message)
        {
            this.message = message;
        }

        private string message = "";
        public override string Message => message;
    }
}