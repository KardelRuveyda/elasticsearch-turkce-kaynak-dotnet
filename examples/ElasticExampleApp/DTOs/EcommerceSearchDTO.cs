namespace ElasticExampleApp
{
    public class EcommerceSearchDTO
    {
        public string? CityName { get; set; }
        public string? CustomerFullName { get; set; }
        public string? CategoryName { get; set; }
        public string? Manufacturer { get; set; }
        public double? TaxfulTotalPriceGte { get; set; }
        public double? TaxfulTotalPriceLte { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
    }
}
