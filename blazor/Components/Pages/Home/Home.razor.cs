using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;

namespace blazor.Components.Pages.Home;

public partial class Home : ComponentBase
{
    User? CurrentUser { get; set; } = null;
    bool Sending = false;

    [Inject]
    private MatchService matchService { get; set; }

    public async Task JoinGame()
    {
        if (CurrentUser is null || Sending) { return; }
        Sending = true;
        Console.WriteLine("sending");
        await matchService.JoinMatchmakingAsync(CurrentUser.Id, "");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {

        if (CurrentUser is not null) { return; }
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity is null || !user.Identity.IsAuthenticated) { return; }

        int.TryParse(user.FindFirst("Id")?.Value, out int id);
        CurrentUser = new(id, user.FindFirst("email")?.Value ?? "");

        StateHasChanged();
    }


}