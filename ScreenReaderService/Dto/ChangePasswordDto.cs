using Newtonsoft.Json;

namespace ScreenReaderService.Dto
{
    public class ChangePasswordDto
    {
        [JsonProperty("session")]
        public SessionDto Session { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}