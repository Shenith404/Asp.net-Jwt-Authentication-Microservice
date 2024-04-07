
using System.ComponentModel.DataAnnotations;


namespace Authentication.Core.DTOs
{
    public class LockOutDetailsInfoDTO
    {
        [Required]
        public string  Email { get; set; }
        public bool  LockoutEnable { get; set; }

        public DateTimeOffset LockoutEndDate   { get; set; } 


    }
}
