using MHRSLite_EL.IdentityModels;
using MHRSLite_EL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_DAL
{
    public class MyContext : IdentityDbContext<AppUser,AppRole,string>
    {
        //CONSTRUCTOR METOD
        public MyContext(DbContextOptions<MyContext> options)
            //myCon yerine bu yazılır
            :base(options)
        {
            
        }
        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<Clinic> Clinic { get; set; }
        public virtual DbSet<HospitalClinic> HospitalClinics { get; set; }
        public virtual DbSet<AppointmentHour> AppointmentHours { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }


    }
}
