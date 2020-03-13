using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopProSystem.Filters;

namespace TopProSystem.Extension.AccountRole
{
    [CustomAuthorize]
    [MasterAuthorize(Roles = "Account")]
    public abstract partial class BaseAccountController : Controller
    {
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("Home", "NotFound404Page", new { pageUrl = this.Request.RawUrl, area = "" });
        }
    }
}