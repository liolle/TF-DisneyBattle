using System.ComponentModel.DataAnnotations;

namespace blazor.models;

public class LoginModel
{
  [Required]
  [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
  public string UserName { get; set; } = "";
  [Required]
  [DataType(DataType.Password)]
  public string Password { get; set; } = "";

}

public class LoginResult
{
  public bool IsSuccess { get; set; }

  public bool IsFailure { get; set; }

  public string? ErrorMessage { get; set; }

  public string? Result { get; set; }

  public string? Exception { get; } 
}
