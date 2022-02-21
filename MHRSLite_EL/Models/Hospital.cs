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
        [StringLength(500,ErrorMessage ="Adres bilgisi en fazla 500 karakter olmalıdır!")]

        public string Address { get; set; }
        [StringLength(10,MinimumLength =10,ErrorMessage="Başında 0 olmadan 10 karakter olacak şekilde girilmelidir!")]

        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        //ilçe tablosuyla ilişki kuruldu

        [ForeignKey("DistrictId")]
        public virtual District HospitalDistrict { get; set; }

        //HospitalClinics tablosunda ilişki kurulmuştur
        public virtual List<HospitalClinic> HospitalClinics { get; set; }
    }
}
