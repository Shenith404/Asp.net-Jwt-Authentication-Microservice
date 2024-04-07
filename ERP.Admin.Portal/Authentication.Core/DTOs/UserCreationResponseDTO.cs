using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Core.DTOs
{
    public class UserCreationResponseDTO
    {
        public string UserName { get; set; }



        public string  EmailConfirmedToken { get; set; } = string.Empty;
        public string Message { get; set; }
    }
}
