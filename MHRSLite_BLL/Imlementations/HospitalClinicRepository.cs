﻿using MHRSLite_BLL.Contracts;
using MHRSLite_DAL;
using MHRSLite_EL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_BLL.Imlementations
{
    public class HospitalClinicRepository:Repository<HospitalClinic>, IHospitalClinicRepository
    {
        private readonly MyContext _myContext;

        public HospitalClinicRepository(MyContext myContext)
            : base(myContext)
        {
            _myContext = myContext;
        }
    }
}
