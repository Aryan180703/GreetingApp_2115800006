using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Interface
{
    public interface IEmailService
    {
        public bool SendEmail(string toEmail, string subject, string body);
    }
}
