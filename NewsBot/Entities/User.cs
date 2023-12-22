using NewsBot.Enums;

namespace NewsBot.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public long ChatId { get; set; }
        public UserType UserType { get; set; }
        public int? ParentId { get; set; }
        public User Parent { get; set; }
    }

}
