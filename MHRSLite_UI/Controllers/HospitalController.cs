using MHRSLite_BLL;
using MHRSLite_BLL.Contracts;
using MHRSLite_EL.IdentityModels;
using MHRSLite_EL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLite_UI.Controllers
{
    public class HospitalController : Controller
    {
        //Global Alan

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly RoleManager<AppRole> _roleManager;

        private readonly IEmailSender _emailSender;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfiguration _configuration;

        public HospitalController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public JsonResult GetHospitalFromClinicId(int id, int districtId)
        {
            try
            {
                var data = new List<Hospital>();
                if (id > 0 && districtId > 0)
                {
                    data = _unitOfWork.HospitalClinicRepository
                        .GetAll(x => x.ClinicId == id)
                        .Select(y => y.Hospital)
                        .Where(x => x.DistrictId == districtId)
                        .Distinct().ToList();
                }
                return Json(new { isSuccess = true, data });
            }
            catch (Exception)
            {
                return Json(new { isSuccess = false });
            }
        }
    }
}
