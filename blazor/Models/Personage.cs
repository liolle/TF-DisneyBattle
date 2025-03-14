namespace blazor.models;

public class Personage(int id, string name, int baseHp, int baseAtk, int baseDef)
{
  public int Id { get; set; } = id;
  public string Name { get; set; } = name;
  public int BaseHp { get; set; } = baseHp;
  public int BaseAtk { get; set; } = baseAtk;
  public int BaseDef { get; set; } = baseDef;
}
