using System.IdentityModel.Tokens.Jwt;
}
    }
        }
            return null;
        {
        catch
        }
            return tokenHandler.ValidateToken(token, validationParameters, out _);
            
            };
                ClockSkew = TimeSpan.Zero
                ValidateLifetime = true,
                ValidAudience = _configuration["Jwt:Audience"] ?? "CareHomeClient",
                ValidateAudience = true,
                ValidIssuer = _configuration["Jwt:Issuer"] ?? "CareHomeAPI",
                ValidateIssuer = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
            {
            var validationParameters = new TokenValidationParameters
            
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!");
            var tokenHandler = new JwtSecurityTokenHandler();
        {
        try
    {
    public ClaimsPrincipal? ValidateToken(string token)
    
    }
        return new JwtSecurityTokenHandler().WriteToken(token);
        
        );
            signingCredentials: credentials
            expires: DateTime.UtcNow.AddHours(8),
            claims: claims,
            audience: _configuration["Jwt:Audience"] ?? "CareHomeClient",
            issuer: _configuration["Jwt:Issuer"] ?? "CareHomeAPI",
        var token = new JwtSecurityToken(
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        );
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!")
        var key = new SymmetricSecurityKey(
        
        };
            new Claim("FullName", user.FullName)
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        {
        var claims = new[]
    {
    public string GenerateToken(User user)
    
    }
        _configuration = configuration;
    {
    public JwtService(IConfiguration configuration)
    
    private readonly IConfiguration _configuration;
{
public class JwtService
/// </summary>
/// Source: Microsoft JWT documentation - https://learn.microsoft.com/en-us/aspnet/core/security/authentication/
/// Service for handling JWT token generation and validation
/// <summary>

namespace CA2_SOA.Services;

using CA2_SOA.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

