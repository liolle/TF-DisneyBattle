namespace blazor.Components.Navigation;

using System.Threading.Tasks;
using blazor.services;
using Microsoft.AspNetCore.Components;


public partial class Navbar : ComponentBase
{
    [Inject]
    public IAuthService? Service {get;set;}
    [Inject]
    private NavigationManager? Navigation {get;set;}
    public bool IsConnected { get; set; }

    public void Login()
    {
        Navigation?.NavigateTo("/login");
    }

    public async Task Logout()
    {
        if (Service is null){return;}
        await Service.Logout();
        Navigation?.Refresh(true);
        await Task.Delay(50);
        StateHasChanged();
    }

}