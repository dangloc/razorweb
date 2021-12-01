using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace razorwebapp.models
{
    public class MyWebContext : IdentityDbContext<AppUser>
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

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                 var tablename = entity.GetTableName();
                 if(tablename.StartsWith("AspNet"))
                 {
                     entity.SetTableName(tablename.Substring(6));
                 }

            }
        }

        public DbSet<Article> articles { get; set; }

    }
}