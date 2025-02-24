#### 1: Blazor app redirect the user on the page 

```C#
string URL = $"https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/authorize?client_id={client_id}&response_type=code&redirect_uri={redirect_uri}&response_mode=query&scope={scope}";		
    
Navigation.NavigateTo(URL, false, false);
```

We are then redirected on the `/oauth-microsoft` on the Blazor app, where we can extract the code from the query parameter.
```C#
protected override async Task OnAfterRenderAsync(bool firstRender)
{
	if (authService is null || Code is null){return;}
	await authService.MicrosoftLogin(Code,"https://localhost:7145/oauth-microsoft","https://localhost:7145/oauth-microsoft");

	navigationManager.NavigateTo("/",true,true);
}
```

#### 2: API endpoint (/oauth/microsoft)
This endpoint is used to transform the code into a token

```c#
[Route("/oauth/microsoft")]
[EnableCors("auth-input")]
public async Task<IActionResult> OauthMicrosoft([FromBody] OauthMicrosoftQuery query)
{
	try
    {
        string? token_name = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigurationException("AUTH_TOKEN_NAME");
        
        QueryResult<string> result = await userService.Execute(query);
        
        if (result.IsFailure && query.Redirect_Failure_Uri is not null){return BadRequest(result.ErrorMessage);}
        
        if (result.IsFailure){return BadRequest(result.ErrorMessage);}
        
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(60)
        };
        
        Response.Cookies.Append(token_name, result.Result, cookieOptions);
        
        return Ok();
    }
    catch (Exception)
    {
        return BadRequest("Server error");
    }
}
```

#### 3: The API exchange the code for a token 
This token contain all information need to uniquely identify an account in my database. 
```C#
var tokenResponse = await httpClient.PostAsync(
$"https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/token",
    new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "client_id", client_id},
        { "client_secret", client_secret },
        { "code", query.Code },
        { "redirect_uri", query.Redirect_Success_Uri },
        { "scope", "openid profile email" },
        { "grant_type", "authorization_code" }
    })
);
```
#### 4: The API return a token
The token is added to an HttpOnly cookie.

Once we get the response from the API, we get redirected to the home page with a page reload to regenerate all the scoped services and the `AuthProvider`.

This service use the httpContextAccessor to get the token and extract the relevant claims on each page reload.

```c#
public class AuthProvider(IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.Delay(20);
        HttpContext? httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        string? accessToken = httpContext.Request.Cookies["disney_battle_auth_token"];

        if (accessToken is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jsonToken = handler.ReadJwtToken(accessToken);

        string? email = jsonToken.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
        string? id = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        string? provider = jsonToken.Claims.FirstOrDefault(c => c.Type == "Provider")?.Value;

        if (email is null || id is null || provider is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        List<Claim> claims =
        [
            new (nameof(User.Id), id),
            new (nameof(User.Email), email),
            new ("Provider", provider)
        ];
        ClaimsIdentity identity = new(claims, "cookieAuth");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

}
```

#### 5 Use AuthorizeView
We can now use `CascadingAuthenticationState` and `AuthorizeView` after registering the `AuthProvider` service.
```c#
builder.Services.AddTransient<AuthenticationStateProvider,AuthProvider>();
```

```html
<CascadingAuthenticationState>
    <Router AppAssembly="typeof(Program).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="typeof(Layout.MainLayout)" />
            <FocusOnNavigate RouteData="@routeData" Selector="h1" />
        </Found>
    </Router>
</CascadingAuthenticationState>
```

```html
<PageTitle>Home</PageTitle>

<div class="home-container">
    <Navbar />
    <div class="main">
        <AuthorizeView>
            <Authorized>
                <button @onclick="JoinGame" >Join Game</button>
            </Authorized>
        </AuthorizeView>
    </div>
</div>
```

#### 6 Graph
...
