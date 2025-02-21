using blazor.Components;
using blazor.services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5032") });
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IGameService,GameService>();
builder.Services.AddTransient<AuthenticationStateProvider,AuthProvider>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IMatchHubService,MatchHubService>();
builder.Services.AddScoped<MatchService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<MatchHubService>("/match_hub");

app.UseStatusCodePagesWithRedirects("/404");

app.Run();
