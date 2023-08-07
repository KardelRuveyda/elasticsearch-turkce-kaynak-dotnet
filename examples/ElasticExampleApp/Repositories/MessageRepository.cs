namespace ElasticExampleApp
{
    public class MessageRepository : ElasticsearchRepository<Message>, IMessageRepository
    {

        public MessageRepository(string connectionString, string indexName) : base(connectionString, indexName)
        {
        }
    }

    public interface IMessageRepository : IRepository<Message>
    {
    }
}
