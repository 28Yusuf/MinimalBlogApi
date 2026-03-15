using Microsoft.EntityFrameworkCore;
using TechBlogApi.Context;

namespace TechBlogApi.Extensions
{
    public static class SeedData
    {
        public static void AddSeedDatas(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!context.Database.GetAppliedMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.Contacts.Any())
            {
                //Add Seed Datas
            }
        }
    }
}