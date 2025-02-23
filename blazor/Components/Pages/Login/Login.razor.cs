using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;

namespace blazor.Components.Pages.Login;

public partial class Login : ComponentBase
{
    public LoginModel Model { get; set; } = new();

    [Inject]
    private IAuthService? Service { get; set; }

    [Inject]
    private NavigationManager? Navigation { get; set; }

    private async Task SubmitValidFrom()
    {
        if (Service is null){return;}
        bool result = await Service.CredentialLogin(Model);
        if (!result) {return;}
        Navigation?.NavigateTo("/", true);
    }

    public void GoToRegisterPage()
    {
        Navigation?.NavigateTo("/register", false, true);
    }

    public void MicrosoftLogin()
    {

        string scope = "openid profile api://b86508f5-448b-4789-ba1d-286d6c115ea1/default";
        string redirect_uri = "https://localhost:7145";

        string URL = $"https://login.microsoftonline.com/9c523e69-1868-4f28-826a-993ddf8f33a8/oauth2/v2.0/authorize?client_id=b86508f5-448b-4789-ba1d-286d6c115ea1&response_type=token&redirect_uri={redirect_uri}&scope={scope}";

        Navigation?.NavigateTo(URL, false, false);
    }
}