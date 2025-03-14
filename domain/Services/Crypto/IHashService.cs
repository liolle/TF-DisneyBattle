namespace disney_battle.domain.services;

public interface IHashService
{
  public string HashPassword(string password);
  public bool VerifyPassword(string hashedPassword, string password);
}
