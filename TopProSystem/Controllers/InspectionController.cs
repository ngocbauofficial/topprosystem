using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Controllers
{
    public class InspectionController : Controller
    {
        public ActionResult GetInspectionMenu()
        {
            TempData["active"] = "active-menu";
            return View();
        }
	}
}