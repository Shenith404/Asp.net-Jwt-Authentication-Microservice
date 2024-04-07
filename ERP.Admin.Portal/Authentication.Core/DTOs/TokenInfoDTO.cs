using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Core.DTOs
{
    public class TokenInfoDTO
    {
        public string  JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
