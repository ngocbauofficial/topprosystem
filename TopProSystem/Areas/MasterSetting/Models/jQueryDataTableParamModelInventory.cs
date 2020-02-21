using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.Models
{
    public partial class InventorySearchModel
    {    /// </summary>       
        public string PurchaseContractNo { get; set; }
        public string ShippingMonth { get; set; }
        public string SupplierContractNo { get; set; }
        public string MakerCode { get; set; }
        public string SupplierCode { get; set; }
        public string CommondityCode { get; set; }
        public string UserCode { get; set; }
        public string RawMaterialType { get; set; }
    }
    public class jQueryDataTableInventory
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public int draw { get; set; }


        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public int length { get; set; }
    }

}