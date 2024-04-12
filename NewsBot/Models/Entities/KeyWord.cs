using System.ComponentModel.DataAnnotations;

namespace NewsBot.Models.Entities
{
    public class KeyWord
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }
    }

}
