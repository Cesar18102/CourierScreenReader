using System.Net;

using Newtonsoft.Json;

namespace ScreenReaderService.Data
{
    public class ResponseError
    {
        [JsonProperty("code")]
        public HttpStatusCode Code { get; private set; }

        [JsonProperty("error")]
        public string Message { get; private set; }
    }
}