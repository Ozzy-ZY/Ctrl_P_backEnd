using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthModels
{
    public class AppUserRegisterationDto(string UserName, string Email, string Password)
    {
        public string UserName { get; } = UserName;

        [EmailAddress]
        public string Email { get; set; } = Email;

        public string Password { get; } = Password;
    }
}