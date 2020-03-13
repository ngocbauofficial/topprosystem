using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using TopProSystem.Models;

namespace TopProSystem.Models
{
    public class SessionContext
    {
        public void SetAuthenticationToken(string name, bool isPersistant, Areas.MasterSetting.Models.MA003 userData)
        {
            
            string data = null;
            if (userData != null)
                data = new JavaScriptSerializer().Serialize(userData);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddDays(0.5), isPersistant, userData.MCIDCD);
            string cookieData = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieData)
            {
                HttpOnly = true,
                Expires = ticket.Expiration
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public Areas.MasterSetting.Models.MA003 GetUserData()
        {
            Areas.MasterSetting.Models.MA003 userData = null;

            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                    // string userid = new JavaScriptSerializer().Deserialize(ticket.UserData, typeof(string)) as string;
                    userData = new Areas.MasterSetting.Models.MA003() { MCIDCD = ticket.UserData, MCIDNM = ticket.Name };
                }
            }
            catch (Exception ex)
            {
                return userData;
            }

            return userData;
        }
    }
}