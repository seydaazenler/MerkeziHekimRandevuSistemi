﻿using MHRSLite_BLL;
using MHRSLite_BLL.Contracts;
using MHRSLite_EL.IdentityModels;
using MHRSLite_EL.Models;
using MHRSLite_UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLite_UI.Controllers
{
    public class PatientController : Controller
    {
        //Global Alan
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly RoleManager<AppRole> _roleManager;

        private readonly IEmailSender _emailSender;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfiguration _configuration;

        public PatientController(
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

        [Authorize]
        public IActionResult Index()
        {
            //geçmiş randevular olacak
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [Authorize]
        public IActionResult Appointment()
        {
            try
            {
                //illerin sıralama işlemini yaptık
                ViewBag.Cities = _unitOfWork.CityRepository.GetAll(orderBy:x=> x.OrderBy(a=> a.CityName));
                ViewBag.Clinics = _unitOfWork.ClinicRepository.GetAll(orderBy: x => x.OrderBy(a => a.ClinicName));
                return View();
            }
            catch (Exception ex)
            {

                return View();
            }
        }

        [Authorize]
        public IActionResult FindAppointment(int cityid, int? distid, int cid, int? hid, int? dr)
        {
            try
            {
                var data = _unitOfWork.HospitalClinicRepository.GetAll(x => x.ClinicId == cid && x.HospitalId == hid.Value)
                                                               .Select(a => a.AppointmentHours).ToList();
                //buraya geri dönülecek
                var list = new List<AvailableDoctorAppointmentViewModel>();
                foreach (var item in data)
                {
                    foreach (var subitem in item)
                    {
                        var hospitalClinicData = _unitOfWork.HospitalClinicRepository.GetFirstOrDefault(x => x.Id == subitem.HospitalClinicId);

                        var hours = subitem.Hours.Split(',');
                        var appointment = _unitOfWork.AppointmentRepository.GetAll(x => x.HospitalClinicId == subitem.HospitalClinicId && (x.AppointmentDate > DateTime.Now.AddDays(-1) && x.AppointmentDate < DateTime.Now.AddDays(2))).ToList();
                        foreach (var houritem in hours)
                        {
                            if (appointment.Count(x => x.AppointmentDate == (Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString())) && x.AppointmentHour == houritem) == 0)
                            {
                                list.Add(new AvailableDoctorAppointmentViewModel()
                                {
                                    HospitalClinicId = subitem.HospitalClinicId,
                                    ClinicId = hospitalClinicData.ClinicId,
                                    HospitalId = hospitalClinicData.HospitalId,
                                    DoctorTCNumber = hospitalClinicData.DoktorId,
                                    Doctor = _unitOfWork.DoctorRepository.GetFirstOrDefault(x => x.TCNumber == hospitalClinicData.DoktorId, includeProperties: "AppUser"),
                                    Hospital = _unitOfWork.HospitalRepository.GetFirstOrDefault(x => x.Id == hospitalClinicData.HospitalId),
                                    Clinic = _unitOfWork.ClinicRepository.GetFirstOrDefault(x => x.Id == hospitalClinicData.ClinicId),
                                    HospitalClinic = hospitalClinicData
                                });
                                break;
                            }
                        }
                    }
                }
                list = list.Distinct().OrderBy(x => x.Doctor.AppUser.Name).ToList();
                return View(list);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [Authorize]
        public IActionResult FindAppointmentHours(int hcid)
        {
            try
            {
                var list = new List<AvailableDoctorAppointmentHoursViewModel>();

                var data = _unitOfWork.AppointmentHourRepository
                     .GetFirstOrDefault(x => x.HospitalClinicId == hcid);
                var hospitalClinicData =
                         _unitOfWork.HospitalClinicRepository
                         .GetFirstOrDefault(x => x.Id == hcid);

                var hours = data.Hours.Split(',');
                var appointment = _unitOfWork
                    .AppointmentRepository
                    .GetAll(
                    x => x.HospitalClinicId == hcid
                    &&
                    (x.AppointmentDate > DateTime.Now.AddDays(-1)
                    &&
                    x.AppointmentDate < DateTime.Now.AddDays(2)
                    )
                    ).ToList();
                foreach (var houritem in hours)
                {
                    string myHourBase = houritem.Substring(0, 2) + ":00";
                    var appointmentHourData =
                        new AvailableDoctorAppointmentHoursViewModel()
                        {
                            AppointmentDate = DateTime.Now.AddDays(1),
                            Doctor =
                               _unitOfWork.DoctorRepository
                               .GetFirstOrDefault(x => x.TCNumber == hospitalClinicData.DoktorId),
                            HourBase = myHourBase
                        };
                    list.Add(appointmentHourData);
                    if (appointment.Count(
                        x =>
                        x.AppointmentDate == (
                        Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString())) &&
                        x.AppointmentHour == houritem
                        ) == 0)
                    {
                        if (list.Count(x => x.HourBase == myHourBase) > 0)
                        {
                            appointmentHourData.Hours.Add(houritem);
                        }
                    }

                }

                list = list.Distinct().ToList();
                return View(list);


            }
            catch (Exception)
            {

                throw;
            }

        }


        [Authorize]
        public IActionResult FindAppointmentPrevious(int cityid,int? distid, int cid, int? hid, int? dr)
        {
            try
            {
                //Dışarıdan gelen clinicId'nin olduğu HospitalClinic kayıtlarını al
                var data = _unitOfWork.HospitalClinicRepository.GetAll(x => x.ClinicId == cid && x.HospitalId == hid.Value)
                                                               .Select(a => a.AppointmentHours).ToList();

                var list = new List<PatientAppointmentViewModel>();
                foreach (var item in data)
                {
                    foreach (var subitem in item)
                    {
                        var hospitalClinicData = _unitOfWork.HospitalClinicRepository.GetFirstOrDefault(x => x.Id == subitem.HospitalClinicId);

                        //o hastanede o klinikteki çalışma ssatleri çekildi
                        var hours = subitem.Hours.Split(',');
                        var appointment = _unitOfWork.AppointmentRepository.GetAll(x=> x.HospitalClinicId == subitem.HospitalClinicId
                                                                                   && (x.AppointmentDate>DateTime.Now.AddDays(-1)
                                                                                   && x.AppointmentDate < DateTime.Now.AddDays(2)
                                                                                   )
                                                                                   ).ToList();
                        foreach (var houritem in hours)
                        {
                            if (appointment.Count(x=> x.AppointmentDate==(Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString()))
                            && x.AppointmentHour == houritem) ==0)
                            {
                                list.Add(new PatientAppointmentViewModel()
                                {
                                    AppointmentDate = Convert.ToDateTime(DateTime.Now.AddDays(1)),
                                    HospitalClinicId = subitem.HospitalClinicId,
                                    DoctorId = hospitalClinicData.DoktorId,
                                    AvailableHour = houritem,
                                    Doctor = _unitOfWork.DoctorRepository.GetFirstOrDefault(x => x.TCNumber == hospitalClinicData.DoktorId,
                                    includeProperties: "AppUser")
                                });
                            }
                        }
                    }
                }

                list = list.Distinct().OrderBy(x=> x.AppointmentDate).ToList();
                return View(list);
                                                              
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult SaveAppointment(int hid, string date,string hour)
        {
            try
            {
                //randevu kayıt edilecek
                //aynı saate ve tarihe başka randevusu var mı?
                DateTime appointmentDate = Convert.ToDateTime(date);
                if (_unitOfWork.AppointmentRepository.GetFirstOrDefault(x=> x.AppointmentDate ==  appointmentDate && x.AppointmentHour==hour)!=null)
                {
                    //aynı tarih ve saatte randevusu varsa
                    TempData["SaveAppointmentsStatus"] = $"{date} - {hour} tarihinde bir kliniğe zaten randevu almışsınız." +
                        $" Aynı tarih ve saate başka randevu alınamaz!";
                    return RedirectToAction("Index", "Patient");
                }

                Appointment patientAppointment = new Appointment()
                {
                    CreatedDate=DateTime.Now,
                    PatientId = HttpContext.User.Identity.Name,
                    HospitalClinicId = hid,
                    AppointmentDate=appointmentDate,
                    AppointmentHour=hour
                    
                };

                var result = _unitOfWork.AppointmentRepository.Add(patientAppointment);
                TempData["SaveAppointmentStatus"] = result ? "Randevunuz başarıyla oluşturuldu" : "HATA: Beklenmedik bir hata oluştu!";

                return RedirectToAction("Index", "Patient");
            }
            catch (Exception ex)
            {
                TempData["SaveAppointmentStatus"] = "HATA: " + ex.Message;
                return RedirectToAction("Index", "Patient");
                
            }
        }
    }
}
