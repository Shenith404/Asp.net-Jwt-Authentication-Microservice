

using System.ComponentModel.DataAnnotations;

namespace ERP.Authentication.Core.DTOs
{
    public class AuthenticationRequestDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }


    }
}
