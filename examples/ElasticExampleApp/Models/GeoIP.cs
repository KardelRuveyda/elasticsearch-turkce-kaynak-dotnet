using System.Text.Json.Serialization;

namespace ElasticExampleApp
{
    public class GeoIP
    {
        [JsonPropertyName("city_name")]
        public string? CityName { get; set; }
    }
}
