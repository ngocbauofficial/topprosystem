using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.Models
{
    public partial class MA003
    {
        [NotMapped]
        public  IEnumerable<SelectListItem> UserCodes { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> SectionCode { get; set; }

    }
}