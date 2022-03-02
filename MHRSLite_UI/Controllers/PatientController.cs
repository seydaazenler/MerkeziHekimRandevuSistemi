﻿using ClosedXML.Excel;
using MHRSLite_BLL;
using MHRSLite_BLL.Contracts;
using MHRSLite_EL;
using MHRSLite_EL.Enums;
using MHRSLite_EL.IdentityModels;
using MHRSLite_EL.Models;
using MHRSLite_UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MHRSLiteEntityLayer.Constants;

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
        public IActionResult Index(int pageNumberPast = 1, int pageNumberFuture = 1)
        {
            try
            {
                ViewBag.PageNumberPast = pageNumberPast;
                ViewBag.PageNumberFuture = pageNumberFuture;
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
                ViewBag.Cities = _unitOfWork.CityRepository.GetAll(orderBy: x => x.OrderBy(a => a.CityName));
                ViewBag.Clinics = _unitOfWork.ClinicRepository.GetAll(orderBy: x => x.OrderBy(a => a.ClinicName));
                return View();
            }
            catch (Exception ex)
            {

                return View();
            }
        }

        [Authorize]
        public IActionResult FindAppointment(int cityid, int? distid,
             int cid, int? hid, int? dr)
        {
            try
            {
                TempData["ClinicId"] = cid;
                TempData["HospitalId"] = hid.Value;

                //Dışarıdan gelen hid ve clinicid'nin olduğu HospitalClinic kayıtlarını al
                var data = _unitOfWork.HospitalClinicRepository
                    .GetAll(x => x.ClinicId == cid
                    && x.HospitalId == hid.Value)
                    .Select(a => a.AppointmentHours)
                    .ToList();
                var list = new List<AvailableDoctorAppointmentViewModel>();
                foreach (var item in data)
                {
                    foreach (var subitem in item)
                    {
                        var hospitalClinicData =
                            _unitOfWork.HospitalClinicRepository
                            .GetFirstOrDefault(x => x.Id == subitem.HospitalClinicId);
                        var hours = subitem.Hours.Split(',');
                        var appointment = _unitOfWork
                            .AppointmentRepository
                            .GetAll(
                            x => x.HospitalClinicId == subitem.HospitalClinicId
                            &&
                            (x.AppointmentDate > DateTime.Now.AddDays(-1)
                            &&
                            x.AppointmentDate < DateTime.Now.AddDays(2)
                            )
                            ).ToList();
                        foreach (var houritem in hours)
                        {
                            if (appointment.Count(
                                x =>
                                x.AppointmentDate == (
                                Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString())) &&
                                x.AppointmentHour == houritem
                                ) == 0)
                            {
                                list.Add(new AvailableDoctorAppointmentViewModel()
                                {
                                    HospitalClinicId = subitem.HospitalClinicId,
                                    ClinicId = hospitalClinicData.ClinicId,
                                    HospitalId = hospitalClinicData.HospitalId,
                                    DoctorTCNumber = hospitalClinicData.DoktorId,
                                    Doctor = _unitOfWork.DoctorRepository
                                    .GetFirstOrDefault(x => x.TCNumber ==
                                    hospitalClinicData.DoktorId, includeProperties: "AppUser"),
                                    Hospital = _unitOfWork.HospitalRepository
                                    .GetFirstOrDefault(x => x.Id ==
                                    hospitalClinicData.HospitalId),
                                    Clinic = _unitOfWork.ClinicRepository
                                    .GetFirstOrDefault(x => x.Id == hospitalClinicData.ClinicId),
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

                var data = _unitOfWork.
                    AppointmentHourRepository.
                    GetFirstOrDefault(x => x.HospitalClinicId == hcid);

                var hospitalClinicData =
                         _unitOfWork.HospitalClinicRepository
                         .GetFirstOrDefault(x => x.Id == hcid);
                Doctor dr = _unitOfWork.DoctorRepository
                    .GetFirstOrDefault(x => x.TCNumber == hospitalClinicData.DoktorId
                    , includeProperties: "AppUser");
                ViewBag.Doctor = "Dr." + dr.AppUser.Name + " " + dr.AppUser.Surname;


                var hours = data.Hours.Split(',');

                var appointment = _unitOfWork
                    .AppointmentRepository
                    .GetAll(
                    x => x.HospitalClinicId == hcid
                    &&
                    (x.AppointmentDate > DateTime.Now.AddDays(-1)
                    &&
                    x.AppointmentDate < DateTime.Now.AddDays(2)
                    &&
                    x.AppointmentStatus != AppointmentStatus.Cancelled
                    )
                    ).ToList();

                foreach (var houritem in hours)
                {
                    string myHourBase = houritem.Substring(0, 2) + ":00";
                    var appointmentHourData =
                      new AvailableDoctorAppointmentHoursViewModel()
                      {
                          AppointmentDate = DateTime.Now.AddDays(1),
                          Doctor = dr,
                          HourBase = myHourBase,
                          HospitalClinicId = hcid
                      };
                    if (list.Count(x => x.HourBase == myHourBase) == 0)
                    {
                        list.Add(appointmentHourData);
                    }
                    if (appointment.Count(
                        x =>
                        x.AppointmentDate == (
                        Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString())) &&
                        x.AppointmentHour == houritem
                        ) == 0)
                    {
                        if (list.Count(x => x.HourBase == myHourBase) > 0)
                        {
                            list.Find(x => x.HourBase == myHourBase
                                ).Hours.Add(houritem);
                        }
                    }
                }
                return View(list);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [Authorize]
        public IActionResult FindAppointmentPrevious(int cityid, int? distid, int cid, int? hid, int? dr)
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
                        var appointment = _unitOfWork.AppointmentRepository.GetAll(x => x.HospitalClinicId == subitem.HospitalClinicId
                                                                                   && (x.AppointmentDate > DateTime.Now.AddDays(-1)
                                                                                   && x.AppointmentDate < DateTime.Now.AddDays(2)
                                                                                   )
                                                                                   ).ToList();
                        foreach (var houritem in hours)
                        {
                            if (appointment.Count(x => x.AppointmentDate == (Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString()))
                            && x.AppointmentHour == houritem) == 0)
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

                list = list.Distinct().OrderBy(x => x.AppointmentDate).ToList();
                return View(list);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public JsonResult SaveAppointment(int hcid, string date, string hour)
        {
            var message = string.Empty;
            try
            {

                //randevu kayıt edilecek
                //aynı saate ve tarihe başka randevusu var mı?
                DateTime appointmentDate = Convert.ToDateTime(date);
                if (_unitOfWork.AppointmentRepository
                    .GetFirstOrDefault(x => x.AppointmentDate == appointmentDate
                    && x.AppointmentHour == hour
                    && x.AppointmentStatus != AppointmentStatus.Cancelled
                    ) != null)
                {
                    //aynı tarih ve saatte randevusu varsa
                    message = $"{date} - {hour} tarihinde bir kliniğe zaten randevu almışsınız." +
                        $" Aynı tarih ve saate başka randevu alınamaz!";
                    return Json(new { isSuccess = false, message });

                    #region RomatologyAppointment_ClaimsCheck
                    // Eğer romatoloji randevusu istenmiş ise
                    var hcidData = _unitOfWork.HospitalClinicRepository
                                        .GetFirstOrDefault(x =>
                                            x.Id == hcid,
                                            includeProperties: "Hospital,Clinic,Doctor");
                    if (hcidData.Clinic.ClinicName == ClinicsConstants.ROMATOLOGY)
                    {
                        //claim kontrolü yapılacak
                        string resultMessage =
                            AvailabilityMessageForRomatologyAppointment(hcidData);

                        if (!string.IsNullOrEmpty(resultMessage))
                        {
                            return Json(new { isSuccess = false, message = resultMessage });
                        }
                    }

                    #endregion

                }

                Appointment patientAppointment = new Appointment()
                {
                    CreatedDate = DateTime.Now,
                    PatientId = HttpContext.User.Identity.Name,
                    HospitalClinicId = hcid,
                    AppointmentDate = appointmentDate,
                    AppointmentHour = hour,
                    AppointmentStatus = AppointmentStatus.Active

                };

                bool result = _unitOfWork.AppointmentRepository.Add(patientAppointment);
                message = result ? "Randevunuz başarıyla oluşturuldu" : "HATA: Beklenmedik bir hata oluştu!";

                if (result)
                {
                    //Randevu bilgilerini pdf olarak emaille gönderilsin
                    var data = _unitOfWork.AppointmentRepository.GetAppointmentByID(
                        HttpContext.User.Identity.Name,
                        patientAppointment.HospitalClinicId,
                        patientAppointment.AppointmentDate,
                        patientAppointment.AppointmentHour);

                    var user = _userManager.FindByNameAsync(HttpContext.User.Identity.Name).Result;

                    var emailMessage = new EmailMessage()
                    {
                        Contacts = new string[] { },
                        Subject = "MHRS(Merkezi Hekim Randevu Sistemi) - Randevu Bilgileri",
                        Body = $"Merhaba {user.Name}{user.Surname}, <br/> randevu bilgileriniz pdf olarak ektedir."
                    };

                    _emailSender.SendAppointmentPdf(emailMessage, data);
                }

                return result ? Json(new { isSuccess = true, message })
                              : Json(new { isSuccess = false, message });

            }
            catch (Exception ex)
            {
                message = "HATA: " + ex.Message;
                return Json(new { isSuccess = false, message });

            }
        }

        private string AvailabilityMessageForRomatologyAppointment(HospitalClinic hcidData)
        {
            try
            {
                string returnMessage = string.Empty;
                //usera ait aspnetuserclaims tablosunda kayıt varsa o kayıtlardan
                //Dahiliye-Romatoloji kaydının valuesu alınacak.
                var claimList = HttpContext.User.Claims.ToList();
                var claim = claimList.FirstOrDefault(x =>
                x.Type == "DahiliyeRomatoloji");
                if (claim != null)
                {
                    //2_dd.MM.yyyy
                    var claimValue = claim.Value;
                    //yöntem 1
                    int claimHCID = Convert.ToInt32(
                        claimValue.Substring(0, claimValue.IndexOf('_')));
                    DateTime claimDate = Convert.ToDateTime(
                        claimValue.Substring(claimValue.IndexOf('_') + 1).ToString());
                    //yöntem 2
                    //string[] array = claimValue.Split('_');
                    //int claimHCID = Convert.ToInt32(array[0]);
                    //DateTime claimDate = Convert.ToDateTime(array[1].ToString());

                    var claimHCIDdata = _unitOfWork.HospitalClinicRepository
                                       .GetFirstOrDefault(x =>
                                       x.Id == claimHCID,
                                       includeProperties: "Hospital");

                    //Claim bilgiler ayıklandı
                    //Acaba ayıklanan bilgilerdeki hastane ile randevu alınmak istenen hastane aynı mı değil mi?
                    if (hcidData.Hospital.Id != claimHCIDdata.Hospital.Id)
                    {
                        returnMessage = $"Romatoloji için dahilye muayenesi şarttır. Romatoloji randevusu alabileceğiniz uygun hastane: {claimHCIDdata.Hospital.HospitalName}";
                    }
                }
                else
                {
                    returnMessage = "DİKKAT! Romatolojiye randevu alabilmeniz için Dahiliyede son bir ay içinde muayene olmuş olmanız gereklidir!";
                }
                return returnMessage;
            }
            catch (Exception)
            {

                throw;
            }


        }

        [Authorize]
        public JsonResult CancelAppointment(int id)
        {
            var message = string.Empty;
            try
            {
                var appointment = _unitOfWork.AppointmentRepository.GetFirstOrDefault(x => x.Id == id);
                if (appointment != null)
                {
                    appointment.AppointmentStatus = AppointmentStatus.Cancelled;
                    var result = _unitOfWork.AppointmentRepository.Update(appointment);
                    message = result ? "Randevunuz iptal edildi." : "HATA: Beklenmedik bir sorun oluştu!";

                    return result ?
                        Json(new { isSuccess = true, message })
                        : Json(new { isSuccess = false, message });
                }
                else
                {
                    message = "HATA: Randevu bulunamadığı için iptal edilemedi. Tekrar deneyiniz..";
                    return Json(new { isSuccess = false, message });
                }
            }
            catch (Exception ex)
            {
                message = "HATA: " + ex.Message;
                return Json(new { isSuccess = false, message });
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpcomingAppointmentsExcelExport()
        {
            try
            {
                DataTable dt = new DataTable("Grid");
                var patientId = HttpContext.User.Identity.Name;
                var data = _unitOfWork.AppointmentRepository.GetUpComingAppointments(patientId);

                dt.Columns.Add("İL");
                dt.Columns.Add("İLÇE");
                dt.Columns.Add("HASTANE");
                dt.Columns.Add("KLİNİK");
                dt.Columns.Add("DOKTOR");
                dt.Columns.Add("RADENVU TARİHİ");
                dt.Columns.Add("RADENVU SATTİ");

                foreach (var item in data)
                {
                    var Doctor = item.HospitalClinic.Doctor.AppUser.Name + "" + item.HospitalClinic.Doctor.AppUser.Surname;
                    dt.Rows.Add(
                        item.HospitalClinic.Hospital.HospitalDistrict.City.CityName,
                        item.HospitalClinic.Hospital.HospitalDistrict.DistrictName,
                        item.HospitalClinic.Hospital.HospitalName,
                        item.HospitalClinic.Clinic.ClinicName,
                        Doctor,
                        item.AppointmentDate,
                        item.AppointmentHour);
                }
                //EXCEL OLUŞTUR
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        //return File ile dosya otomatik tarayıcı penceresine iner
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
