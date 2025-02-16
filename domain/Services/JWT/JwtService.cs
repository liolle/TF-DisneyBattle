using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using disney_battle.dal.entities;
using disney_battle.domain.exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace disney_battle.domain.services;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    /// <exception cref="MissingConfigurationException">Thrown when a configuration could not be found in th injected IConfiguration.</exception>
    public JwtService(IConfiguration configuration)
    {
        string? jwt_key = configuration["JWT_KEY"] ?? throw new MissingConfigurationException("JWT_KEY");
        string? jwt_issuer = configuration["JWT_ISSUER"] ?? throw new MissingConfigurationException("JWT_ISSUER");
        string? jwt_audience = configuration["JWT_AUDIENCE"] ?? throw new MissingConfigurationException("JWT_AUDIENCE");

        _secretKey = jwt_key;
        _issuer = jwt_issuer;
        _audience = jwt_audience;
    }

    public string generate(UserEntity user)
    {
        List<Claim> claims =
        [
            new(nameof(UserEntity.Id), user.Id.ToString()),    
            new(nameof(UserEntity.Email), user.Email),                
        ];

        // Standardized User info
        ClaimsIdentity identity = new(claims,CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);

        // Token signing info
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256Signature);

        JwtSecurityToken token = new(
            issuer: _issuer,
            audience: _audience,
            claims: principal.Claims, 
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token); 
    }
}