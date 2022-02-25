using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_EL.Enums
{
    public class AllEnums
    {
    }

    public enum Genders
    {
        Belirtilmemis,
        Bay,
        Bayan
    }

    public enum RoleNames:byte
    {
        Passive,
        Admin,
        Patient, //hasta
        PassiveDoctor,
        ActiveDoctor

    }
    public enum AppointmentStatus:byte
    {
        Past=0,
        Active=1,
        Cancelled=2
    }
}
