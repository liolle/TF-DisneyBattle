
using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;

namespace blazor.Components.Pages.Register;

public partial class Register : ComponentBase
{
    public RegisterModel Model { get; set; } = new();

    [Inject]
    public IAuthService? Service { get; set; }
    [Inject]
    private NavigationManager? Navigation { get; set; }

    private async Task SubmitValidFrom()
    {
        if (Service is null){return;}
        bool result = await Service.Register(Model);
        if (!result) { return; }
        await Task.Delay(50);
        Navigation?.NavigateTo("/login");
    }
}