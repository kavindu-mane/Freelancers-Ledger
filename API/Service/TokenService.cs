using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Interfaces;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Service
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService()
        {
            var IssuerSigningKey = Environment.GetEnvironmentVariable("JWT_SECRET");

            if (string.IsNullOrEmpty(IssuerSigningKey))
            {
                throw new Exception(
                    "Issuer Signing Key not found. Ensure the .env file is correctly configured and placed in the root directory."
                );
            }

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IssuerSigningKey));
        }

        public string CreateToken(AppUser user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("Email not found.");
            }

            var claims = new List<Claim> { new(JwtRegisteredClaimNames.Email, user.Email) };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = creds,
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
