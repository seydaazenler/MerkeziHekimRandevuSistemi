using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_BLL.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        ICityRepository CityRepository { get; }
        IDistrictRepository DistrictRepository { get; }
        IDoctorRepository DoctorRepository { get; }

        IPatientRepository PatientRepository { get; }
    }
}
