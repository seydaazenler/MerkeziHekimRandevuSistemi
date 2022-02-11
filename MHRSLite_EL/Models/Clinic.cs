using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MHRSLite_EL.Models
{
    [Table("Clinics")]
    public class Clinic:Base<int>
    {
        [Required]
        [StringLength(100,MinimumLength =2,ErrorMessage ="Klinik adı en az 2 en çok 100 karakter olmalıdır!")]
        public string ClinicName { get; set; }

        //HospitalClinics tablosunda clinic ile ilişki kuruldu
        public virtual List<HospitalClinics> HospitalClinics { get; set; }
    }
}
