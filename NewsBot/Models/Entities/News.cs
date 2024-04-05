namespace NewsBot.Entities
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MessageId { get; set; }

        public DateTime CreatedDate { get; set; }
        public ICollection<NewsKeyWord> NewsKeyWords { get; set; }
    }
}
