using blazor.Components;
using blazor.services;
using DotNetEnv;
using edllx.dotnet.csrf;
using Microsoft.AspNetCore.Components.Authorization;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

// Add Env &  Json configuration
Env.Load();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddSingleton(typeof(Program).Assembly);
builder.Services.AddSingleton<CSRFService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5032") });
builder.Services.AddScoped<HttpInfoService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<MatchService>();


var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseHttpMetrics();
app.UseStatusCodePagesWithReExecute("/404");

app.UseCSRFBlazorServer();
app.UseAntiforgery();

app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

app.MapHub<ConnectionHub>("/match_hub");
app.MapMetrics();


app.Run();
