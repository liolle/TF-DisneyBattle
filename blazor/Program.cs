using blazor.services;
using DotNetEnv;
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

  builder.Services.AddAuthorizationCore();
  builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5032") });
  builder.Services.AddScoped<IGameService,GameService>();
  builder.Services.AddScoped<IAuthService,AuthService>();
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

app.UseRouting();
app.UseHttpMetrics();
app.UseAntiforgery();

app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

  app.MapHub<ConnectionHub>("/match_hub");
  app.MapMetrics(); 
  app.UseStatusCodePagesWithRedirects("/404");

  app.Run();
