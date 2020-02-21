using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.Models
{
    public class DataInspecExcel
    {
        public string Status { get; set; }
        public string InspectionNo { get; set; }
        public string RawMaterialNo { get; set; }
        public int? Quantity { get; set; }
        public double? Weight { get; set; }
        public string StatusCode { get; set; }
        public double? PurchaseUP { get; set; }
        public double? PurchaseUPD { get; set; }
        public double? PurAmount { get; set; }
        public double? PurAmountDomestic { get; set; }
        public double? TaxAmount { get; set; }
        public double? TaxAmountDomestic { get; set; }
    }
}