using MHRSLite_BLL.Contracts;
using MHRSLite_DAL;
using MHRSLite_EL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_BLL.Imlementations
{
    public class DoctorRepository :Repository<Doctor>,IDoctorRepository
    {
        private MyContext myContext;

        public DoctorRepository(MyContext myContext)
            :base(myContext)
        {

        }
        
    }
}
