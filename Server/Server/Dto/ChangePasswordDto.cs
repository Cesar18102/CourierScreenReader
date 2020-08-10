using Newtonsoft.Json;

namespace Server.Dto
{
    public class ChangePasswordDto
    {
        [JsonProperty("session")]
        public SessionDto Session { get; private set; }

        [JsonProperty("password")]
        public string Password { get; private set; }
    }
}