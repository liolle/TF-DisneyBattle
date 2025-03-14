using disney_battle.cqs;
using disney_battle.dal.entities;
using disney_battle.domain.services.models;

namespace disney_battle.domain.cqs.queries;


public class CredentialLoginQuery(string username, string password, string redirect_success_uri,string redirect_failure_uri ) : IQueryDefinition<string>
{
  public string UserName { get; } = username;
  public string Password { get; } = password;
  public string Redirect_Success_Uri { get; } = redirect_success_uri;
  public string Redirect_Failure_Uri { get; } = redirect_failure_uri;
}

public class OauthMicrosoftQuery(string code, string redirect_success_uri,string redirect_failure_uri ) : IQueryDefinition<Task<string>>
{
  public string Code { get; } = code;
  public string Redirect_Success_Uri { get; } = redirect_success_uri;
  public string Redirect_Failure_Uri { get; } = redirect_failure_uri;
}

public class AccountFromProviderQuery(int? user_Id, string? provider_id , EProvider provider = EProvider.credential) : IQueryDefinition<AccountEntity>
{
  public int? User_Id {get;set;} = user_Id;
  public string? Provider_id {get;set;} = provider_id;
  public EProvider Provider {get;set;} = provider;
}
