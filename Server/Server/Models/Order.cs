using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public class Order
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("taker")]
        public Account Taker { get; set; }

        [JsonProperty("from_a")]
        public string FromA { get; set; }

        [JsonProperty("to_b")]
        public string ToB { get; set; }

        [JsonProperty("take_date_time")]
        public DateTime TakeDateTime { get; set; }

        [JsonProperty("gain")]
        public float Gain { get; set; }

        [JsonProperty("paid")]
        public bool Paid { get; set; }
    }
}