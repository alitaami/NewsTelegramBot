using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBot.Entities
{
    public class NewsKeyWord
    {
        public int Id { get; set; } 
        public int NewsId { get; set; } 
        public int KeyWordId { get; set; }

        [ForeignKey(nameof(NewsId))]
        public News News { get; set; }

        [ForeignKey(nameof(KeyWordId))]
        public KeyWord KeyWord { get; set; }
    }

}
