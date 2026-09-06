using Microsoft.EntityFrameworkCore;

namespace EnglishBuddy.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<WordCard> WordCard { get; set; }
    }
}
