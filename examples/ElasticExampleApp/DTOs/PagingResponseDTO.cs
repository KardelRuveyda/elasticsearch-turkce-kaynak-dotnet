namespace ElasticExampleApp
{
    public class PagingResponseDTO<T>
    {
        public T? Response { get; set; }
        public long TotalCount { get; set; }
    }
}
