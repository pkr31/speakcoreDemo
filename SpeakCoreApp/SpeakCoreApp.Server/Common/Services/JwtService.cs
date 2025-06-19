using Microsoft.IdentityModel.Tokens;
using Serilog;
using speakcore.API.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SpeakCoreApp.Server.Common.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(Registration user)
        {
            try
            {
                var jwtSettings = _config.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"]!;
                var issuer = jwtSettings["Issuer"]!;
                var audience = jwtSettings["Audience"]!;
                var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]!);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim("userId", user.RegistrationId.ToString()),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName)
                };

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                    signingCredentials: creds
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred");
                throw;
            }

        }
    }
}
