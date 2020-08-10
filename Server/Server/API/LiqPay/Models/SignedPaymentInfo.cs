using Newtonsoft.Json;

namespace Server.API.LiqPay.Models
{
    public class SignedPaymentInfo
    {
        [JsonProperty("data")]
        public string Data { get; private set; }

        [JsonProperty("signature")]
        public string Signature { get; private set; }

        public SignedPaymentInfo(string data, string signature)
        {
            Data = data;
            Signature = signature;
        }
    }
}