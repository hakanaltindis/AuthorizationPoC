using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebApi.Helpers;

namespace WebApi.Data
{
    public class WebApiDbContext : DbContext
    {
        public WebApiDbContext(DbContextOptions<WebApiDbContext> options, IIdentityHelper identityHelper)
            : base(options)
        {
            _identityHelper = identityHelper;
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        private readonly IIdentityHelper _identityHelper;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Blog>().Property<Guid>("_tenantId").HasColumnName("TenantId");

            // Configure entity filters
            builder.Entity<Blog>().HasQueryFilter(b => EF.Property<Guid>(b, "_tenantId") == _identityHelper.TenantId);
            builder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            foreach (var item in ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Added && e.Metadata.GetProperties().Any(p => p.Name == "_tenantId"))
                )
            {
                item.CurrentValues["_tenantId"] = _identityHelper.TenantId;
            }

            return base.SaveChanges();
        }
    }
}
