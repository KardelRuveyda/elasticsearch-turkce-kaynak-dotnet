using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;

namespace ElasticExampleApp
{
    public class ElasticsearchRepository<T> : IRepository<T> where T : class
    {
        protected readonly ElasticsearchClient _client;
        private readonly string _indexName;

        public ElasticsearchRepository(string connectionString, string indexName)
        {
            var connectionSettings = new ElasticsearchClientSettings(new Uri(connectionString)).DefaultIndex(indexName);
            _client = new ElasticsearchClient(connectionSettings);
            _indexName = indexName;
        }

        public async Task<T> GetAsync(int id)
        {
            var response = await _client.GetAsync<T>(id, g => g.Index(_indexName));
            return response.Source!;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var response = await _client.SearchAsync<T>(s => s.Index(_indexName));
            return response.Documents;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .Query(q => q
                    .Bool(b => b.Filter(f => f
                        .Match(m => m.Field(predicate))
                    ))
                )
            );
            return response.Documents;
        }

        public async Task<bool> AddAsync(T entity)
        {
            var response = await _client.IndexAsync(entity);

            if (response.IsValidResponse)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var response = await _client.UpdateAsync<T, object>(_indexName, Id.From(entity), u => u.Doc(entity));

            if (response.IsValidResponse)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _client.DeleteAsync<T>(id);

            if (response.IsValidResponse)
            {
                return true;
            }

            return false;
        }
    }

    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
