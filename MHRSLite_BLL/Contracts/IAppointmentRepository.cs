﻿using MHRSLite_EL.Models;
using MHRSLite_EL.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_BLL.Contracts
{
    public interface IAppointmentRepository : IRepositoryBase<Appointment>
    {
        //Gideceği randevular 
        List<AppointmentVM> GetUpComingAppointments(string patientid);
        //Geçmiş randevular
        List<AppointmentVM> GetPastAppointments(string patientid);

        //Randevu aldıktan sonra email içinde pdf halinde randevu bilgilerini göndermek için randevuyu bulmak gerekir
        AppointmentVM GetAppointmentByID(string patientid,int hcid,DateTime appointmentDate,string appointmentHour);
        List<AppointmentVM> GetAppointmentsIM(DateTime? dt);
    }
    
}
