using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.Models
{
    public partial class MA012
    {
        public virtual ICollection<MA001> MA001s { get; set; }
    }
}