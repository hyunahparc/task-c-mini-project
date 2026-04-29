using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskFlow.Helpers
{
    public static class JwtHelper
    {
        public static string CreateToken(List<Claim> claims, IConfiguration configuration)
        {
            string? keyString = configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(keyString))
            {
                throw new Exception("JWT Key is missing in configuration.");
            }

            SymmetricSecurityKey key =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            SigningCredentials creds =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
