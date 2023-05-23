using System.Web;
using System.Web.Mvc;

namespace LTW_NTTinh_2024802010018
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
