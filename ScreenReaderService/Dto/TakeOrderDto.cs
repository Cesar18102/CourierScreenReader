using Newtonsoft.Json;

namespace ScreenReaderService.Dto
{
    public class TakeOrderDto
    {
        [JsonProperty("session")]
        public SessionDto Session { get; set; }

        [JsonProperty("order")]
        public OrderDto Order { get; set; }
    }
}