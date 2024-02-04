namespace NewsBot.Models.ViewModels
{
    public class NewsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MessageId { get; set; } 
         public List<int> KeyWords { get; set; }
    }
    public class NewsUpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MessageId { get; set; }
        public List<int> KeyWords { get; set; }

    }
}
