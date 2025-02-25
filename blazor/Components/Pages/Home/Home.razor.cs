using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace blazor.Components.Pages.Home;

public partial class Home : ComponentBase
{
    bool Sending = false;

    int CurrentUserId;

    [Inject]
    private MatchService? MatchService { get; set; }

    [Inject]
    private AuthenticationStateProvider? AuthProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (AuthProvider is null) { return; }
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (!(user.Identity?.IsAuthenticated ?? false)){return;}

        _ = int.TryParse(user.FindFirst("Id")?.Value, out CurrentUserId);

        if (MatchService is null){return;}

        MatchService.MatchFound += HandleMatchFound;
    }

    private void HandleMatchFound(GameMatch gameMatch, Player player){
        Console.WriteLine($"Match found between: {gameMatch.P1} and {gameMatch.P2}");
    }

    public async Task JoinGame()
    {
        if (MatchService is null || CurrentUserId == 0 || Sending) { return; }
        Sending = true;
        await Task.Delay(50);
        await MatchService.JoinMatchmakingAsync(CurrentUserId);
        Sending = false;
    }

}