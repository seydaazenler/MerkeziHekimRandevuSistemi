using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHRSLite_EL.PagingListModels
{
    //Herhangi bir şey içerisine gelebilir o yüzden Generic<T> yapıdadır
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public List<T> ItemList { get; set; }

        public PaginatedList(List<T> items, int count, int pageindex, int pageSize)
        {
            PageIndex = pageindex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            ItemList = items;
        }

        public bool PreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool NextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static PaginatedList<T> CreateAsync(List<T> sourcelist, int pageindex, int pageSize)
        {
            var count = sourcelist.Count;
            //bulunduğum sayfadan bir eksiltip sayfada kaç eleman olacaksa o kadar veriyi atla
            var items = sourcelist.Skip((pageindex - 1) * pageSize)
            //sayfada kaç eleman olmasını istiyorsan o kadarını al
            .Take(pageSize)
            //listele
            .ToList();
            //oluşan veriyi yeni nesne yaratarak gönder
            return new PaginatedList<T>(items, count, pageindex, pageSize);

        }
    }
}
