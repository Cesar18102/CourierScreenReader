using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace Server.Dto
{
    public class OrderDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "from_a is required")]
        [JsonProperty("from_a")]
        public string FromA { get; private set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "to_b is required")]
        [JsonProperty("to_b")]
        public string ToB { get; private set; }

        [Required(ErrorMessage = "gain is required")]
        [JsonProperty("gain")]
        public float Gain { get; private set; }
    }
}