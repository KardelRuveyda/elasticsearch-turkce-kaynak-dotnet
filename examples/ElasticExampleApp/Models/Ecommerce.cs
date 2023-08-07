using System.Text.Json.Serialization;

namespace ElasticExampleApp
{
    public class ECommerce
    {
        [JsonPropertyName("_id")]
        public int Id { get; set; }
        [JsonPropertyName("customer_first_name")]
        public string? CustomerFirstName { get; set; }
        [JsonPropertyName("customer_last_name")]
        public string? CustomerLastName { get; set; }
        [JsonPropertyName("customer_full_name")]
        public string? CustomerFullName { get; set; }
        [JsonPropertyName("category")]
        public string[]? Categories { get; set; }
        [JsonPropertyName("order_id")]
        public int OrderId { get; set; }
        [JsonPropertyName("order_date")]
        public DateTime OrderDate { get; set; }
        [JsonPropertyName("products")]
        public Product[]? Products { get; set; }
        [JsonPropertyName("taxful_total_price")]
        public double TaxfulTotalPrice { get; set; }
        [JsonPropertyName("geoip")]
        public GeoIP? GeoIP { get; set; }
    }
}
