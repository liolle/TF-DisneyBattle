using blazor.models;
using Microsoft.AspNetCore.Components;

namespace blazor.Components.Pages.Home;

public partial class Home : ComponentBase
{
    User? CurrentUser { get; set; } = null;

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