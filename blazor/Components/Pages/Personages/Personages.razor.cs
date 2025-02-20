using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;

namespace blazor.Components.Pages.Personages;

public partial class Personages : ComponentBase
{
    List<Personage> Per = [];

    [Inject]
    private IGameService gameService { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Per = await gameService.AllPersons();
            StateHasChanged();
        }
    }
}