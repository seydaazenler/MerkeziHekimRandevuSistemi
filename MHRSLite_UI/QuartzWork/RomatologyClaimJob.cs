﻿using MHRSLite_BLL.Contracts;
using MHRSLite_EL.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MHRSLite_UI.QuartzWork
{
    [DisallowConcurrentExecution]
    public class RomatologyClaimJob : IJob
    {
        private readonly ILogger<RomatologyClaimJob> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public RomatologyClaimJob(ILogger<RomatologyClaimJob> logger,
            IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var date = DateTime.Now.AddMonths(-1);
                //son bir aydaki dahilyedeki iptal olan hariç tüm randevuları getir
                var appointment = _unitOfWork.AppointmentRepository
                    .GetAppointmentsIM(date).OrderByDescending(x => x.AppointmentDate).ToList();

                foreach (var item in appointment)
                {
                    //usera ait dahiliyeRomatoloji claimi yoksa eklenmeli
                    // yarın devam....
                    //Claim varsa tarihi aynı mı? tarihi aynı değilse tarihi replace et
                    //Claim yoksa yeni claim ekle

                    //Romatoloji Claim
                    //hcid_04.03.2022
                    var claimValue = $"{item.HospitalClinicId}_{item.AppointmentDate.ToString("dd/MM/yyyy")}";
                    Claim romatologyClaim = new Claim("DahiliyeRomatoloji", claimValue, ClaimValueTypes.String, "Internal");

                    //userın claim listesini alalım ve control edelim
                    var claimList = await _userManager.GetClaimsAsync(item.Patient.AppUser);
                    var claim = claimList.FirstOrDefault(x => x.Type == "DahiliyeRomatoloji");

                    if (claim == null)
                    {
                        //Claim yoksa claim ekleyelim
                        await _userManager.AddClaimAsync(item.Patient.AppUser, romatologyClaim);
                    }
                    else
                    {
                        //eğer Claim varsa içerisindeki değerler ayıklanmalıdır

                        //YÖNTEM 1
                        //int claimHCID = Convert.ToInt32(
                        //claim.Value.Substring(0, claim.Value.IndexOf('_')));
                        //DateTime claimDate = Convert.ToDateTime(
                        //claim.Value.Substring(claim.Value.IndexOf('_') + 1).ToString());

                        //YÖNTEM 2
                        string[] array = claim.Value.Split('_');
                        int claimHCID = Convert.ToInt32(array[0]);
                        DateTime claimDate = Convert.ToDateTime(array[1].ToString());

                        if (claimDate < item.AppointmentDate)
                        {
                            await _userManager.ReplaceClaimAsync(item.Patient.AppUser, claim, romatologyClaim);
                        }
                    }
                }
                _logger.LogInformation("RomatologyClaims updated");
            }
            catch (Exception ex)
            {

                //loglanacak
            }
        }
    }
}

