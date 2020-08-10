using System;

using Newtonsoft.Json;

namespace ScreenReaderService.Data
{
    public class Session
    {
        [JsonProperty("user_id")]
        public int UserId { get; private set; }

        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("expires")]
        public DateTime Expires { get; private set; }
    }
}