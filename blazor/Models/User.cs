namespace blazor.models;

public class User(int id, string email)
{
  public int Id { get; set; } = id;
  public string Email { get; set; } = email;
}
