using blazor.models;

namespace blazor.services;

public interface IAuthService
{
    User? GetUser();
    Task<bool> Register(RegisterModel model);
    Task<bool> CredentialLogin(LoginModel model);
    Task MicrosoftLogin(string code,string redirect_success_uri,string redirect_failure_uri);
    Task Logout();
}