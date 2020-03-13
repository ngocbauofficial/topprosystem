using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StaticResources.Controller;

namespace TopProSystem.Areas.MasterSetting
{
    public class MessageSendView
    {
        public static string MessageNotifi(string actionName)
        {
            var action = actionName.ToLower().Trim();
            string message = String.Empty;
            switch (action)
            {
                case "insert":
                    message = ErrorResource.Insert;
                    break;
                case "update":
                    message = ErrorResource.Change;
                    break;
                case "delete":
                    message = ErrorResource.Delete;
                    break;
                default:
                    message = "Unnown action";
                    break;
            }
            return "SuccessAlert('" + message + "')";
        }

        
    }
}