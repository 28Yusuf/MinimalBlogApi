using System.Text;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TechBlogApi.Context;
using TechBlogApi.Helpers;
using TechBlogApi.Models;
using TechBlogApi.Repositories.Concretes;
using TechBlogApi.Repositories.Interfaces;
using TechBlogApi.Services.Abstracts;
using TechBlogApi.Services.Concretes;
using TechBlogApi.UnitOfWorks;

namespace TechBlogApi.Extensions;

public static class ServiceRegistration
{
    public static void AddRegister(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddCarter();
        services.Configure<JwtSetting>(config.GetRequiredSection("JWT"));
        services.AddHttpContextAccessor();

        #region Authentication And Authorization
        services.AddIdentity<AppUser, AppRole>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 1;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(
            opt =>
            {
                opt.DefaultAuthenticateScheme =
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        )
        .AddJwtBearer(opt =>
        {
            opt.SaveToken = false;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["JWT:Issuer"],
                ValidAudience = config["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"] ?? "")),
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddAuthorization();
        services.AddScoped<TokenHandlers>();
        services.AddSignalR();
        #endregion


        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<ISocialService, SocialService>();
        services.AddScoped<IPostService, PostService>();
    }
}