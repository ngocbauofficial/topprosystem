using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.Models
{
    public partial class MA006
    {
        [NotMapped]
        public IEnumerable<SelectListItem> CommodityCode { get; set; }
    }
}