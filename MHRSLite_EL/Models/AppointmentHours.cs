﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MHRSLite_EL.Models
{
    public class AppointmentHours :Base<int>
    {
        public int HospitalClinicId { get; set; }

        [Required]
        public string Hours { get; set; }

        [ForeignKey("HospitalClinicId")]
        public virtual HospitalClinics GetHospitalClinics { get; set; }
    }
}
