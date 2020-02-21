
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using TopProSystem.Models;
using System.Threading;
using System.Globalization;

namespace TopProSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult GetMainMenu()
        {
            if (Session[ConstantData.SessionUserID] == null) return View("NotFound404Page");
            return View();
        }

        [HttpPost]
        public ActionResult ChangeLanguage(string ddlCulture, string Url)
        {
            if (ddlCulture != null)
            {
                //   Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ddlCulture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ddlCulture);
            }
            var CurrentCookie = System.Web.HttpContext.Current.Request.Cookies.Get("Language");
            if (CurrentCookie == null)
            {
                CurrentCookie = new HttpCookie("Language");

            }

            CurrentCookie.Value = ddlCulture;
            Response.Cookies.Add(CurrentCookie);
            return Redirect(Url);
        }

        public JsonResult SessionTimeOut()
        {
            if (Session[Models.ConstantData.SessionUserID] == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}