namespace Application.DTOs;

public class ChangePasswordDTO
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public bool PasswordConfirmation  { get; set; }
}