
using disney_battle.exceptions;

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

    public static UserEntity Create(string id, string userName, string email,string password,string createdAt){
        if (!DateTime.TryParse(createdAt, out DateTime date)){
            throw new MalformedInputException("createdAt -> UserEntity.Create()");
        }

        if (!int.TryParse(id, out int parsed_id)){
            throw new MalformedInputException("Id -> UserEntity.Create()");
        }

        return new UserEntity( parsed_id,  userName,  email, password, date);
    }
}