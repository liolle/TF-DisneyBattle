using System.Security.Claims;
using blazor.models;
using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.services;

public class AuthProvider(IAuthService service) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        
        ClaimsIdentity identity = new();
        User? user = service.GetUser();

        for (int i = 0; i<10; i++)
        {
            int round = (int) Math.Ceiling(Math.Pow(2, i));
            await Task.Delay(round);
            User? u = service.GetUser();
            if(u is not null){
                user = u;
                break;
            }
        }

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