//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TopProSystem.Areas.MasterSetting.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LogUserAction
    {
        public long ID { get; set; }
        public string TableName { get; set; }
        public string UserCode { get; set; }
        public string Remark { get; set; }
        public string ActionType { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string RecordPrivateKey { get; set; }
    }
}
