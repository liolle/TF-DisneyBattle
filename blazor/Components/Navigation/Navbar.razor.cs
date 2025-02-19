namespace blazor.Components.Navigation;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;


public partial class Navbar : ComponentBase
{
    [Inject]
    public IAuthService? Service { get; set; }
    [Inject]
    private NavigationManager? Navigation { get; set; }
    public bool IsConnected { get; set; }

    User? CurrentUser { get; set; } = null;
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity is null || !user.Identity.IsAuthenticated) { return; }

            int.TryParse(user.FindFirst("Id")?.Value, out int id);
            CurrentUser = new(id, user.FindFirst("email")?.Value ?? "");
            StateHasChanged();
        }
    }

    public void Login()
    {
        Navigation?.NavigateTo("/login");
    }

    public void NavigateToHomePage()
    {
        Navigation?.NavigateTo("/");
    }

    public async Task Logout()
    {
        if (Service is null) { return; }
        await Service.Logout();
        Navigation?.Refresh(true);
        await Task.Delay(50);
        StateHasChanged();
    }

}