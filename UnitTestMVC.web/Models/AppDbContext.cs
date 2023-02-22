using Microsoft.EntityFrameworkCore;

namespace UnitTestMVC.web.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public virtual DbSet<Product> Products { get; set; }
    }
}
