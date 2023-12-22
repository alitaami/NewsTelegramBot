using NewsBot.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsBot.Entities
{
    public class UserActivity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ActivityType ActivityType { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
