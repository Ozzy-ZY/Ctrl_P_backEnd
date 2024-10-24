using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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