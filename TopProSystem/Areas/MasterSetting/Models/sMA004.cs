﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.Models
{
    public partial class MA004
    {
        [NotMapped]
        public IEnumerable<SelectListItem> WarehouseCategory { get; set; }
    }
}