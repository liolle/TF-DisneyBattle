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
}