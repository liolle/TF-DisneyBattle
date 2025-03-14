
namespace disney_battle.dal.entities;

public class UserEntity
{
  public int Id {get;}
  public string Email {get;set;}
  public DateTime Created_At {get;}

  internal UserEntity(int id, string email,DateTime createdAt)
  {
    Id = id;
    Email = email;
    Created_At = createdAt;
  }

  public static UserEntity Create(int id, string email, DateTime createdAt){
    return new UserEntity( id,  email, createdAt);
  }
}
