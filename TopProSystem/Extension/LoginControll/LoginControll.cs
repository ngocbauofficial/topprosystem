using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Extension.LoginControll
{
    public partial class LoginControll
    {
        public bool _loginControll(string name)
        {
            List<string> d = HttpContext.Current.Application["UserCode"] as List<string>;
            if (d != null)
            {
                lock (d)
                {
                    if (d.Contains(name))
                    {
                        return false;
                    }
                    d.Add(name);
                }
            }
            return true;
        }

        public List<string> _GetUserLoggedIn()
        {
            List<string> d = HttpContext.Current.Application["UserCode"] as List<string>;
            return d;
        }

        public void _RemoveUserSession(string userid)
        {
            List<string> d = HttpContext.Current.Application["UserCode"] as List<string>;
            d.Remove(userid);
          //  HttpContext.Current.Session.Remove("UserCode");
        }
    }
}