using blazor.services;
using Microsoft.AspNetCore.Components;

namespace blazor.Components.Pages.Oauth.MicrosoftOauth;

public partial class MicrosoftOauth : ComponentBase
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string? Code { get; set; }

    [Inject]
    IAuthService? authService {get;set;}

    [Inject]
    NavigationManager? navigationManager {get;set;}



    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (authService is null || Code is null){
            return;
        }
        await authService.MicrosoftLogin(Code,"https://localhost:7145/oauth-microsoft","https://localhost:7145/oauth-microsoft");

        navigationManager?.NavigateTo("/",true,true);
    }
}