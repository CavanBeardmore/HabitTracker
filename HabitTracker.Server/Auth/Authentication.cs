using HabitTracker.Server.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HabitTracker.Server.Auth
{
    public class Authentication : IAuthentication
    {
        private readonly string _jwtSecret;

        public Authentication(string jwtSecret)
        {
            _jwtSecret = jwtSecret;
        }

        public string GenerateJWTToken(string username)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username)
            };
            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(_jwtSecret)
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
