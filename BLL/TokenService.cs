
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(
            string userId,
            string email,
            IEnumerable<string> roles)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var secret = jwtSettings["SecretKey"];

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException(
                    "Chưa cấu hình JwtSettings:SecretKey.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(secret);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "SecretKey phải có ít nhất 32 byte khi sử dụng HS256.");
            }

            var key = new SymmetricSecurityKey(keyBytes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email ?? ""),
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiration = jwtSettings[
                "AccessTokenExpirationMinutes"];

            if (!double.TryParse(expiration, out double minutes)
                || minutes <= 0)
            {
                throw new InvalidOperationException(
                    "AccessTokenExpirationMinutes không hợp lệ.");
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }
    }
}