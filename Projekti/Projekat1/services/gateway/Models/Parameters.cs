using Newtonsoft.Json;

namespace gateway.Models
{
    public class Parameters
    {
        // [JsonProperty]
        // public int ID { get; set; }

        [JsonProperty]
        public int Sys { get; set; }

        [JsonProperty]
        public int Dias { get; set; }

        [JsonProperty]
        public int Pulse { get; set; }

        //[JsonProperty]
        //public long Timestamp {get; set; }

        [JsonProperty]
        public int UserID { get; set; }

    }

    
}