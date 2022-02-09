
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MHRSLite_EL.IdentityModels
{
    public class AppRole : IdentityRole
    {
        [StringLength(400,ErrorMessage="Role açıklamasına en fazla 400 karakter girilebilir!")]
        public string Description { get; set; }
    }
}
