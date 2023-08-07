using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace ElasticExampleApp
{
    public class ECommerceRepository : ElasticsearchRepository<ECommerce>, IECommerceRepository
    {

        public ECommerceRepository(string connectionString, string indexName) : base(connectionString, indexName)
        {
        }

        public async Task<ImmutableList<ECommerce>> TermQuerySearchWithCustomerFirstName(string customerFirstName)
        {
            var result = await _client.SearchAsync<ECommerce>(
                s => s.Query(
                    q => q.Term(
                        t => t.Field("customer_first_name.keyword").Value(customerFirstName)
                        )));

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PrefixQuerySearchWithCustomerFullName(string customerFirstName)
        {
            var result = await _client.SearchAsync<ECommerce>(
                s => s.Query(
                    q => q.Prefix(
                        p => p.Field("customer_full_name.keyword").Value(customerFirstName)
                        )));

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> RangeQuerySearchWithTaxfulPrice(double from, double to)
        {
            var result = await _client.SearchAsync<ECommerce>(
                s => s.Query(
                    q => q.Range(
                        t => t.NumberRange(
                            nr => nr.Field(f => f.TaxfulTotalPrice)
                                .Gt(from)
                                .Lte(to)
                        ))));

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FuzzyQuerySearchWithCustomerFirstName(string customerFirstNameFuzzy)
        {
            var result = await _client.SearchAsync<ECommerce>(
                s => s.Query(
                    q => q.Fuzzy(
                        fu => fu.Field(f => f.CustomerFirstName!.Suffix("keyword")).Value(customerFirstNameFuzzy)
                            .Fuzziness(new Fuzziness("1"))
                        )));

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchPhraseWithCustomerFullName(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(
                s => s.Query(
                    q => q.MatchPhrase(
                        m => m.Field(f => f.CustomerFullName).Query(customerFullName)
                        )));

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQuery(EcommerceSearchDTO compoundSearchDTO)
        {
            var result = await _client.SearchAsync<ECommerce>(
                s => s.Query(q =>
                    q
                        .Bool(b =>
                        b
                            .Must(m => m.Term(t => t.Field("geoip.city_name").Value(compoundSearchDTO.CityName!)))
                            .MustNot(mn => mn.Range(r => r.NumberRange(
                                nr => nr.Field(f => f.TaxfulTotalPrice)
                                .Gte(compoundSearchDTO.TaxfulTotalPriceGte)
                                .Lte(compoundSearchDTO.TaxfulTotalPriceLte))))
                            .Should(s => s.Term(t => t.Field(f => f.Categories!.Suffix("keyword")).Value(compoundSearchDTO.CategoryName!)))
                            .Filter(f => f.Term(t => t.Field("manufacturer.keyword").Value(compoundSearchDTO.Manufacturer!)))
                        )
            ));

            return result.Documents.ToImmutableList();
        }

        public async Task<(List<ECommerce>? list, long count)> CustomQuery(EcommerceSearchDTO customQuery)
        {
            List<Action<QueryDescriptor<ECommerce>>> listQuery = new();

            if (!string.IsNullOrEmpty(customQuery.CityName))
            {
                listQuery.Add((q) => q.MatchPhrase(
                   m => m.Field("geoip.city_name").Query(customQuery.CityName)
                    ));

                /*
                    listQuery.Add((q) => q.MatchPhrase(
                        m => m.Field(f => f.GeoIP!.CityName).Query(customQuery.CityName)
                    ));
                */
            }

            if (!string.IsNullOrEmpty(customQuery.CategoryName))
            {
                listQuery.Add((q) => q.Match(
                    m => m.Field(f => f.Categories).Query(customQuery.CategoryName).Fuzziness(new Fuzziness("1"))
                ));
            }

            if (!string.IsNullOrEmpty(customQuery.CustomerFullName))
            {
                listQuery.Add((q) => q.Wildcard(
                    m => m.Field(f => f.CustomerFullName).Value(customQuery.CustomerFullName)
                ));
            }
            if (customQuery.TaxfulTotalPriceGte != null)
            {
                listQuery.Add((q) => q.Range(r => r.NumberRange(
                                nr => nr.Field(f => f.TaxfulTotalPrice)
                                .Gte(customQuery.TaxfulTotalPriceGte)))
                );
            }

            if (customQuery.TaxfulTotalPriceLte != null)
            {
                listQuery.Add((q) => q.Range(r => r.NumberRange(
                                nr => nr.Field(f => f.TaxfulTotalPrice)
                                .Lte(customQuery.TaxfulTotalPriceLte)))
                );
            }

            var pageFrom = (customQuery.Page - 1) * customQuery.PageSize;

            var result = await _client.SearchAsync<ECommerce>(
                s => s
                    .Size(customQuery.PageSize)
                    .From(pageFrom)
                    .Query(q => q.Bool(b => b.Must(listQuery.ToArray()))
            ));

            return (list: result.Documents?.ToList(), result.Total);
        }
    }

    public interface IECommerceRepository : IRepository<ECommerce>
    {
        Task<ImmutableList<ECommerce>> TermQuerySearchWithCustomerFirstName(string customerFirstName);
        Task<ImmutableList<ECommerce>> PrefixQuerySearchWithCustomerFullName(string customerFirstName);
        Task<ImmutableList<ECommerce>> RangeQuerySearchWithTaxfulPrice(double from, double to);
        Task<ImmutableList<ECommerce>> FuzzyQuerySearchWithCustomerFirstName(string customerFirstNameFuzzy);
        Task<ImmutableList<ECommerce>> MatchPhraseWithCustomerFullName(string customerFullName);
        Task<ImmutableList<ECommerce>> CompoundQuery(EcommerceSearchDTO compoundSearchDTO);
        Task<(List<ECommerce>? list, long count)> CustomQuery(EcommerceSearchDTO customQuery);
    }
}
