using Newtonsoft.Json;

namespace Server.API.LiqPay.Dto
{
    public class PaymentStatusDto
    {
        [JsonProperty("status")]
        public string Status { get; private set; }
    }
}