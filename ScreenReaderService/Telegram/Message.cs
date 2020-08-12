using Newtonsoft.Json;

namespace ScreenReaderService.Telegram
{
    public class Message
    {
        [JsonProperty("message_id")]
        public int Id { get; private set; }

        [JsonProperty("from")]
        public User Sender { get; private set; }

        [JsonProperty("text")]
        public string Text { get; private set; }

        [JsonProperty("date")]
        public int Date { get; private set; }
    }
}
