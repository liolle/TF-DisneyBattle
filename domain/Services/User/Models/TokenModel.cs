using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using disney_battle.cqs;
using disney_battle.dal.entities;
using disney_battle.domain.cqs.queries;
using disney_battle.exceptions;

namespace disney_battle.domain.services.models;

public class MicrosoftTokenModel(
    string token_type,
    int expires_in,
    int ext_expires_in,
    string access_token,
    string refresh_token = "",
    string id_token = "",
    string scope = "")
{
    public string Token_Type { get; } = token_type;
    public int Expires_In { get; } = expires_in;
    public int Ext_Expires_In { get; } = ext_expires_in;
    public string Access_Token { get; } = access_token;
    public string Refresh_Token { get; } = refresh_token;
    public string Id_Token { get; } = id_token;
    public string Scope { get; } = scope;


    public List<Claim> GetClaims(UserService userService)
    {

        var handler = new JwtSecurityTokenHandler();

        if (Access_Token is null) { throw new MalformedInputException("Microsoft Access_Token"); }

        JwtSecurityToken jsonToken = handler.ReadJwtToken(Access_Token);

        string email = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? throw new MalformedInputException("Microsoft Access_Token");

        QueryResult<AccountEntity> result = userService.Execute(new AccountFromProviderQuery(0,email,EProvider.microsoft));
     
        List<Claim> claims =
        [
            new(nameof(BaseTokenClaims.Id), result.Result.Id.ToString()),
            new(nameof(BaseTokenClaims.Email), email),
            new(nameof(BaseTokenClaims.Provider), EProvider.microsoft.ToString()),
        ];

        return claims;
    }
}

public enum EProvider
{
    credential,
    microsoft,
    google
}

public class BaseTokenClaims(int id, string email, EProvider provider)
{
    public int Id { get; set; } = id;
    public EProvider Provider { get; set; } = provider;
    public string Email { get; set; } = email;
}