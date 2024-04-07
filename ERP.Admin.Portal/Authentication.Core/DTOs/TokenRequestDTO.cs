using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Core.DTOs
{
    public class TokenRequestDTO
    {
 
        [Required]
        public string UserName { get; set; }
     
        [Required]
        public string Role { get; set; }    

        [Required]
        public string  UserId { get; set; }
    }
}
