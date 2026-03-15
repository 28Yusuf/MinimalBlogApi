using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TechBlogApi.Models;

namespace TechBlogApi.Helpers
{
    public class TokenHandlers
    {
        private readonly JwtSetting _jwtSetting;
        public TokenHandlers(IOptions<JwtSetting> options)
        {
            _jwtSetting = options.Value;
        }

        public TokenModel GenerateToken(AppUser user)
        {
            TokenModel model = new();
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key));
            SigningCredentials credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new Claim[]
            {
                new Claim("Bio", user.Bio),
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim("userId", user.Id.ToString())
            };

            model.ExpirationTime = DateTime.Now.AddMinutes(5);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
            (
                expires: model.ExpirationTime,
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                signingCredentials: credentials
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            model.AccessToken = tokenHandler.WriteToken(jwtSecurityToken);
            model.RefreshToken = GenerateRefreshToken();
            return model;
        }

        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[32];
            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}