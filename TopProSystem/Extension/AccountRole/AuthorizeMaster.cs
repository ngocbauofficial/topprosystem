using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopProSystem.Extension.AccountRole;

namespace TopProSystem.Extension
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class MasterAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly bool _dontValidate;


        public MasterAuthorizeAttribute()
            : this(false)
        {
        }

        public MasterAuthorizeAttribute(bool dontValidate)
        {
            this._dontValidate = dontValidate;
        }

        private void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        private IEnumerable<MasterAuthorizeAttribute> GetAdminAuthorizeAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(MasterAuthorizeAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(MasterAuthorizeAttribute), true))
                .OfType<MasterAuthorizeAttribute>();
        }

        private bool IsAdminPageRequested(AuthorizationContext filterContext)
        {
            var adminAttributes = GetAdminAuthorizeAttributes(filterContext.ActionDescriptor);
            if (adminAttributes != null && adminAttributes.Any())
                return true;
            return false;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (_dontValidate)
                return;

            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [AdminAuthorize] attribute when a child action cache is active");

            if (IsAdminPageRequested(filterContext))
            {
                if (!this.HasAdminAccess(filterContext))
                    this.HandleUnauthorizedRequest(filterContext);
            }
        }

        public virtual bool HasAdminAccess(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID] == null)
            {
                return false;
            }
            var service = new PermissionService();
            bool result;
            switch (Roles)
            {
                case "Master":
                    result = service.Authorize(StandardPermissionProvider.Master);
                    return result;
                case "Purchase":
                    result = service.Authorize(StandardPermissionProvider.Purchase);
                    return result;

                case "Account":
                    result = service.Authorize(StandardPermissionProvider.Account);
                    return result;

                case "RawMaterial":
                    result = service.Authorize(StandardPermissionProvider.Purchase);
                    return result;
                case "Sales":
                    result = service.Authorize(StandardPermissionProvider.Sale);
                    return result;

                case "Production":
                    result = service.Authorize(StandardPermissionProvider.Production);
                    return result;
                case "Delivery":
                    result = service.Authorize(StandardPermissionProvider.Delivery);
                    return result;
                case "Inventory":
                    result = service.Authorize(StandardPermissionProvider.Inventory);
                    return result;
                case "Inspection":
                    result = service.Authorize(StandardPermissionProvider.Inspection);
                    return result;
                default:
                    return false;
            }
        }
        public string Roles { get; set; }
    }
}