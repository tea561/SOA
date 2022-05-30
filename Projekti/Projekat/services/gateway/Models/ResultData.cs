using Newtonsoft.Json;

namespace gateway.Models
{
    public class ResultData
    {
        [JsonProperty]
        public int ID { get; set; }

        [JsonProperty]
        public Parameters? HealthParameters {get; set; }

        [JsonProperty]
        public string? ResourceUrl {get;set;}

        [JsonProperty]
        public string? ResourceTitle { get; set; }

    }

    
}