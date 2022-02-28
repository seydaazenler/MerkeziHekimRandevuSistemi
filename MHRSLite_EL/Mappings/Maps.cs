using AutoMapper;
using MHRSLite_EL.Models;
using MHRSLite_EL.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_EL.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            //Appointment'ı AppointmentVMye dönüştür
            CreateMap<Appointment, AppointmentVM>();
            CreateMap<AppointmentVM, Appointment>();

            //ReverseMap sayesinde yukarıdaki 2 ayrı işlemi tek satırda yapmış oluruz
            //Dönüştürmek istenilen ilk yazılır
            CreateMap<AppointmentVM, Appointment>().ReverseMap();
        }
    }
}
