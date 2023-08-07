namespace ElasticExampleApp
{
    public class Message
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Text { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedDate { get; set; }
    }
}
