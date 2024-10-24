using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuthModels
{
    public class RequestTokenModel
    {
        public string ExpiredToken { get; set; }
        public string RefreshToken {get; set; }
    }
}
