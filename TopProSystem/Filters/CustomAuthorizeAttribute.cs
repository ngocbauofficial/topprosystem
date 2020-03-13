using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TopProSystem.Models;

namespace TopProSystem.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            var authCookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            bool sessionState = String.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session[ConstantData.SessionUserID]));
 
            if (sessionState == true || authCookie == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary() { { "controller", "Account" }, { "action", "GetLogin" }, { "area", "" }, { "returnUrl", filterContext.HttpContext.Request.RawUrl } });
            }

           

            base.OnAuthorization(filterContext);
        }
    }
}