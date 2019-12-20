using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Extension.AccountRole
{
    public partial class PermissionMappingModel
    {
        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecord>();
            AvailableCustomerRoles = new List<SecurityLevel>();
            AvailableActionRoles = new List<PermissionAction>();
            Allowed = new Dictionary<string, IDictionary<string, bool>>();
            AllowedAction = new Dictionary<string, IDictionary<string, bool>>();

        }
        public IList<PermissionRecord> AvailablePermissions { get; set; }
        public IList<SecurityLevel> AvailableCustomerRoles { get; set; }

        public IList<PermissionAction> AvailableActionRoles { get; set; }

        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<string, bool>> Allowed { get; set; }

        public IDictionary<string, IDictionary<string, bool>> AllowedAction { get; set; }
    }

}