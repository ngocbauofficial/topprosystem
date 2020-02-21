using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TopProSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Application["UserCode"] = new System.Collections.Generic.List<string>();

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
                //   System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {

                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //string[] d = Application["UserCode"] as string[];
            //string UserLoggedIn = Session["UserCode"] == null ? string.Empty : (string)Session["UserCode"];
            //if (UserLoggedIn.Length > 0)
            //{
            //    if (!Array.Exists(d,x=>x.ToUpper() == UserLoggedIn.ToUpper()))
            //    {
            //        HttpContext.Current.Session.Remove("UserCode");
            //    }
            //}
        }

        protected void Session_End(object sender, EventArgs e)
        {
            string UserLoggedIn = Session["UserCode"] == null ? string.Empty : (string)Session["UserCode"];
            if (UserLoggedIn.Length > 0)
            {
                System.Collections.Generic.List<string> d = Application["UserCode"] as System.Collections.Generic.List<string>;
                if (d != null)
                {
                    lock (d)
                    {
                        d.Remove(UserLoggedIn);
                    }
                }
            }
        }
    }
}
