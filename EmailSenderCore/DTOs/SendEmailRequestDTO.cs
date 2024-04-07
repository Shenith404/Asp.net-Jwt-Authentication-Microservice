using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSenderCore.DTOs
{
    public class SendEmailRequestDTO
    {
        [Required]
        public string SenderEmail { get; set; }

        [Required]
        public string Body { get; set; }


    }
}
