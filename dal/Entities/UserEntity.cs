
namespace disney_battle.dal.entities;

public class UserEntity
{
    public int Id {get;}
    public string UserName {get;set;}
    public string Email {get;set;}
    public string Password {get;set;}
    public DateTime CreatedAt {get;}

    internal UserEntity(int id, string userName, string email,string password,DateTime createdAt)
    {
        Id = id;
        UserName = userName;
        Email = email;
        Password = password;
        CreatedAt = createdAt;
    }

    public static UserEntity Create(int id, string userName, string email,string password,DateTime createdAt){
        return new UserEntity( id,  userName,  email, password, createdAt);
    }
}