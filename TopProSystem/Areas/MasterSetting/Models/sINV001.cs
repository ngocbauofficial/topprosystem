using System;
using System.ComponentModel.DataAnnotations;
using StaticResources.View.INV001;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.WebPages.Html;

namespace TopProSystem.Areas.MasterSetting.Models
{
    [MetadataType(typeof(MetaData_INV001))]
    public partial class INV001
    {
        [NotMapped]
        public double? NCAPUFAT { get; set; }
        [NotMapped]
        public double? NCACSTHC { get; set; }
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
        public Dictionary<string, string> LabelType { get; set; }
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

        [NotMapped]
        public Dictionary<string, string> InventoryType { get; set; }
        [NotMapped]
        public Dictionary<string, string> InventoryStatus { get; set; }
        [NotMapped]
        public Dictionary<string, string> Packing { get; set; }
        [NotMapped]
        public Dictionary<string, string> Location { get; set; }
    }

    public class MetaData_INV001
    {

        [Display(Name = "CARGSDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CARGSDT { get; set; }
        [Display(Name = "CAUPDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAUPDT { get; set; }
        [Display(Name = "CADTCD", ResourceType = typeof(Resource))]
        public string CADTCD { get; set; }
        [Display(Name = "CACTLM1", ResourceType = typeof(Resource))]
        public string CACTLM1 { get; set; }
        [Display(Name = "CAINVTP", ResourceType = typeof(Resource))]
        public string CAINVTP { get; set; }
        [Display(Name = "CAINVST", ResourceType = typeof(Resource))]
        public string CAINVST { get; set; }
        [Display(Name = "CAINVNO", ResourceType = typeof(Resource))]
        public string CAINVNO { get; set; }
        [Display(Name = "CACMDCD", ResourceType = typeof(Resource))]
        public string CACMDCD { get; set; }
        [Display(Name = "CAUSRCD", ResourceType = typeof(Resource))]
        public string CAUSRCD { get; set; }
        [Display(Name = "CACTRTP", ResourceType = typeof(Resource))]
        public string CACTRTP { get; set; }
        [Display(Name = "CADPSCD", ResourceType = typeof(Resource))]
        public string CADPSCD { get; set; }
        [Display(Name = "CASPTCD", ResourceType = typeof(Resource))]
        public string CASPTCD { get; set; }
        [Display(Name = "CAPURGD", ResourceType = typeof(Resource))]
        public string CAPURGD { get; set; }
        [Display(Name = "CALCTCD", ResourceType = typeof(Resource))]
        public string CALCTCD { get; set; }
        [Display(Name = "CASTSCD", ResourceType = typeof(Resource))]
        public string CASTSCD { get; set; }
        [Display(Name = "CARSNCD", ResourceType = typeof(Resource))]
        public string CARSNCD { get; set; }
        [Display(Name = "CASECCD", ResourceType = typeof(Resource))]
        public string CASECCD { get; set; }
        [Display(Name = "CASCTNC", ResourceType = typeof(Resource))]
        public string CASCTNC { get; set; }
        [Display(Name = "CAIDCD", ResourceType = typeof(Resource))]
        public string CAIDCD { get; set; }
        [Display(Name = "CATRDCD", ResourceType = typeof(Resource))]
        public string CATRDCD { get; set; }
        [Display(Name = "CASPEC", ResourceType = typeof(Resource))]
        public string CASPEC { get; set; }
        [Display(Name = "CACOAT", ResourceType = typeof(Resource))]
        public string CACOAT { get; set; }
        [Display(Name = "CACOATF", ResourceType = typeof(Resource))]
        public Nullable<double> CACOATF { get; set; }
        [Display(Name = "CACOATB", ResourceType = typeof(Resource))]
        public Nullable<double> CACOATB { get; set; }
        [Display(Name = "CAGRADE", ResourceType = typeof(Resource))]
        public string CAGRADE { get; set; }
        [Display(Name = "CACMBT", ResourceType = typeof(Resource))]
        public string CACMBT { get; set; }
        [Display(Name = "CACMBW", ResourceType = typeof(Resource))]
        public string CACMBW { get; set; }
        [Display(Name = "CACMBL", ResourceType = typeof(Resource))]
        public string CACMBL { get; set; }
        [Display(Name = "CASIZET", ResourceType = typeof(Resource))]
        public Nullable<double> CASIZET { get; set; }
        [Display(Name = "CASIZEW", ResourceType = typeof(Resource))]
        public Nullable<double> CASIZEW { get; set; }
        [Display(Name = "CASIZEL", ResourceType = typeof(Resource))]
        public Nullable<double> CASIZEL { get; set; }
        [Display(Name = "CABSZT", ResourceType = typeof(Resource))]
        public Nullable<double> CABSZT { get; set; }
        [Display(Name = "CABSZW", ResourceType = typeof(Resource))]
        public Nullable<double> CABSZW { get; set; }
        [Display(Name = "CABSZL", ResourceType = typeof(Resource))]
        public Nullable<double> CABSZL { get; set; }
        [Display(Name = "CACILID", ResourceType = typeof(Resource))]
        public Nullable<double> CACILID { get; set; }
        [Display(Name = "CACILOD", ResourceType = typeof(Resource))]
        public Nullable<double> CACILOD { get; set; }
        [Display(Name = "CAQTY", ResourceType = typeof(Resource))]
        public Nullable<double> CAQTY { get; set; }
        [Display(Name = "CAINPCK", ResourceType = typeof(Resource))]
        public Nullable<double> CAINPCK { get; set; }
        [Display(Name = "CAWT", ResourceType = typeof(Resource))]
        public Nullable<double> CAWT { get; set; }
        [Display(Name = "CAPRUPU", ResourceType = typeof(Resource))]
        public string CAPRUPU { get; set; }
        [Display(Name = "CAPURUP", ResourceType = typeof(Resource))]
        public Nullable<double> CAPURUP { get; set; }
        [Display(Name = "CAPRUPD", ResourceType = typeof(Resource))]
        public Nullable<double> CAPRUPD { get; set; }
        [Display(Name = "CADNSTY", ResourceType = typeof(Resource))]
        public Nullable<double> CADNSTY { get; set; }
        [Display(Name = "CACTRNO", ResourceType = typeof(Resource))]
        public string CACTRNO { get; set; }
        [Display(Name = "CACTITM", ResourceType = typeof(Resource))]
        public Nullable<double> CACTITM { get; set; }
        [Display(Name = "CAMKCD", ResourceType = typeof(Resource))]
        public string CAMKCD { get; set; }
        [Display(Name = "CAORDNO", ResourceType = typeof(Resource))]
        public string CAORDNO { get; set; }
        [Display(Name = "CASPLCD", ResourceType = typeof(Resource))]
        public string CASPLCD { get; set; }
        [Display(Name = "CASPPNO", ResourceType = typeof(Resource))]
        public string CASPPNO { get; set; }
        [Display(Name = "CATCNO", ResourceType = typeof(Resource))]
        public string CATCNO { get; set; }
        [Display(Name = "CAMCSPC", ResourceType = typeof(Resource))]
        public string CAMCSPC { get; set; }
        [Display(Name = "CAVESEL", ResourceType = typeof(Resource))]
        public string CAVESEL { get; set; }
        [Display(Name = "CASMNTH", ResourceType = typeof(Resource))]
        public Nullable<int> CASMNTH { get; set; }
        [Display(Name = "CAPIVNO", ResourceType = typeof(Resource))]
        public string CAPIVNO { get; set; }
        [Display(Name = "CAPIVDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAPIVDT { get; set; }
        [Display(Name = "CAISPNO", ResourceType = typeof(Resource))]
        public string CAISPNO { get; set; }
        [Display(Name = "CACRLOS", ResourceType = typeof(Resource))]
        public Nullable<double> CACRLOS { get; set; }
        [Display(Name = "CAINDCT", ResourceType = typeof(Resource))]
        public Nullable<double> CAINDCT { get; set; }
        [Display(Name = "CASEDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CASEDT { get; set; }
        [Display(Name = "CASHPDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CASHPDT { get; set; }
        [Display(Name = "CASTKPO", ResourceType = typeof(Resource))]
        public string CASTKPO { get; set; }
        [Display(Name = "CAPODT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAPODT { get; set; }
        [Display(Name = "CAPAYDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAPAYDT { get; set; }
        [Display(Name = "CADCRCD", ResourceType = typeof(Resource))]
        public string CADCRCD { get; set; }
        [Display(Name = "CACRRCD", ResourceType = typeof(Resource))]
        public string CACRRCD { get; set; }
        [Display(Name = "CAEXRT", ResourceType = typeof(Resource))]
        public Nullable<double> CAEXRT { get; set; }
        [Display(Name = "CAEXRTT", ResourceType = typeof(Resource))]
        public string CAEXRTT { get; set; }
        [Display(Name = "CATXEXR", ResourceType = typeof(Resource))]
        public Nullable<double> CATXEXR { get; set; }
        [Display(Name = "CATEXRT", ResourceType = typeof(Resource))]
        public string CATEXRT { get; set; }
        [Display(Name = "CATXCD", ResourceType = typeof(Resource))]
        public string CATXCD { get; set; }
        [Display(Name = "CATXRT", ResourceType = typeof(Resource))]
        public Nullable<double> CATXRT { get; set; }
        [Display(Name = "CAPSLP", ResourceType = typeof(Resource))]
        public string CAPSLP { get; set; }
        [Display(Name = "CAPSLPI", ResourceType = typeof(Resource))]
        public Nullable<double> CAPSLPI { get; set; }
        [Display(Name = "CAPUFRT", ResourceType = typeof(Resource))]
        public string CAPUFRT { get; set; }
        [Display(Name = "CAPFUPU", ResourceType = typeof(Resource))]
        public string CAPFUPU { get; set; }
        [Display(Name = "CAPFRUP", ResourceType = typeof(Resource))]
        public Nullable<double> CAPFRUP { get; set; }
        [Display(Name = "CACTPDT", ResourceType = typeof(Resource))]
        public string CACTPDT { get; set; }
        [Display(Name = "CACTUPU", ResourceType = typeof(Resource))]
        public string CACTUPU { get; set; }
        [Display(Name = "CACTUP", ResourceType = typeof(Resource))]
        public Nullable<double> CACTUP { get; set; }
        [Display(Name = "CATRPDT", ResourceType = typeof(Resource))]
        public string CATRPDT { get; set; }
        [Display(Name = "CATRUPU", ResourceType = typeof(Resource))]
        public string CATRUPU { get; set; }
        [Display(Name = "CATRFUP", ResourceType = typeof(Resource))]
        public Nullable<double> CATRFUP { get; set; }
        [Display(Name = "CAOENO", ResourceType = typeof(Resource))]
        public string CAOENO { get; set; }
        [Display(Name = "CAOEITM", ResourceType = typeof(Resource))]
        public Nullable<double> CAOEITM { get; set; }
        [Display(Name = "CACSTCD", ResourceType = typeof(Resource))]
        public string CACSTCD { get; set; }
        [Display(Name = "CADLVDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CADLVDT { get; set; }
        [Display(Name = "CACTSPC", ResourceType = typeof(Resource))]
        public string CACTSPC { get; set; }
        [Display(Name = "CAOEPO", ResourceType = typeof(Resource))]
        public string CAOEPO { get; set; }
        [Display(Name = "CAPRDNM", ResourceType = typeof(Resource))]
        public string CAPRDNM { get; set; }
        [Display(Name = "CAPCKTP", ResourceType = typeof(Resource))]
        public string CAPCKTP { get; set; }
        [Display(Name = "CAUSMNG", ResourceType = typeof(Resource))]
        public string CAUSMNG { get; set; }
        [Display(Name = "CASLUWT", ResourceType = typeof(Resource))]
        public Nullable<double> CASLUWT { get; set; }
        [Display(Name = "CAPCKWT", ResourceType = typeof(Resource))]
        public Nullable<double> CAPCKWT { get; set; }
        [Display(Name = "CACALWT", ResourceType = typeof(Resource))]
        public Nullable<double> CACALWT { get; set; }
        [Display(Name = "CAJOBNO", ResourceType = typeof(Resource))]
        public string CAJOBNO { get; set; }
        [Display(Name = "CAJBITM", ResourceType = typeof(Resource))]
        public Nullable<double> CAJBITM { get; set; }
        [Display(Name = "CAPRCDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAPRCDT { get; set; }
        [Display(Name = "CAPRCTP", ResourceType = typeof(Resource))]
        public string CAPRCTP { get; set; }
        [Display(Name = "CAMCHNO", ResourceType = typeof(Resource))]
        public string CAMCHNO { get; set; }
        [Display(Name = "CASUBNO", ResourceType = typeof(Resource))]
        public string CASUBNO { get; set; }
        [Display(Name = "CAOGLNO", ResourceType = typeof(Resource))]
        public string CAOGLNO { get; set; }
        [Display(Name = "CAISSDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAISSDT { get; set; }
        [Display(Name = "CAANNDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAANNDT { get; set; }
        [Display(Name = "CADMGCD", ResourceType = typeof(Resource))]
        public string CADMGCD { get; set; }
        [Display(Name = "CAPCKNO", ResourceType = typeof(Resource))]
        public string CAPCKNO { get; set; }
        [Display(Name = "CAPCKDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAPCKDT { get; set; }
        [Display(Name = "CAPCKM", ResourceType = typeof(Resource))]
        public string CAPCKM { get; set; }
        [Display(Name = "CAADJNO", ResourceType = typeof(Resource))]
        public string CAADJNO { get; set; }
        [Display(Name = "CADINO", ResourceType = typeof(Resource))]
        public string CADINO { get; set; }
        [Display(Name = "CADONO", ResourceType = typeof(Resource))]
        public string CADONO { get; set; }
        [Display(Name = "CADLVTO", ResourceType = typeof(Resource))]
        public string CADLVTO { get; set; }
        [Display(Name = "CADLDTT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CADLDTT { get; set; }
        [Display(Name = "CAIVCDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> CAIVCDT { get; set; }
        [Display(Name = "CASSLP", ResourceType = typeof(Resource))]
        public string CASSLP { get; set; }
        [Display(Name = "CAISSLP", ResourceType = typeof(Resource))]
        public Nullable<double> CAISSLP { get; set; }
        [Display(Name = "CAPURAT", ResourceType = typeof(Resource))]
        public Nullable<double> CAPURAT { get; set; }
        [Display(Name = "CAPUFAT", ResourceType = typeof(Resource))]
        public Nullable<double> CAPUFAT { get; set; }
        [Display(Name = "CAHCGAT", ResourceType = typeof(Resource))]
        public Nullable<double> CAHCGAT { get; set; }
        [Display(Name = "CATRFAT", ResourceType = typeof(Resource))]
        public Nullable<double> CATRFAT { get; set; }
        [Display(Name = "CACSTHC", ResourceType = typeof(Resource))]
        public Nullable<double> CACSTHC { get; set; }
        [Display(Name = "CAYLDAT", ResourceType = typeof(Resource))]
        public Nullable<double> CAYLDAT { get; set; }
        [Display(Name = "CAINTPC", ResourceType = typeof(Resource))]
        public Nullable<double> CAINTPC { get; set; }
        [Display(Name = "CAPRATB", ResourceType = typeof(Resource))]
        public Nullable<double> CAPRATB { get; set; }
        [Display(Name = "CAINTPK", ResourceType = typeof(Resource))]
        public Nullable<double> CAINTPK { get; set; }
        [Display(Name = "CAPKATB", ResourceType = typeof(Resource))]
        public Nullable<double> CAPKATB { get; set; }

        [Display(Name = "CARMTP", ResourceType = typeof(Resource))]
        public Nullable<double> CARMTP { get; set; }

        [Display(Name = "CADLVCD", ResourceType = typeof(Resource))]
        public Nullable<double> CADLVCD { get; set; }

        [Display(Name = "CAPRUP", ResourceType = typeof(Resource))]
        public Nullable<double> CAPRUP { get; set; }

        [Display(Name = "CAPURATD", ResourceType = typeof(Resource))]
        public Nullable<double> CAPURATD { get; set; }

        [Display(Name = "CAPTXAT", ResourceType = typeof(Resource))]
        public Nullable<double> CAPTXAT { get; set; }

        [Display(Name = "CAPTXAD", ResourceType = typeof(Resource))]
        public Nullable<double> CAPTXAD { get; set; }

        [Display(Name = "CAPRDDIA", ResourceType = typeof(Resource))]
        public Nullable<double> CAPRDDIA { get; set; }

        [Display(Name = "CASTLGR", ResourceType = typeof(Resource))]
        public Nullable<double> CASTLGR { get; set; }
    }


}