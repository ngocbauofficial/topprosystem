using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StaticResources.View.PUR001;
using StaticResources;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.Models
{
    [MetadataType(typeof(MetaData_PUR001))]
    public partial class PUR001
    {
        [NotMapped]
        public bool _completed  { get; set; } // bien tam dung cho viec disable update
       // [NotMapped]
       // public IEnumerable<SelectListItem> Grades { get; set; }
        //[NotMapped]
        //public IEnumerable<SelectListItem> MakerCodes { get; set; }
        // [NotMapped]
        //public IEnumerable<SelectListItem> CurrencyCodes { get; set; }
        // [NotMapped]
        // public IEnumerable<SelectListItem> Priceterms { get; set; }
        //[NotMapped]
        // public IEnumerable<SelectListItem> TypeOfTerms { get; set; }
        //  [NotMapped]
        // public IEnumerable<SelectListItem> CommodityCodes { get; set; }
        //  [NotMapped]
        //  public IEnumerable<SelectListItem> ContractTypes { get; set; }
      //  [NotMapped]
        //public IEnumerable<SelectListItem> SettelementTerms { get; set; }
        // [NotMapped]
        // public IEnumerable<SelectListItem> TaxCodes { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> ExchangeRates { get; set; }
       // [NotMapped]
      //  public IEnumerable<SelectListItem> Specs { get; set; }
      //  [NotMapped]
      //  public IEnumerable<SelectListItem> Coatings { get; set; }
        // [NotMapped]
        //public IEnumerable<SelectListItem> UserCodes { get; set; }
        // [NotMapped]
        // public IEnumerable<SelectListItem> RawMaterialTypes { get; set; }
      //  [NotMapped]
      //  public IEnumerable<SelectListItem> SteelGrades { get; set; }
        //  [NotMapped]
        //  public IEnumerable<SelectListItem> PersonIncharges { get; set; }
        //  [NotMapped]
        // public IEnumerable<SelectListItem> SupplierCodes { get; set; }
        // [NotMapped]
        // public IEnumerable<SelectListItem> DeliveryConditionCodes { get; set; }
        //  [NotMapped]
        //  public IEnumerable<SelectListItem> ExchangeRateType { get; set; }
        [NotMapped]
        public Dictionary<string, string> Grades { get; set; }
        [NotMapped]
        public Dictionary<string, string> MakerCodes { get; set; }
        [NotMapped]
        public Dictionary<string, string> CurrencyCodes { get; set; }
        [NotMapped]
        public Dictionary<string, string> Priceterms { get; set; }
        [NotMapped]
        public Dictionary<string, string> TypeOfTerms { get; set; }
        [NotMapped]
        public Dictionary<string, string> CommodityCodes { get; set; }
        [NotMapped]
        public Dictionary<string, string> ContractTypes { get; set; }
        [NotMapped]
        public Dictionary<string, string> SettelementTerms { get; set; }
        [NotMapped]
        public Dictionary<string, string> TaxCodes { get; set; }
        //[NotMapped]
        //public Dictionary<string, string> ExchangeRates { get; set; }
        [NotMapped]
        public Dictionary<string, string> Specs { get; set; }
       [NotMapped]
        public Dictionary<string, string> Coatings { get; set; }
        [NotMapped]
        public Dictionary<string, string> UserCodes { get; set; }
        [NotMapped]
        public Dictionary<string, string> RawMaterialTypes { get; set; }
        [NotMapped]
        public Dictionary<string, string> SteelGrades { get; set; }
        [NotMapped]
        public Dictionary<string, string> PersonIncharges { get; set; }
        [NotMapped]
        public Dictionary<string, string> SupplierCodes { get; set; }
        [NotMapped]
        public Dictionary<string, string> DeliveryConditionCodes { get; set; }
        [NotMapped]
        public Dictionary<string, string> ExchangeRateType { get; set; }
    }

    public class MetaData_PUR001
    {
        [Display(Name = "AADSTNC", ResourceType = typeof(Resource))]
        public string AADSTNC { get; set; }
        [Display(Name = "AARGSDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> AARGSDT { get; set; }
        [Display(Name = "AAUPDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> AAUPDT { get; set; }
        [Display(Name = "AADTCD", ResourceType = typeof(Resource))]
        public string AADTCD { get; set; }
        [Display(Name = "AAMKCD", ResourceType = typeof(Resource))]
        public string AAMKCD { get; set; }
        [Display(Name = "AAPURNO", ResourceType = typeof(Resource))]
        public string AAPURNO { get; set; }
        [Display(Name = "AACTITM", ResourceType = typeof(Resource))]
        public Nullable<double> AACTITM { get; set; }
        [Display(Name = "AASPLCD", ResourceType = typeof(Resource))]
        public string AASPLCD { get; set; }
        [Display(Name = "AAUSRCD", ResourceType = typeof(Resource))]
        public string AAUSRCD { get; set; }
        [Display(Name = "AASCTNC", ResourceType = typeof(Resource))]
        public string AASCTNC { get; set; }
        [Display(Name = "AAIDCD", ResourceType = typeof(Resource))]
        public string AAIDCD { get; set; }
        [Display(Name = "AACMDCD", ResourceType = typeof(Resource))]
        public string AACMDCD { get; set; }
        [Display(Name = "AATRDCD", ResourceType = typeof(Resource))]
        public string AATRDCD { get; set; }
        [Display(Name = "AACTRTP", ResourceType = typeof(Resource))]
        public string AACTRTP { get; set; }
        [Display(Name = "AADPSCD", ResourceType = typeof(Resource))]
        public string AADPSCD { get; set; }
        [Display(Name = "AASPTCD", ResourceType = typeof(Resource))]
        public string AASPTCD { get; set; }
        [Display(Name = "AASHPDT", ResourceType = typeof(Resource))]
        public Nullable<double> AASHPDT { get; set; }
        [Display(Name = "AASPPNO", ResourceType = typeof(Resource))]
        public string AASPPNO { get; set; }
        [Display(Name = "AATCNO", ResourceType = typeof(Resource))]
        public string AATCNO { get; set; }
        [Display(Name = "AACRRCD", ResourceType = typeof(Resource))]
        public string AACRRCD { get; set; }
        [Display(Name = "AAEXRTT", ResourceType = typeof(Resource))]
        public string AAEXRTT { get; set; }
        [Display(Name = "AAEXRT", ResourceType = typeof(Resource))]
        public Nullable<double> AAEXRT { get; set; }
        [Display(Name = "AATXCD", ResourceType = typeof(Resource))]
        public string AATXCD { get; set; }
        [Display(Name = "AATXRT", ResourceType = typeof(Resource))]
        public Nullable<double> AATXRT { get; set; }
        [Display(Name = "AATEXRT", ResourceType = typeof(Resource))]
        public string AATEXRT { get; set; }
        [Display(Name = "AATXEXR", ResourceType = typeof(Resource))]
        public Nullable<double> AATXEXR { get; set; }
        [Display(Name = "AACMPCD", ResourceType = typeof(Resource))]
        public string AACMPCD { get; set; }
        [Display(Name = "AACMPDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> AACMPDT { get; set; }
        [Display(Name = "AADLVCD", ResourceType = typeof(Resource))]
        public string AADLVCD { get; set; }
        [Display(Name = "AAUSMSG", ResourceType = typeof(Resource))]
        public string AAUSMSG { get; set; }
        [Display(Name = "AACLLNO", ResourceType = typeof(Resource))]
        public string AACLLNO { get; set; }
        [Display(Name = "AASETRM", ResourceType = typeof(Resource))]
        public string AASETRM { get; set; }
        [Display(Name = "AAPRICE", ResourceType = typeof(Resource))]
        public string AAPRICE { get; set; }
        [Display(Name = "AAPTTRM", ResourceType = typeof(Resource))]
        public string AAPTTRM { get; set; }
        [Display(Name = "AADLPOT", ResourceType = typeof(Resource))]
        public string AADLPOT { get; set; }
        [Display(Name = "AARMK1", ResourceType = typeof(Resource))]
        public string AARMK1 { get; set; }
        [Display(Name = "AARMK2", ResourceType = typeof(Resource))]
        public string AARMK2 { get; set; }
        [Display(Name = "AARMK3", ResourceType = typeof(Resource))]
        public string AARMK3 { get; set; }
        [Display(Name = "AARMK4", ResourceType = typeof(Resource))]
        public string AARMK4 { get; set; }
        [Display(Name = "AARMK5", ResourceType = typeof(Resource))]
        public string AARMK5 { get; set; }
        [Display(Name = "ABDSTNC", ResourceType = typeof(Resource))]
        public string ABDSTNC { get; set; }
        [Display(Name = "ABRGSDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> ABRGSDT { get; set; }
        [Display(Name = "ABUPDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> ABUPDT { get; set; }
        [Display(Name = "ABDTCD", ResourceType = typeof(Resource))]
        public string ABDTCD { get; set; }
        [Display(Name = "ABPURNO", ResourceType = typeof(Resource))]
        public string ABPURNO { get; set; }
        [Display(Name = "ABCTITM", ResourceType = typeof(Resource))]
        public Nullable<int> ABCTITM { get; set; }
        [Display(Name = "ABMCSPC", ResourceType = typeof(Resource))]
        public string ABMCSPC { get; set; }
        [Display(Name = "ABCOAT", ResourceType = typeof(Resource))]
        public string ABCOAT { get; set; }
        [Display(Name = "ABGRADE", ResourceType = typeof(Resource))]
        public string ABGRADE { get; set; }
        [Display(Name = "ABCMBT", ResourceType = typeof(Resource))]
        public string ABCMBT { get; set; }
        [Display(Name = "ABCMBW", ResourceType = typeof(Resource))]
        public string ABCMBW { get; set; }
        [Display(Name = "ABCMBL", ResourceType = typeof(Resource))]
        public string ABCMBL { get; set; }
        [Display(Name = "ABSIZET", ResourceType = typeof(Resource))]
        public Nullable<double> ABSIZET { get; set; }
        [Display(Name = "ABSIZEW", ResourceType = typeof(Resource))]
        public Nullable<double> ABSIZEW { get; set; }
        [Display(Name = "ABSIZEL", ResourceType = typeof(Resource))]
        public Nullable<double> ABSIZEL { get; set; }
        [Display(Name = "ABBSZT", ResourceType = typeof(Resource))]
        public Nullable<double> ABBSZT { get; set; }
        [Display(Name = "ABBSZW", ResourceType = typeof(Resource))]
        public Nullable<double> ABBSZW { get; set; }
        [Display(Name = "ABBSZL", ResourceType = typeof(Resource))]
        public Nullable<double> ABBSZL { get; set; }
        [Display(Name = "ABDLVDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> ABDLVDT { get; set; }
        [Display(Name = "ABQTY", ResourceType = typeof(Resource))]
        public Nullable<double> ABQTY { get; set; }
        [Display(Name = "ABWT", ResourceType = typeof(Resource))]
        public Nullable<double> ABWT { get; set; }
        [Display(Name = "ABSEDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> ABSEDT { get; set; }
        [Display(Name = "ABTTLWT", ResourceType = typeof(Resource))]
        public Nullable<double> ABTTLWT { get; set; }
        [Display(Name = "ABSESWT", ResourceType = typeof(Resource))]
        public Nullable<double> ABSESWT { get; set; }
        [Display(Name = "ABPRUPU", ResourceType = typeof(Resource))]
        public string ABPRUPU { get; set; }
        [Display(Name = "ABPRUP", ResourceType = typeof(Resource))]
        public Nullable<double> ABPRUP { get; set; }
        [Display(Name = "ABPRUPD", ResourceType = typeof(Resource))]
        public Nullable<double> ABPRUPD { get; set; }
        [Display(Name = "ABPRAT", ResourceType = typeof(Resource))]
        public Nullable<double> ABPRAT { get; set; }
        [Display(Name = "ABPRATD", ResourceType = typeof(Resource))]
        public Nullable<double> ABPRATD { get; set; }
        [Display(Name = "ABPTXAT", ResourceType = typeof(Resource))]
        public Nullable<double> ABPTXAT { get; set; }
        [Display(Name = "ABPTXAD", ResourceType = typeof(Resource))]
        public Nullable<double> ABPTXAD { get; set; }
        [Display(Name = "ABORDNO", ResourceType = typeof(Resource))]
        public string ABORDNO { get; set; }

        [Display(Name = "AARMTP", ResourceType = typeof(Resource))]
        public string AARMTP { get; set; }
        [Display(Name = "RAPSTLGR", ResourceType = typeof(Resource))]
        public string RAPSTLGR { get; set; }
        [Display(Name = "ABPRDNM", ResourceType = typeof(Resource))]
        public string ABPRDNM { get; set; }
        [Display(Name = "ABPRDDIA", ResourceType = typeof(Resource))]
        public string ABPRDDIA { get; set; }
        [Display(Name = "ABPIVNO", ResourceType = typeof(Resource))]
        public string ABPIVNO { get; set; }
        [Display(Name = "ABPIVDT", ResourceType = typeof(Resource))]
        public string ABPIVDT { get; set; }
        [Display(Name = "ABVESEL", ResourceType = typeof(Resource))]
        public string ABVESEL { get; set; }


    }
}