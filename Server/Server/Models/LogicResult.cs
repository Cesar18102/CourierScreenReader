using Newtonsoft.Json;

namespace Server.Models
{
    public class LogicResult
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        public LogicResult(bool result)
        {
            Result = result;
        }
    }
}