using System.ComponentModel.DataAnnotations;

namespace blazor.models;

public class RegisterModel
{
    [Required]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string UserName { get; set; } = "";
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = "";
}

public class RegisterResult
{
    public bool IsSuccess { get; set; }

    public bool IsFailure { get; set; }

    public string? ErrorMessage { get; set; }

    public string? Exception { get; } 
}