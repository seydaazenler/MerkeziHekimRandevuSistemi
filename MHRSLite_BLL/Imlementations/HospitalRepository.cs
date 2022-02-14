using MHRSLite_BLL.Contracts;
using MHRSLite_DAL;
using MHRSLite_EL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MHRSLite_BLL.Imlementations
{
    public class HospitalRepository:Repository<Hospital>,IHospitalRepository
    {
        private readonly MyContext _myContext;

        public HospitalRepository(MyContext myContext)
            : base(myContext)
        {
            //Repositorylere kalıtım aldıkları yerdeki metotlar yeterli görünüyor.
            //Ancak ilerleyen zamanlarda Generic yapının karşılamadığı bir ihtiyaç olursa
            //bir metot eklenebilir. O metot _myContext'i kullanarak işlemi yapsın diye 
            //burada _myContext = myContext yapıldı.
            //Örn: Bir önceki projede CategoryRepository'de dashboard için ihtiyaç duyuldu
            //  Örn: Sistem yöneticilerinin yada müdürlerinin istediği raporlar
            // Örn : İstanbuldaki toplam Dahiliye Klinik sayısı
            _myContext = myContext;
        }
    }
}
