using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.Models
{

    public partial class RawMaterial
    {
        [NotMapped]
        public PUR001 PurchaseContract { get; set; }
         [NotMapped]
        public string AASPLCD_dl { get; set; }
         [NotMapped]
        public string AACMDCD_dl { get; set; }
         [NotMapped]
        public string AAUSRCD_dl { get; set; }
         [NotMapped]
        public string AAMKCD_dl { get; set; }
         [NotMapped]
        public string AAIDCD_dl { get; set; }
         [NotMapped]
        public string AACTRTP_dl { get; set; }
    }

   
}