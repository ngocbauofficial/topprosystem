using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Controllers
{
    public class PakingController : Controller
    {
        public ActionResult GetPakingMenu()
        {
            TempData["active"] = "active-menu";
            return View();
        }
	}
}