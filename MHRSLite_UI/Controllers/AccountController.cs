﻿using MHRSLite_BLL;
using MHRSLite_BLL.Contracts;
using MHRSLite_EL;
using MHRSLite_EL.Enums;
using MHRSLite_EL.IdentityModels;
using MHRSLite_EL.Models;
using MHRSLite_UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MHRSLite_UI.Controllers
{
    public class AccountController : Controller
    {
        //Solid prensiplerine uygun bağımlıklıklardan kurtulmaya yönelik yapı oluşturuyoruz.
        //Dependecy Injection

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly RoleManager<AppRole> _roleManager;

        private readonly IEmailSender _emailSender;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfiguration _configuration;

        public AccountController(
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
            CheckRoles();
        }

        //Global.asax dosyası olmadığı için rolleri burada oluşturururz
        private void CheckRoles()
        {
            var allRoles = Enum.GetNames(typeof(RoleNames));
            foreach (var item in allRoles)
            {
                if (!_roleManager.RoleExistsAsync(item).Result)
                {
                    var result = _roleManager.CreateAsync(new AppRole()
                    {
                        Name = item,
                        Description = item
                    }).Result;
                }
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                //ModelState is valid değilse validasyon hatası vardır!
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                //sepeti onayla, gibi aynı işleri aynı anda yapmak gerektiğinde async kullanırız yavaşlama olmasın diye
                //aynı userName ve Maile sahip kişi tekrar eklenmemeli
                var checkUserForUserName = await _userManager.FindByNameAsync(model.TCNumber);
                //USERNAME = TCNUMBER
                if (checkUserForUserName != null)
                {
                    ModelState.AddModelError(nameof(model.TCNumber), "Bu kullanıcı sistemde zaten mevcut!");
                    return View(model);
                }
                var checkUserForEMail = await _userManager.FindByEmailAsync(model.Email);
                if (checkUserForEMail != null)
                {
                    ModelState.AddModelError(nameof(model.Email), "Bu Email sistemde mevcut! Farklı bir EMail ile kayıt olunuz");
                    return View(model);
                }
                //yeni kullanıcı oluşturulur
                AppUser newUser = new AppUser()
                {
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.TCNumber,
                    Gender = model.Gender
                    //TODO: Birth Date? / Phone Number?
                };
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded) //sonuç başarılı ise
                {
                    //eğer giriş işlemleri doğru ise kullanıcı = hasta ataması yapılır
                    var roleResult = await _userManager.AddToRoleAsync(newUser, RoleNames.Passive.ToString());
                    
                    //email gönderilmelidir
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callBackUrl = Url.Action("ConfirmEmail", "Account",
                        new { userId = newUser.Id, code = code }, protocol: Request.Scheme);

                    var emailMessage = new EmailMessage()
                    {
                        Contacts = new String[] { newUser.Email },
                        Subject = "Merkezi Hekim Randevu Sistemi (MHRS) - EMail Aktivasyon",
                        Body = $"Merhaba {newUser.Name} {newUser.Surname}, <br/> Hesabınızı aktifleştirmek için " +
                        $"<a href='{HtmlEncoder.Default.Encode(callBackUrl)}'> buraya </a> tıklayınız"
                    };
                    await _emailSender.SendAsync(emailMessage);
                    //patient tablosuna ekleme yapılmalıdır
                    Patient newPatient = new Patient()
                    {
                        TCNumber = model.TCNumber,
                        UserId = newUser.Id

                    };
                    if (_unitOfWork.PatientRepository.Add(newPatient) == false)
                    {
                        //sistem yöneticisine email gitsin
                        var emailMessageToAdmin = new EmailMessage()
                        {
                            Contacts = new string[] { _configuration.GetSection("ManagerEmails:Email").Value },
                            CC = new string[] {_configuration.GetSection("ManagerEmails:EmailToCC").Value },
                            Subject="MHRSLite - HATA! Patient Tablosu",
                            Body= $"Aşağıdaki bilgilere sahip kişi eklenşrken hata oluştu. Patient tablosuna bilgileri ekleyiniz." +
                            $"<br/> Bilgiler: TcNumber {model.TCNumber} <br/> UserId {newUser.Id}"
                        };
                        await _emailSender.SendAsync(emailMessageToAdmin);
                    }

                    return RedirectToAction("Login", "Account");

                }
                else
                {
                    ModelState.AddModelError("", "Beklenmedik bir hata oluştu!");
                    return View(model);
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    return NotFound("Sayfa bulunamadı!");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı!");
                }
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user, code);

                if (result.Succeeded)
                {
                    //user pasif rolde mi ?
                    if (_userManager.IsInRoleAsync(user, RoleNames.Passive.ToString()).Result)
                    {
                        await _userManager.RemoveFromRoleAsync(user, RoleNames.Passive.ToString());
                        await _userManager.RemoveFromRoleAsync(user, RoleNames.Patient.ToString());
                    }
                    TempData["EmailConfirmedMessage"] = "Hesabınız aktifleşmiştir..";
                    return RedirectToAction("Login", "Account");
                }

                //TODO: Login sayfasında bu tempdata view ekranında kontrol edilecek
                //aynı sayfada kalmasını istersek viewbag ile göndeririz
                ViewBag.EmailConfirmedMessage = "Hesap aktifleştirme başarısızdır!";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.EmailConfirmedMessage = "Beklenmedik bir hata oldu! Tekrar deneyiniz";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Veri girişleri düzgün olmaldır!");
                    return View(model);
                }
                //buradaki true yanlış girdiğinde hesabı kilitleyeyim mi? bunun ayarı böyle yapılır
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalıdır!");
                    return View(model);
                }
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu!");
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            try
            {
                var user =await _userManager.FindByEmailAsync(email);
                if (user==null)
                {
                    ViewBag.ResetPasswordMessage = "Email adresine ait sistemde kayıtlı kullanıcı yok!";
                }
                else
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callBackUrl = Url.Action("ConfirmResetPassword", "Account", 
                        new {userId =user.Id,code=code },protocol:Request.Scheme);

                    var emailMessage = new EmailMessage()
                    {
                        Subject = "Merkezi Hekim Randevu Sistemi(MHRS) - Şifremi Unuttum",
                        Body = $"Merhaba {user.Name} {user.Surname} Yeni parola belirlemek için " +
                        $"<a href='{HtmlEncoder.Default.Encode(callBackUrl)}'>buraya</a> tıklayınız."
                    };
                    await _emailSender.SendAsync(emailMessage);
                    ViewBag.ResetPasswordMessage = "Posta kutunuza şifre güncelleme maili gönderilmiştir.";
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ResetPasswordMessage = "Beklenmedik bir hata oluştu!";
                return View();
            }
        }

        [HttpGet]
        public IActionResult ConfirmResetPassword(string userId,string code)
        {
            if (string.IsNullOrEmpty(userId)||string.IsNullOrEmpty(code))
            {
                return BadRequest("deneme");
            }
            ViewBag.UserId = userId;
            ViewBag.Code = code;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı!");
                    return View(model);
                }

                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
                var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["ConfirmResetPasswordMessage"] = "Şifreniz yenilendi";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "HATA: Şifre yenileme işlemi başarısız!");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu!");
                return View(model);
            }
        }
    }
}
