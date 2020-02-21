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
    [MasterAuthorize(Roles = "Purchase")]
    public abstract partial class BasePurchaseController : Controller
    {
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Account", new { pageUrl = this.Request.RawUrl, area = "" });
        }
    }
}