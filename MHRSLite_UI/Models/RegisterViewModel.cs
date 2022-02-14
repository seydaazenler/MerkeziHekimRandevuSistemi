using MHRSLite_EL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLite_UI.Models
{
    public class RegisterViewModel
    {
        [Display(Name="TC Kimlik Numarası")]
        [MinLength(11)]
        [StringLength(11, ErrorMessage = "TC Kimlik numarası 11 haneli olmalıdır!")]
        public string TCNumber { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Ad alanı boş geçilemez!")]
        [Display(Name = "Ad")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad alanı boş geçilemez!")]
        [Display(Name = "Soyad")]
        [StringLength(50)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "EMail alanı boş geçilemez!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş geçilemez!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifreniz minimum 6 karakterli olmalıdır!")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Cinsiyet alanı boş geçilemez!")]
        public Genders Gender { get; set; }

        [Display(Name="Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

    }
}
