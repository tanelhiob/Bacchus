using Microsoft.EntityFrameworkCore;

namespace Bacchus.Server.Data
{
    public class BacchusDbContext : DbContext
    {
        public DbSet<Bid> Bids { get; set; }

        public BacchusDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
