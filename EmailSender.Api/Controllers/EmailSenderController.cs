using EmailSender.Application;
using EmailSenderCore.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmailSender.Api.Controllers
{
    public class EmailSenderController : ControllerBase
    {
        IEmailSender _emailSender;
        public EmailSenderController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequestDTO sendEmailRequestDTO)
        {

            if(ModelState.IsValid)
            {
                var resutl =await _emailSender.SendEmail(sendEmailRequestDTO.SenderEmail, sendEmailRequestDTO.Body); ;
                if(resutl == true)
                {
                    return Ok("Email is sent");
                }
            }
            return BadRequest();
        }
    }
}
