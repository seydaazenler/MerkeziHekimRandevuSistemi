using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLite_UI.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Yeni Şifre alanı boş geçilemez!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifreniz minimum 6 karakterli olmalıdır!")]
        [Display(Name = "Yeni Şifre")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Yeni Şifre Tekrar alanı boş geçilemez!")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare(nameof(NewPassword), ErrorMessage = "Şifreler uyuşmuyor")]
        public string ConfirmNewPassword { get; set; }

        public string Code { get; set; }
        public string UserId { get; set; }

    }
}
