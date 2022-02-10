using MHRSLite_BLL;
using MHRSLite_EL;
using MHRSLite_EL.Enums;
using MHRSLite_EL.IdentityModels;
using MHRSLite_UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IEmailSender emailSender)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
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
                var checkUserForUserName = await _userManager.FindByNameAsync(model.UserName);

                if (checkUserForUserName != null)
                {
                    ModelState.AddModelError(nameof(model.UserName), "Bu kullanıcı sistemde zaten mevcut!");
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
                    UserName = model.UserName,
                    Gender = model.Gender
                    //TODO: Birth Date? / Phone Number?
                };
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded) //sonuç başarılı ise
                {
                    //eğer giriş işlemleri doğru ise kullanıcı = hasta ataması yapılır
                    var roleResult = await _userManager.AddToRoleAsync(newUser, RoleNames.Patient.ToString());
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
        public async Task<IActionResult> ConfirmEmail(string userId,string code)
        {
            try
            {
                if (userId==null || code==null)
                {
                    return NotFound("Sayfa bulunamadı!");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user==null)
                {
                    return NotFound("Kullanıcı bulunamadı!");
                }
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await  _userManager.ConfirmEmailAsync(user, code);

                if (result.Succeeded)
                {
                    //user pasif rolde mi ?
                    if (_userManager.IsInRoleAsync(user,RoleNames.Passive.ToString()).Result)
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
    }
}
