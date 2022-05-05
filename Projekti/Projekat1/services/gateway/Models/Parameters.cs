using Newtonsoft.Json;

namespace gateway.Models
{
    public class Parameters
    {
        // [JsonProperty]
        // public int ID { get; set; }

        [JsonProperty]
        public int SysPressure { get; set; }

        [JsonProperty]
        public int DiasPressure { get; set; }

        [JsonProperty]
        public int Pulse {get; set; }

        [JsonProperty]
        public string? Time {get; set; }

    }

    
}