using MHRSLite_BLL.Contracts;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLite_UI.QuartzWork
{
    public class RomatologyClaimJob
    {
        [DisallowConcurrentExecution]
        public class RomatologyClaimJob : IJob
        {
            private readonly ILogger<RomatologyClaimJob> _logger;
            private readonly IUnitOfWork _unitOfWork;
            public RomatologyClaimJob(
                ILogger<RomatologyClaimJob> logger,
                IUnitOfWork unitOfWork)
            {
                _logger = logger;
                _unitOfWork = unitOfWork;
            }



            public Task Execute(IJobExecutionContext context)
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
                        //Claim varsa tarihi aynı mı? tarihi aynı değilse sil ve yeniden ekle
                        //Claim yoksa yeni claim ekle
                    }
                }
                catch (Exception)
                {

                    //loglanacak
                }
            }
        }
    }
}
