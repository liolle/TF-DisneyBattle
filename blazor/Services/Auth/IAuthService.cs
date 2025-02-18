using blazor.models;

namespace blazor.services;

public interface IAuthService
{
    User? GetUser();
    Task Auth();
    Task<bool> Register(RegisterModel model);
    Task<bool> Login(LoginModel model);
    Task Logout();
}