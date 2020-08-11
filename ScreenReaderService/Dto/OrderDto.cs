using Newtonsoft.Json;

namespace ScreenReaderService.Dto
{
    public class OrderDto
    {
        [JsonProperty("from_a")]
        public string FromA { get; set; }

        [JsonProperty("to_b")]
        public string ToB { get; set; }

        [JsonProperty("gain")]
        public float Gain { get; set; }
    }
}