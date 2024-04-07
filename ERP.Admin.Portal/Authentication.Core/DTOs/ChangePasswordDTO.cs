using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Core.DTOs
{
    public class ChangePasswordDTO
    {
        public string NewPassword { get; set; }

        public string OldPassword { get; set; }
    }
}
