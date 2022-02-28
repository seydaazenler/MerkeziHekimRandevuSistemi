using MHRSLite_EL;
using MHRSLite_EL.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MHRSLite_BLL
{
    public  interface IEmailSender
    {
        Task SendAsync(EmailMessage message);

        void SendAppointmentPdf(EmailMessage message, AppointmentVM data);

        Task SendAppointmentPdfAsync(EmailMessage message, AppointmentVM data);
    }
}
