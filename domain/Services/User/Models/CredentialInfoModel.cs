namespace disney_battle.domain.services.models;

public class CredentialInfoModel(int id, string email, string password, DateTime created_At)
{
    public int Id { get; } = id;
    public string Email { get; } = email;
    public string Password { get; } = password;
    public DateTime Created_At { get; } = created_At;
}