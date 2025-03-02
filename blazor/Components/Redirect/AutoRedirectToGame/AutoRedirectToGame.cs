namespace blazor.Components.Redirect.RedirectToGame;
using blazor.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public partial class AutoRedirectToGame : ComponentBase
{
    [Inject]
    private MatchService? MatchService { get; set; }

    [Inject]
    private AuthenticationStateProvider? AuthProvider { get; set; }

    [Inject]
    NavigationManager? Navigation { get; set; }

    protected override void OnInitialized()
    {
        if (MatchService is null || AuthProvider is null) { return; }
        MatchService.OnStateChanged += NavigateToGame;
    }

    private async Task NavigateToGame()
    {
        if (AuthProvider is null) { return; }
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (!(user.Identity?.IsAuthenticated ?? false)) { return; }

        if (user.HasClaim(val=>val.Type =="PlayerState" && val.Value == "PlayerPlaying"))
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