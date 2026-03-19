using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechBlogApi.Models;
using TechBlogApi.Models.Common;

namespace TechBlogApi.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        private readonly IHttpContextAccessor context;
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor context) : base(options)
        {
            this.context = context;
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostBookMark> PostBookMarks { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<SocialLink> SocialLinks { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();
            var now = DateTime.Now;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added && entry.Entity is BaseEntity entity)
                {
                    entity.CreatedDate = now;
                    entity.CreatedBy = int.Parse(context?.HttpContext?.User.FindFirstValue("userId") ?? "1");
                    entity.IsDeleted = false;
                }
                else if (entry.State == EntityState.Modified && entry.Entity is BaseEntity updEntity)
                {
                    updEntity.UpdatedBy = int.Parse(context?.HttpContext?.User.FindFirstValue("userId") ?? "1");
                    updEntity.UpdatedDate = now;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>()
            .HasOne(x => x.PostBookMark)
            .WithOne(y => y.Post)
            .HasForeignKey<PostBookMark>(x => x.PostId);
            
            builder.Entity<Post>()
            .HasOne(x => x.PostLike)
            .WithOne(y => y.Post)
            .HasForeignKey<PostLike>(x => x.PostId);
        }
    }
}
