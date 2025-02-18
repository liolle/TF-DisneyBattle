using System.Security.Claims;
using blazor.models;
using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.services;

public class AuthProvider(IAuthService service) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await service.Auth();
        ClaimsIdentity identity = new();
        User? user = service.GetUser();
        if (user != null){
            List<Claim> claims =
            [
                new (nameof(User.Id), user.Id.ToString()),
                new (nameof(User.Email), user.Email)
            ];
            identity = new ClaimsIdentity(claims, "cookieAuth");
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}