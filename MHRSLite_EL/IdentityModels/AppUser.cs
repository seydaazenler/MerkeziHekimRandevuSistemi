using MHRSLite_EL.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MHRSLite_EL.IdentityModels
{
    public class AppUser : IdentityUser
    {
        //kullanıcı özellikleri girilecek,
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Surname { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public string Picture { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        //Gender alma(Cinsiyet)
        public Genders Gender { get; set; }

    }
}
