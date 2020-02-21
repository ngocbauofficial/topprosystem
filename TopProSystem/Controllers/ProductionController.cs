using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Controllers
{
    public class ProductionController : Controller
    {
        public ActionResult GetProductionMenu()
        {
            TempData["active"] = "active-menu";
            return View();
        }
	}
}