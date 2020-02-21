using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Models
{
    public class ConstantData
    {
        public const string SessionUserID = "UserCode";
        public const string SessionUserName = "Name";
        public const string FailMessage = "Fail";
        public const string SuccessMessage = "Success";
        public const string ActionInsert = "Insert";
        public const string ActionChange = "Change";
        public const string ActionDelete = "Delete";
        public const string Notification_key = "Notification";
        public const string Inused = "used";

        public enum ReportFileName
        {
            Purchase_History_By_Maker = 0,
            Purchase_History_By_Maker_By_Commodity = 1,
            Purchase_History_By_Maker_By_Commodity_By_Spec_By_Size = 2,
            Purchase_History_By_Supplier = 3,
            Purchase_History_By_Supplier_By_Commodity = 4,
            Purchase_History_By_Supplier_By_Commodity_By_Spec_By_Size = 5,
        }

    }
}