using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Controllers
{
    public class DeliveryController : Controller
    {
        public ActionResult GetDeliveryMenu()
        {
            TempData["active"] = "active-menu";
            return View();
        }
	}
}