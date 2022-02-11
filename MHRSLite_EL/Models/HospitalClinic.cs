using MHRSLite_EL.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MHRSLite_EL.Models
{
    [Table("HospitalClinics")]
    public class HospitalClinic :Base<int>
    {
        //Hospital ile ilişki kuruldu
        public int HospitalId { get; set; }

        [ForeignKey("HospitalId")]
        public virtual Hospital Hospital { get; set; }
        //Clinic ile ilişki kuruldu
        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; }

        //Doctor ile ilişki kuruldu
        public string DoktorId { get; set; }
        [ForeignKey("DoktorId")]
        public virtual Doctor Doctor { get; set; }
        public virtual List<AppointmentHour> AppointmentHours { get; set; }

        public virtual List<Appointment> ClinicAppointments { get; set; }
     }
}
