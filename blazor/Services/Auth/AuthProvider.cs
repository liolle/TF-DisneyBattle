using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using blazor.models;
using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.services;

public class AuthProvider(IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.Delay(20);
        HttpContext? httpContext = httpContextAccessor.HttpContext;


        if (httpContext is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        string? accessToken = httpContext.Request.Cookies["disney_battle_auth_token"];

        if (accessToken is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jsonToken = handler.ReadJwtToken(accessToken);

        string? email = jsonToken.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
        string? id = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        if (email is null || id == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        List<Claim> claims =
        [
            new (nameof(User.Id), id),
            new (nameof(User.Email), email)
        ];
        ClaimsIdentity identity = new ClaimsIdentity(claims, "cookieAuth");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

}