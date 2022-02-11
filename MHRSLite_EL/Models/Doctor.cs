using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MHRSLite_EL.Models
{
    [Table("Doctors")]
    public class Doctor:Base<string>
    {
        public virtual List<HospitalClinics> HospitalClinics { get; set; }

    }
}
