namespace disney_battle.dal.entities;

public class AccountEntity
{
  internal AccountEntity(int id, string provider, int user_Id, string provider_Id)
  {
    Id = id;
    Provider = provider;
    User_Id = user_Id;
    Provider_Id = provider_Id;
  }

  public int Id {get;}
  public string Provider {get;set;}
  public int User_Id {get;set;}
  public string Provider_Id {get;set;}

  public static AccountEntity Create(int id, string provider, int user_Id, string provider_Id){
    return new(id,provider,user_Id,provider_Id);
  }
}
