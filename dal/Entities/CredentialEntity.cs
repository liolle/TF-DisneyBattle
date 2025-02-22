namespace disney_battle.dal.entities;

public class CredentialEntity
{
    internal CredentialEntity(int account_Id, string user_name, string password)
    {
        Account_Id = account_Id;
        User_name = user_name;
        Password = password;
    }

    public int Account_Id {get;set;}
    public string User_name {get;set;}
    public string Password {get;set;}

    public static CredentialEntity Create(int account_Id, string user_name, string password){
        return new CredentialEntity(account_Id,user_name,password);
    }
}