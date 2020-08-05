using Newtonsoft.Json;

namespace ScreenReaderService.Telegram
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; private set; }

        [JsonProperty("username")]
        public string Username { get; private set; }
    }
}
