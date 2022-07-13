using Microsoft.EntityFrameworkCore;

namespace MinimalAPI
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {

        }

        public DbSet<Person> People => Set<Person>();
    }
}
