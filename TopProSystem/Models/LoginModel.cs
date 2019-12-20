using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TopProSystem.Models
{
    public class LoginModel
    {
     
            [Display(Name = "User")]
            [Required(ErrorMessage = "UserName is Required")]
            public string UserName { set; get; }

            [Required(ErrorMessage = "Password is Required")]
            [Display(Name = "Password")]
            public string Password { set; get; }

        
    }
}