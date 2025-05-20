using System.Text.Json.Serialization;

namespace disney_battle.dal.entities;

public class PersonageEntity
{
  [JsonPropertyName("id")]
  public int Id { get; }
  [JsonPropertyName("name")]
  public string Name { get; set; }
  [JsonPropertyName("basehp")]
  public int BaseHp { get; set; }
  [JsonPropertyName("baseatk")]
  public int BaseAtk { get; set; }
  [JsonPropertyName("basedef")]
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
    return new PersonageEntity(id, name, baseHp, baseAtk, baseDef);
  }

}
