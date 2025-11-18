using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AllinOne.Models.Configuration;
using AllinOne.Services.Interfaces;

namespace AllinOne.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IOptions<JwtSettings> options, ILogger<JwtService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public string GenerateToken(string username, string appKey)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("appKey", appKey),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Secret);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "JWT token expired at {Expires}", ex.Expires);
                return null;
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogWarning(ex, "JWT validation failed: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while validating JWT token");
                return null;
            }
        }
    }
}
