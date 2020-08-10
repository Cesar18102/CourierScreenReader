using Newtonsoft.Json;

namespace ScreenReaderService.Data
{
    public class SignedPaymentInfo
    {
        [JsonProperty("data")]
        public string Data { get; private set; }

        [JsonProperty("signature")]
        public string Signature { get; private set; }
    }
}