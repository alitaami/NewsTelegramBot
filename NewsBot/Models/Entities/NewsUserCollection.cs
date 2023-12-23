using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBot.Entities
{
    public class NewsUserCollection
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NewsId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(NewsId))]
        public News News { get; set; }
    }
}
