using Microsoft.EntityFrameworkCore;
using NewsBot.Entities;

namespace NewsBot.Data.NewsContext
{
    public class NewsContext : DbContext
    {
        public NewsContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
