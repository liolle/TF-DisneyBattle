using System.Security.Claims;

namespace disney_battle.domain.services.models;

public class CredentialInfoModel(int id, string email, string password, DateTime created_At)
{
    public int Id { get; } = id;
    public string Email { get; } = email;
    public string Password { get; } = password;
    public DateTime Created_At { get; } = created_At;


    public List<Claim>  GetClaims(){
        List<Claim> claims =
        [
            new(nameof(BaseTokenClaims.Id), Id.ToString()),
            new(nameof(BaseTokenClaims.Email), Email),
            new(nameof(BaseTokenClaims.Provider), EProvider.credential.ToString()),
        ];

        return claims;
    }
}