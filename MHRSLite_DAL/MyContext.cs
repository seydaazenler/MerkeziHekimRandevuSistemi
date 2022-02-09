using MHRSLite_EL.IdentityModels;
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

        
    }
}
