using MHRSLite_BLL.Contracts;
using MHRSLite_DAL;
using MHRSLite_EL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_BLL.Imlementations
{
    public class HospitalClinicRepository:Repository<HospitalClinic>, IHospitalClinicRepository
    {
        

        public HospitalClinicRepository(MyContext myContext)
            : base(myContext)
        {
            
        }
    }
}
