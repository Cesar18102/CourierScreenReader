using Newtonsoft.Json;

namespace Server.API.LiqPay.Models
{
    public class LiqPayPaymentInfo
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("public_key")]
        public string PublicKey { get; set; }

        [JsonIgnore]
        public string PrivateKey { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; private set; }

        [JsonProperty("order_id")]
        public string OrderId { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("server_url")]
        public string CallbackUrl { get; private set; }

        [JsonConstructor]
        public LiqPayPaymentInfo() { }

        public LiqPayPaymentInfo(PaymentPrepareModel prepare)
        {
            Amount = prepare.Amount;
            OrderId = prepare.OrderId;
            Description = prepare.Description;
            CallbackUrl = prepare.CallbackUrl;
        }

        /*private string GetDate(DateTime dt)
        {
            StringBuilder date = new StringBuilder($"{dt.Year}-");
            AppendDateItem(dt.Month, date).Append("-");
            AppendDateItem(dt.Day, date).Append(" ");
            AppendDateItem(dt.Hour, date).Append(":");
            AppendDateItem(dt.Minute, date).Append(":");
            return AppendDateItem(dt.Second, date).ToString();
        }

        private StringBuilder AppendDateItem(int item, StringBuilder builder)
        {
            if (item < 10)
                builder.Append("0");
            return builder.Append(item);
        }*/
    }
}