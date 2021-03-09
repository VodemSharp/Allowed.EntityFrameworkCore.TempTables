using Newtonsoft.Json;

namespace Allowed.EntityFrameworkCore.TempTables.Sample.Models
{
    public class JsonAddress
    { 
        [JsonProperty("address1")]
        public string Address1 { get; set; }

        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("coordinates")]
        public JsonCoordinates Coordinates { get; set; }
    }
}
