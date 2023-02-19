using Microsoft.EntityFrameworkCore;
namespace EntityConnector.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        DbSet<EntityConnector.Models.RandomNumberData> numberData { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // modelBuilder.Entity<EntityConnector.Models.RandomNumberData>()
            
        //     // modelBuilder.Entity<EntityConnector.Models.RandomNumberData>
        //     // modelBuilder.Entity<EntityConnector.Models.RandomNumberData>
            
        // }
    }
}