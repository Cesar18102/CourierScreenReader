using Newtonsoft.Json;

namespace Server.Models
{
    public class Account
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}