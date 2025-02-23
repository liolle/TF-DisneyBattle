using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;

namespace blazor.Components.Pages.Home;

public partial class Home : ComponentBase
{
    User? CurrentUser { get; set; } = null;
    bool Sending = false;

    [Inject]
    private MatchService? matchService { get; set; }

    public async Task JoinGame()
    {
        if (CurrentUser is null || Sending) { return; }
        Sending = true;
        Console.WriteLine("sending");
        await Task.Delay(50);
        //await matchService.JoinMatchmakingAsync(CurrentUser.Id, "");
    }

}