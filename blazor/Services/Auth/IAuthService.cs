using blazor.models;

namespace blazor.services;

public interface IAuthService
{
    User? GetUser();
    Task<bool> Register(RegisterModel model);
    Task<bool> Login(LoginModel model);
    Task Logout();
}