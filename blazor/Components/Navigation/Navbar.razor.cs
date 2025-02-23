namespace blazor.Components.Navigation;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;

public partial class Navbar : ComponentBase
{

    [Inject]
    private IAuthService? Service {get;set;}

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
        if (Service is null) { return; }
        await Service.Logout();
        Navigation?.Refresh();
    }

}