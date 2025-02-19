using blazor.models;
using blazor.services;
using Microsoft.AspNetCore.Components;

namespace blazor.Components.Pages.Login;

public partial class Login : ComponentBase
{
    public LoginModel Model { get; set; } = new();

    [Inject]
    public IAuthService? Service {get;set;}
    [Inject]
    private NavigationManager? Navigation {get;set;}

    private async Task SubmitValidFrom (){
        if (Service is null){return;}
       bool result = await Service.Login(Model);
       if (!result) {return;}
       await Task.Delay(50);
       Navigation?.NavigateTo("/",true);
    }

    public void GoToRegisterPage(){
        Navigation?.NavigateTo("/register",false,true);
    }
}