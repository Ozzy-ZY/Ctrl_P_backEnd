using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuthModels
{
    public record AppUserLoginDto(string UserName, string Password)
    {
    }
}