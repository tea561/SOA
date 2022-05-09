using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace gateway.Models
{
    public class Parameters
    {
        // [JsonProperty]
        // public int ID { get; set; }
        [Display(Name = "sys", Description = "Systolic blood pressure")]
        [JsonProperty("sys")]
        public int Sys { get; set; }

        [JsonProperty("dias")]
        public int Dias { get; set; }

        [JsonProperty("pulse")]
        public int Pulse { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp {get; set; }

        [JsonProperty("userID")]
        public int UserID { get; set; }

    }

    
}