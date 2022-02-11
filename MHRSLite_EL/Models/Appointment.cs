using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MHRSLite_EL.Models
{
    public class Appointment : Base<int>
    {
        public string PatientId { get; set; }
        public int HospitalClinicId { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        [Required]
        [StringLength(5,MinimumLength =5,ErrorMessage ="Randevu saati XX:XX şeklinde olmalıdır!")]
        public string AppointmentHour { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        public virtual HospitalClinics HospitalClinic { get; set; }
    }
}
