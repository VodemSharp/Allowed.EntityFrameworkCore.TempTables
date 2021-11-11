using Newtonsoft.Json;

namespace Allowed.EntityFrameworkCore.TempTables.Sample.Models
{
    public class JsonCoordinates
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
}
