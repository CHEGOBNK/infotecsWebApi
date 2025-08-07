using infotecsWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace infotecsWebApi
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Value> Values => Set<Value>();
        public DbSet<Result> Results => Set<Result>();
    }
}
