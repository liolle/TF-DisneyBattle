
using blazor.models;
using Microsoft.JSInterop;

namespace blazor.services;
public class AuthService(IJSRuntime jS) : IAuthService
{
    private User? CurrentUser;

    public User? GetUser()
    {
        return CurrentUser;
    }
    public async Task Auth()
    {
        var response = await jS.InvokeAsync<User>("auth");
        CurrentUser = response;
    }
    public async Task<bool> Register(RegisterModel model)
    {
        var response = await jS.InvokeAsync<LoginResult>("register", model.UserName, model.Email, model.Password);
        if (response is null || response.IsFailure)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> Login(LoginModel model)
    {
        var response = await jS.InvokeAsync<LoginResult>("login", model.UserName, model.Password);
        if (response is null || response.IsFailure)
        {
            return false;
        }
        return true;
    }

    public async Task Logout()
    {
        await jS.InvokeAsync<string>("logout");
    }

}