using MHRSLite_EL.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_EL.IdentityModels
{
    public class AppUser : IdentityUser
    {
        //kullanıcı özellikleri girilecek
        public string Picture { get; set; }
        public DateTime? BirthDate { get; set; }

        //Gender alma(Cinsiyet)
        public Genders Gender { get; set; }

    }
}
