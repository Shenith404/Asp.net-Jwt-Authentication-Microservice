using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender.Application
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(string email, string body);

    }
}
