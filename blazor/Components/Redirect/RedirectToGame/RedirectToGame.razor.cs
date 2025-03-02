namespace blazor.Components.Redirect.RedirectToGame;
using blazor.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public partial class RedirectToGame : ComponentBase
{
    [Inject]
    private MatchService? MatchService { get; set; }

    [Inject]
    private AuthenticationStateProvider? AuthProvider { get; set; }

    [Inject]
    NavigationManager? Navigation { get; set; }

    protected override void OnInitialized()
    {
        Console.WriteLine($"{MatchService?.CurrentState}");
        if (MatchService is null || AuthProvider is null) { return; }
        MatchService.OnStateChanged += NavigateToGame;
        if (MatchService?.CurrentState == "Playing")
        {
            Navigation?.NavigateTo("/game");
        }
    }

    private async Task NavigateToGame()
    {
        if (AuthProvider is null) { return; }
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (!(user.Identity?.IsAuthenticated ?? false)) { return; }
        if (MatchService?.CurrentState == "Playing")
        {

            Navigation?.NavigateTo("/game");
        }
    }

    public void Dispose()
    {
        if (MatchService is null) { return; }
        MatchService.OnStateChanged -= NavigateToGame;
    }
}