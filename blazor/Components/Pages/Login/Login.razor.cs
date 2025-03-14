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
  private IConfiguration? configuration { get; set; }

  [Inject]
  private NavigationManager? Navigation { get; set; }

  private async Task SubmitValidFrom()
  {
    if (Service is null) { return; }
    bool result = await Service.CredentialLogin(Model);
    if (!result) { return; }
    Navigation?.NavigateTo("/", true);
  }

  public void GoToRegisterPage()
  {
    Navigation?.NavigateTo("/register", false, true);
  }

  public void MicrosoftLogin()
  {

    string? redirect_uri = configuration?["REDIRECT_URI"];
    string? scope = configuration?["SCOPE"];
    string? tenant_id = configuration?["MICROSOFT_TENANT_ID"];
    string? client_id = configuration?["MICROSOFT_CLIENT_ID"];

    if (tenant_id is null || client_id is null || scope is null || redirect_uri is null){
      Console.WriteLine("Missing configurations");
      return;
    }

    string URL = $"https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/authorize?client_id={client_id}&response_type=code&redirect_uri={redirect_uri}&response_mode=query&scope={scope}";

    Navigation?.NavigateTo(URL, false, false);
  }
}
