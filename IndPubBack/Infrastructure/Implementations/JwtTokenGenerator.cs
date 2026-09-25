using IndPubBack.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IndPubBack.Infrastructure.Interfaces;
using IndPubBack.DTOs.Responses.User;

namespace IndPubBack.Infrastructure.Implementations;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    public AccessTokenResponse GenerateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(
            Convert.ToDouble(configuration["AppSettings:TokenLifetimeMinutes"]));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                configuration["AppSettings:AccessToken"]
                ?? throw new InvalidOperationException("AppSettings:AccessToken не налаштовано.")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var accessToken = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

        return new AccessTokenResponse(accessTokenString);
    }
}