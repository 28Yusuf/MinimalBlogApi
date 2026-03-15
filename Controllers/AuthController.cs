using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TechBlogApi.Context;
using TechBlogApi.Dtos.Account;
using TechBlogApi.Helpers;
using TechBlogApi.Models;

namespace TechBlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly TokenHandlers _tokenHandler;
        AppDbContext appDbContext;

        public AuthController(UserManager<AppUser> userManager, TokenHandlers tokenHandler, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            _tokenHandler = tokenHandler;
            this.appDbContext = appDbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            AppUser? appUser = await userManager.Users
                .FirstOrDefaultAsync(x => x.UserName == dto.Name || x.Email == dto.Name, cancellationToken);

            if (appUser is null)
                return BadRequest("Invalid Username");

            bool result = await userManager.CheckPasswordAsync(appUser, dto.Password);
            if (!result)
                return BadRequest("Invalid Username or Password");

            TokenModel model = _tokenHandler.GenerateToken(appUser);
            appUser.RefreshToken = model.RefreshToken;
            appUser.RefreshTokenEndDate = model.ExpirationTime.AddMinutes(3);

            IdentityResult updateResult = await userManager.UpdateAsync(appUser);
            if (!updateResult.Succeeded)
                return StatusCode(500, updateResult.Errors);

            return Ok(new { model });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }

            AppUser appUser = new()
            {
                UserName = dto.Name,
                Email = dto.Email,
                Bio = dto.Bio,
                NormalizedUserName = dto.Name.ToUpper(),
                NormalizedEmail = dto.Email.ToUpper()
            };

            IdentityResult result = await userManager.CreateAsync(appUser, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(x => x.Description));
            }
            return Ok(new { Message = "User created successfully", UserId = appUser.Id });
        }

        [HttpGet("users")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetUsers()
        {
            
            return Ok(await appDbContext.Users.AsNoTracking().ToListAsync());
        }
    }
}