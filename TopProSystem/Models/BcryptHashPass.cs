using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCrypt.Net;

namespace TopProSystem.Models
{
    public class BcryptHashPass
    {
        protected static string salt = "AABBZZ";
        public static string HashPassword(string pass)
        {
            pass += salt;
            string _salt = BCrypt.Net.BCrypt.GenerateSalt(8);
            return BCrypt.Net.BCrypt.HashPassword(pass,_salt);
        }

        public static Boolean VerifyHashPass(string text, string pass)
        {
            text += salt;
            return BCrypt.Net.BCrypt.Verify(text, pass);
        }
    }
}