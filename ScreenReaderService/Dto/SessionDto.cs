using Newtonsoft.Json;

namespace ScreenReaderService.Dto
{
    public class SessionDto
    {
        [JsonProperty("user_id")]
        public int? UserId { get; set; }

        [JsonProperty("session_token_salted")]
        public string SessionTokenSalted { get; set; }

        [JsonProperty("salt")]
        public string Salt { get; set; }
    }
}