﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MHRSLite_EL.Models
{
    [Table("Districts")]
    public class District : Base<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "İlçe adı en az 2 en çok 50 karakter aralığında olmalıdır!")]
        public string DistrictName { get; set; }

        //City tablosuyla ilişki kuruldu.
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        //ilçedeki hastaneler -- Hastane tablosunda ilişki kuruldu
        public virtual List<Hospital> DistrictsHospitals { get; set; }
    }
}
