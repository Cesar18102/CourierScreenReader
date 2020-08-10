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

        public static readonly LogicResult TRUE_RESULT = new LogicResult(true);
        public static readonly LogicResult FALSE_RESULT = new LogicResult(false);
    }
}