using Microsoft.EntityFrameworkCore;
namespace EntityConnector.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<NumberData> Data { get; set; }
    }
}