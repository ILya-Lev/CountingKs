using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CountingKs.Models
{
    public class LinkModel
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("rel")]
        public string Relation { get; set; }

        [JsonProperty("Method")]
        public string Method { get; set; }

        [JsonProperty("IsTemplated")]
        public bool IsTemplated { get; set; }
    }
}