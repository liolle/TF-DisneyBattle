namespace disney_battle.dal.entities;

public enum ERole
{
  admin
}


public class RoleEntity
{
  internal RoleEntity(int account_Id, ERole role)
  {
    Account_Id = account_Id;
    Role = role;
  }

  public int Account_Id { get; set; }
  public ERole Role { get; set; }

  public static RoleEntity Create(int account_Id, ERole role){
    return new(account_Id,role);
  }
}
