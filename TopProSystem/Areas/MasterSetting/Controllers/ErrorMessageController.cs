using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.Controllers
{
    public class ErrorMessageController : Controller
    {
        // GET: MasterSetting/ErrorMessage
        public ActionResult Notification(string actionName)
        {
            ViewBag.actionname = actionName;
            return View();
        }
        public ViewResult NotFound()
        {
            return View();
        }
    }
}