using MHRSLite_EL.Enums;
using MHRSLite_EL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MHRSLite_EL.IdentityModels
{
    public class AppUser : IdentityUser
    {
        //kullanıcı özellikleri girilecek
        [Required]
        [StringLength(50,MinimumLength =2,ErrorMessage ="Adınız en az 2 en fazla 50 karakter olmalıdır!")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Soyadınız en az 2 en fazla 50 karakter olmalıdır!")]
        public string Surname { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; } = DateTime.Now;

        public string Picture { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        //Gender alma(Cinsiyet)
        [Required(ErrorMessage ="Cinsiyet seçimi gereklidir!")]
        public Genders Gender { get; set; }

        //Doctor tablosunda ilişkisi kuruldu
        public virtual List<Doctor> Doctors { get; set; }
        //Patient tablosunda ilişkisi kuruldu
        public virtual List<Patient> Patients { get; set; }


    }
}
