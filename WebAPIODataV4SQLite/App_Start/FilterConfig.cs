using System.Web;
using System.Web.Mvc;

namespace WebAPIODataV4SQLite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
