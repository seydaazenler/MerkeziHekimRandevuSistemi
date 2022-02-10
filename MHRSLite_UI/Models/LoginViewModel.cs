﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLite_UI.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı adı boş geçilemez!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Şifre alanı boş geçilemez!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifreniz minimum 6 karakterli olmalıdır!")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
