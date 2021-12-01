using Microsoft.EntityFrameworkCore;

namespace razorwebapp.models
{
    public class MyWebContext : DbContext
    {
        public MyWebContext(DbContextOptions<MyWebContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Article> articles { get; set; }

    }
}