
using blazor.models;
using Microsoft.JSInterop;

namespace blazor.services;
public class AuthService : IAuthService
{
    private User? CurrentUser;

    private readonly IJSRuntime JS ;


    public AuthService(IJSRuntime jS){
        JS = jS;
    }

    public User? GetUser()
    {
        return CurrentUser;
    }
    private async Task Auth()
    {
        var response = await JS.InvokeAsync<User>("auth");
        CurrentUser = response;
        
    }
    public async Task<bool> Register(RegisterModel model)
    {
        var response = await JS.InvokeAsync<LoginResult>("register", model.UserName, model.Email, model.Password);
        if (response is null || response.IsFailure)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> CredentialLogin(LoginModel model)
    {
        var response = await JS.InvokeAsync<LoginResult>("login", model.UserName, model.Password);
        if (response is null || response.IsFailure)
        {
            return false;
        }
        return true;
    }

    public async Task Logout()
    {
        await JS.InvokeAsync<string>("logout");
    }

    public async Task MicrosoftLogin(string code,string redirect_success_uri,string redirect_failure_uri)
    {
        await JS.InvokeVoidAsync("microsoftOauth",code,redirect_success_uri,redirect_failure_uri);
    }
}