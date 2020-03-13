using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopProSystem.Filters;

namespace TopProSystem.Extension.AccountRole
{
    [Authorize]
    [CustomAuthorize]
    [MasterAuthorize(Roles = "Master")]
    public abstract partial class BaseMasterController : Controller
    {
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("Home", "NotFound404Page", new { pageUrl = this.Request.RawUrl, area = "" });
        }
    }
}