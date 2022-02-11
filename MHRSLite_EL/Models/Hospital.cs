using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MHRSLite_EL.Models
{
    [Table("Hospitals")]
    public class Hospital :Base<int>
    {
        [Required]
        [StringLength(400,MinimumLength =2,ErrorMessage ="Hastane adı en az 2 en çok 400 karakter olmalıdır!")]
        public string HospitalName { get; set; }

        public int DistrictId { get; set; }

        //ilçe tablosuyla ilişki kuruldu

        [ForeignKey("DistrictId")]
        public virtual District HospitalDistrict { get; set; }

        //HospitalClinics tablosunda ilişki kurulmuştur
        public virtual List<HospitalClinics> HospitalClinics { get; set; }
    }
}
