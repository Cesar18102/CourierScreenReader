using Newtonsoft.Json;

namespace ScreenReaderService.Telegram
{
    public class UpdateInfo
    {
        [JsonProperty("update_id")]
        public int Id { get; private set; }

        [JsonProperty("message")]
        public Message Message { get; private set; }
    }
}
