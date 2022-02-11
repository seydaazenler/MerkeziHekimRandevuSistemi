using MHRSLite_EL.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MHRSLite_EL.Models
{
    [Table("Doctors")]
    public class Doctor:PersonBase
    {
        public string UserId { get; set; }// Identity Model'in ID değeri burada Foreign Key olacaktır.
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
        public virtual List<HospitalClinic> HospitalClinics { get; set; }

    }
}
