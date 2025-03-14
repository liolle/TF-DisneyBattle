namespace disney_battle.dal.entities;

public class PersonageEntity
{
  public int Id { get; }
  public string Name { get; set; }
  public int BaseHp { get; set; }
  public int BaseAtk { get; set; }
  public int BaseDef { get; set; }

  internal PersonageEntity(int id, string name, int baseHp, int baseAtk, int baseDef)
  {
    Id = id;
    Name = name;
    BaseHp = baseHp;
    BaseAtk = baseAtk;
    BaseDef = baseDef;
  }

  public static PersonageEntity Create(int id, string name, int baseHp, int baseAtk, int baseDef)
  {
    return new PersonageEntity(id,name,baseHp,baseAtk,baseDef);
  }

}
