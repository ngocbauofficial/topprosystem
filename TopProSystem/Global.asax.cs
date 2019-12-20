using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TopProSystem.Models;

namespace TopProSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            if (!Directory.Exists(Server.MapPath("~/FileCreated/log/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/FileCreated/log"));
            }

            string url = "~/FileCreated/log/Readme.log.txt";
            if (!File.Exists(Server.MapPath(url)))
            {
                File.WriteAllText(Server.MapPath(url), "");
            }
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
      {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Language"];
            if (cookie != null && cookie.Value != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }

        }

        protected void Session_Start(object sender, EventArgs e)
        {
           
        }

        protected void Session_End(object sender, EventArgs e)
        {
         
        }
    }
}
