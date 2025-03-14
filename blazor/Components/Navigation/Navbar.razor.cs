namespace blazor.Components.Navigation;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using blazor.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public partial class Navbar : ComponentBase
{

  [Inject]
  private AuthenticationStateProvider? AuthProvider { get; set; }

  [Inject]
  private IAuthService? Service { get; set; }

  [Inject]
  private NavigationManager? Navigation { get; set; }
  public bool IsConnected { get; set; }

  [Inject]
  private IConfiguration? configuration { get; set; }

  private string Provider = "credential";


  public string Page { get; private set; } = "";
  bool LogButtonShowing
  {
    get
    {
      List<string> pgs = [
        "login","register"
      ];

      for (int i = 0; i < pgs.Count; i++)
      {
        if (pgs[i] == Page) { return false; }
      }

      return true;
    }
  }


  protected override void OnInitialized()
  {
    SetPage();
    StateHasChanged();
  }

  protected override async Task OnInitializedAsync()
  {
    if (AuthProvider is null) { return; }
    var authState = await AuthProvider.GetAuthenticationStateAsync();
    var user = authState.User;

    if (user.Identity?.IsAuthenticated ?? false)
    {
      Provider = user.FindFirst("Provider")?.Value ?? "credential";
    }
  }

  private void SetPage()
  {
    if (Navigation is null) { return; }
    ;
    string[] parts = Navigation.Uri.Split(':');
    if (parts.Length == 0) { return; }
    string pattern = @"\/([^\n\/]*)";
    string input = parts[parts.Length - 1];
    Match match = Regex.Match(input, pattern);
    Page = match.Groups[1].Value.ToLower();
  }

  public void Login()
  {
    Navigation?.NavigateTo("/login");
  }

  public void NavigateToPersonage()
  {
    Navigation?.NavigateTo("/personages");
  }

  public void NavigateToHomePage()
  {
    Navigation?.NavigateTo("/");
  }


  public async Task Logout()
  {
    if (configuration is null) { return; }
    string? post_logout_redirect_uri = configuration["POST_REDIRECT_URI"];
    if (Service is null || post_logout_redirect_uri is null) { return; }

    await Service.Logout();
    switch (Provider)
    {
      case "microsoft":
        Navigation?.NavigateTo($"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={post_logout_redirect_uri}");
        break;
      default:
        Navigation?.Refresh();
        break;
    }

  }

}
