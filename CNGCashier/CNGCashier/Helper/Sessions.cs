using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CNGCashier.Models
{
    public static class Sessions
    {
        public static int? CompanyId
        {
            get
            {
                if (HttpContext.Current.Session["companyId"] == null)
                {
                    return null;
                }
                return Convert.ToInt32(HttpContext.Current.Session["companyId"]);
            }
            set {
                HttpContext.Current.Session["companyId"] = value;
            }
        }

        public static string CompanyName
        {
            get
            {
                CompanyRepo companyRepo = new CompanyRepo();

                Company company = companyRepo.List().FirstOrDefault(p => p.Id == CompanyId.Value);

                return company.Name;
            }
        }
    }

    public class AuthorizationFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext != null)
            {
                HttpSessionStateBase Session = filterContext.HttpContext.Session;
                if (Session["uid"] == null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "controller", "Home" },
                            { "action", "Login" }
                        });
                }
            }
        }
    }
}