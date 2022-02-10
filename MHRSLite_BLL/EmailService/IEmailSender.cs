using MHRSLite_EL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MHRSLite_BLL
{
    public  interface IEmailSender
    {
        Task SendAsync(EmailMessage message);
    }
}
