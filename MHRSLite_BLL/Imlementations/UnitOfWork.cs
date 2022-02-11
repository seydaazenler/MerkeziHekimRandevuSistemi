﻿using MHRSLite_BLL.Contracts;
using MHRSLite_DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_BLL.Imlementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyContext _myContext;

        public UnitOfWork(MyContext myContext)
        {
            _myContext = myContext;
            //UnitOfWork tüm repositoryleri oluşturacak.
            CityRepository = new CityRepository(_myContext);
            DistrictRepository = new DistrictRepository(_myContext);
            DoctorRepository = new DoctorRepository(_myContext);
            PatientRepository = new PatientRepository(_myContext);
        }

        

        public ICityRepository CityRepository { get; private set; }
        public IDistrictRepository DistrictRepository { get; private set; }
        public IDoctorRepository DoctorRepository { get; private set; }
        public IPatientRepository PatientRepository { get; private set; }


        public void Dispose()
        {
            _myContext.Dispose();
        }
    }
}
