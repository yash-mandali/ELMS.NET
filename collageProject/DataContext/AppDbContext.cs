using collageProject.Model;
using Microsoft.EntityFrameworkCore;

namespace collageProject.DataContext
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
