using Newtonsoft.Json;

namespace Server.Dto
{
    public class TakeOrderDto
    {
        [JsonProperty("session")]
        public SessionDto Session { get; private set; }

        [JsonProperty("order")]
        public OrderDto Order { get; private set; }
    }
}