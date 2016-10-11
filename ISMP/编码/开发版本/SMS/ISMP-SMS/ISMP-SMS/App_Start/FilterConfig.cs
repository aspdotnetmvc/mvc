using System.Web;
using System.Web.Mvc;

namespace ISMP_SMS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new JsonAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}