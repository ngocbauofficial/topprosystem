using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StaticResources.View.TRA001;

namespace TopProSystem.Areas.MasterSetting.Models
{
    [MetadataType(typeof(MetaDta_TRA001))]
    public partial class TRA001
    {
    }

    public  class MetaDta_TRA001
    {
        [Display(Name = "DAUPDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DAUPDT { get; set; }
        [Display(Name = "DATRNDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DATRNDT { get; set; }
        [Display(Name = "DAWSID", ResourceType = typeof(Resource))]
        public string DAWSID { get; set; }
        [Display(Name = "DAPGMID", ResourceType = typeof(Resource))]
        public string DAPGMID { get; set; }
        [Display(Name = "DALOGDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DALOGDT { get; set; }
        [Display(Name = "DALOGTP", ResourceType = typeof(Resource))]
        public string DALOGTP { get; set; }
        [Display(Name = "DAINVTP", ResourceType = typeof(Resource))]
        public string DAINVTP { get; set; }
        [Display(Name = "DAINVNO", ResourceType = typeof(Resource))]
        public string DAINVNO { get; set; }
        [Display(Name = "DACMDCD", ResourceType = typeof(Resource))]
        public string DACMDCD { get; set; }
        [Display(Name = "DAUSRCD", ResourceType = typeof(Resource))]
        public string DAUSRCD { get; set; }
        [Display(Name = "DACTRTP", ResourceType = typeof(Resource))]
        public string DACTRTP { get; set; }
        [Display(Name = "DADPSCD", ResourceType = typeof(Resource))]
        public string DADPSCD { get; set; }
        [Display(Name = "DASPTCD", ResourceType = typeof(Resource))]
        public string DASPTCD { get; set; }
        [Display(Name = "DADFTSR", ResourceType = typeof(Resource))]
        public string DADFTSR { get; set; }
        [Display(Name = "DANORVS", ResourceType = typeof(Resource))]
        public Nullable<double> DANORVS { get; set; }
        [Display(Name = "DABKCD", ResourceType = typeof(Resource))]
        public string DABKCD { get; set; }
        [Display(Name = "DABKTMG", ResourceType = typeof(Resource))]
        public string DABKTMG { get; set; }
        [Display(Name = "DAPURGD", ResourceType = typeof(Resource))]
        public string DAPURGD { get; set; }
        [Display(Name = "DALCTCD", ResourceType = typeof(Resource))]
        public string DALCTCD { get; set; }
        [Display(Name = "DASTSCD", ResourceType = typeof(Resource))]
        public string DASTSCD { get; set; }
        [Display(Name = "DARSNCD", ResourceType = typeof(Resource))]
        public string DARSNCD { get; set; }
        [Display(Name = "DASECCD", ResourceType = typeof(Resource))]
        public string DASECCD { get; set; }
        [Display(Name = "DASCTNC", ResourceType = typeof(Resource))]
        public string DASCTNC { get; set; }
        [Display(Name = "DAIDCD", ResourceType = typeof(Resource))]
        public string DAIDCD { get; set; }
        [Display(Name = "DACHGWK", ResourceType = typeof(Resource))]
        public string DACHGWK { get; set; }
        [Display(Name = "DATRDCD", ResourceType = typeof(Resource))]
        public string DATRDCD { get; set; }
        [Display(Name = "DASPEC", ResourceType = typeof(Resource))]
        public string DASPEC { get; set; }
        [Display(Name = "DACOAT", ResourceType = typeof(Resource))]
        public string DACOAT { get; set; }
        [Display(Name = "DAGRADE", ResourceType = typeof(Resource))]
        public string DAGRADE { get; set; }
        [Display(Name = "DABSZT", ResourceType = typeof(Resource))]
        public Nullable<double> DABSZT { get; set; }
        [Display(Name = "DABSZW", ResourceType = typeof(Resource))]
        public Nullable<double> DABSZW { get; set; }
        [Display(Name = "DABSZL", ResourceType = typeof(Resource))]
        public Nullable<double> DABSZL { get; set; }
        [Display(Name = "DAQTY", ResourceType = typeof(Resource))]
        public Nullable<double> DAQTY { get; set; }
        [Display(Name = "DAINPCK", ResourceType = typeof(Resource))]
        public Nullable<double> DAINPCK { get; set; }
        [Display(Name = "DAWTCD", ResourceType = typeof(Resource))]
        public string DAWTCD { get; set; }
        [Display(Name = "DAWT", ResourceType = typeof(Resource))]
        public Nullable<double> DAWT { get; set; }
        [Display(Name = "DAPUUPU", ResourceType = typeof(Resource))]
        public string DAPUUPU { get; set; }
        [Display(Name = "DAPURUP", ResourceType = typeof(Resource))]
        public Nullable<double> DAPURUP { get; set; }
        [Display(Name = "DACTRNO", ResourceType = typeof(Resource))]
        public string DACTRNO { get; set; }
        [Display(Name = "DACTITM", ResourceType = typeof(Resource))]
        public Nullable<double> DACTITM { get; set; }
        [Display(Name = "DAMKCD", ResourceType = typeof(Resource))]
        public string DAMKCD { get; set; }
        [Display(Name = "DAORDNO", ResourceType = typeof(Resource))]
        public string DAORDNO { get; set; }
        [Display(Name = "DASPLCD", ResourceType = typeof(Resource))]
        public string DASPLCD { get; set; }
        [Display(Name = "DASPPNO", ResourceType = typeof(Resource))]
        public string DASPPNO { get; set; }
        [Display(Name = "DATCNO", ResourceType = typeof(Resource))]
        public string DATCNO { get; set; }
        [Display(Name = "DAVESEL", ResourceType = typeof(Resource))]
        public string DAVESEL { get; set; }
        [Display(Name = "DASMNTH", ResourceType = typeof(Resource))]
        public Nullable<int> DASMNTH { get; set; }
        [Display(Name = "DAPIVNO", ResourceType = typeof(Resource))]
        public string DAPIVNO { get; set; }
        [Display(Name = "DAPIVDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DAPIVDT { get; set; }
        [Display(Name = "DAISPNO", ResourceType = typeof(Resource))]
        public string DAISPNO { get; set; }
        [Display(Name = "DASEDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DASEDT { get; set; }
        [Display(Name = "DAEXRT", ResourceType = typeof(Resource))]
        public Nullable<double> DAEXRT { get; set; }
        [Display(Name = "DAEXRTT", ResourceType = typeof(Resource))]
        public string DAEXRTT { get; set; }
        [Display(Name = "DATXEXR", ResourceType = typeof(Resource))]
        public Nullable<double> DATXEXR { get; set; }
        [Display(Name = "DATEXRT", ResourceType = typeof(Resource))]
        public string DATEXRT { get; set; }
        [Display(Name = "DATXCD", ResourceType = typeof(Resource))]
        public string DATXCD { get; set; }
        [Display(Name = "DATXRT", ResourceType = typeof(Resource))]
        public Nullable<double> DATXRT { get; set; }
        [Display(Name = "DAPSLP", ResourceType = typeof(Resource))]
        public string DAPSLP { get; set; }
        [Display(Name = "DAPSLPI", ResourceType = typeof(Resource))]
        public Nullable<double> DAPSLPI { get; set; }
        [Display(Name = "DAOPSLP", ResourceType = typeof(Resource))]
        public string DAOPSLP { get; set; }
        [Display(Name = "DAIOPSL", ResourceType = typeof(Resource))]
        public Nullable<double> DAIOPSL { get; set; }
        [Display(Name = "DAPUFRT", ResourceType = typeof(Resource))]
        public string DAPUFRT { get; set; }
        [Display(Name = "DAPFUPU", ResourceType = typeof(Resource))]
        public string DAPFUPU { get; set; }
        [Display(Name = "DAPFRUP", ResourceType = typeof(Resource))]
        public Nullable<double> DAPFRUP { get; set; }
        [Display(Name = "DACTPDT", ResourceType = typeof(Resource))]
        public string DACTPDT { get; set; }
        [Display(Name = "DACTUPU", ResourceType = typeof(Resource))]
        public string DACTUPU { get; set; }
        [Display(Name = "DACTUP", ResourceType = typeof(Resource))]
        public Nullable<double> DACTUP { get; set; }
        [Display(Name = "DATRPDT", ResourceType = typeof(Resource))]
        public string DATRPDT { get; set; }
        [Display(Name = "DATRUPU", ResourceType = typeof(Resource))]
        public string DATRUPU { get; set; }
        [Display(Name = "DATRFUP", ResourceType = typeof(Resource))]
        public Nullable<double> DATRFUP { get; set; }
        [Display(Name = "DAOENO", ResourceType = typeof(Resource))]
        public string DAOENO { get; set; }
        [Display(Name = "DAOEITM", ResourceType = typeof(Resource))]
        public Nullable<double> DAOEITM { get; set; }
        [Display(Name = "DAOEDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DAOEDT { get; set; }
        [Display(Name = "DACSTCD", ResourceType = typeof(Resource))]
        public string DACSTCD { get; set; }
        [Display(Name = "DACTSPC", ResourceType = typeof(Resource))]
        public string DACTSPC { get; set; }
        [Display(Name = "DAPONO", ResourceType = typeof(Resource))]
        public string DAPONO { get; set; }
        [Display(Name = "DAPRDNM", ResourceType = typeof(Resource))]
        public string DAPRDNM { get; set; }
        [Display(Name = "DACTINF", ResourceType = typeof(Resource))]
        public string DACTINF { get; set; }
        [Display(Name = "DAPCKTP", ResourceType = typeof(Resource))]
        public string DAPCKTP { get; set; }
        [Display(Name = "DASLUPU", ResourceType = typeof(Resource))]
        public string DASLUPU { get; set; }
        [Display(Name = "DASLUP", ResourceType = typeof(Resource))]
        public Nullable<double> DASLUP { get; set; }
        [Display(Name = "DAUSMNG", ResourceType = typeof(Resource))]
        public string DAUSMNG { get; set; }
        [Display(Name = "DAJOBNO", ResourceType = typeof(Resource))]
        public string DAJOBNO { get; set; }
        [Display(Name = "DAJBITM", ResourceType = typeof(Resource))]
        public Nullable<double> DAJBITM { get; set; }
        [Display(Name = "DAPRCDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DAPRCDT { get; set; }
        [Display(Name = "DAPRCTP", ResourceType = typeof(Resource))]
        public string DAPRCTP { get; set; }
        [Display(Name = "DAMCHNO", ResourceType = typeof(Resource))]
        public string DAMCHNO { get; set; }
        [Display(Name = "DAOGLNO", ResourceType = typeof(Resource))]
        public string DAOGLNO { get; set; }
        [Display(Name = "DASHIFT", ResourceType = typeof(Resource))]
        public string DASHIFT { get; set; }
        [Display(Name = "DABALWT", ResourceType = typeof(Resource))]
        public Nullable<double> DABALWT { get; set; }
        [Display(Name = "DACALWT", ResourceType = typeof(Resource))]
        public Nullable<double> DACALWT { get; set; }
        [Display(Name = "DAPCKNO", ResourceType = typeof(Resource))]
        public string DAPCKNO { get; set; }
        [Display(Name = "DAPCKDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DAPCKDT { get; set; }
        [Display(Name = "DAPCKM", ResourceType = typeof(Resource))]
        public string DAPCKM { get; set; }
        [Display(Name = "DAADJNO", ResourceType = typeof(Resource))]
        public string DAADJNO { get; set; }
        [Display(Name = "DADINO", ResourceType = typeof(Resource))]
        public string DADINO { get; set; }
        [Display(Name = "DADONO", ResourceType = typeof(Resource))]
        public string DADONO { get; set; }
        [Display(Name = "DADOSUB", ResourceType = typeof(Resource))]
        public Nullable<double> DADOSUB { get; set; }
        [Display(Name = "DADLVTO", ResourceType = typeof(Resource))]
        public string DADLVTO { get; set; }
        [Display(Name = "DADLDTT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DADLDTT { get; set; }
        [Display(Name = "DAIVCDT", ResourceType = typeof(Resource))]
        public Nullable<System.DateTime> DAIVCDT { get; set; }
        [Display(Name = "DASSLP", ResourceType = typeof(Resource))]
        public string DASSLP { get; set; }
        [Display(Name = "DAISSLP", ResourceType = typeof(Resource))]
        public Nullable<double> DAISSLP { get; set; }
        [Display(Name = "DAOSSLP", ResourceType = typeof(Resource))]
        public string DAOSSLP { get; set; }
        [Display(Name = "DAIOSSL", ResourceType = typeof(Resource))]
        public Nullable<double> DAIOSSL { get; set; }
        [Display(Name = "DAFRPDT", ResourceType = typeof(Resource))]
        public string DAFRPDT { get; set; }
        [Display(Name = "DAFRUPU", ResourceType = typeof(Resource))]
        public string DAFRUPU { get; set; }
        [Display(Name = "DAFRTUP", ResourceType = typeof(Resource))]
        public Nullable<double> DAFRTUP { get; set; }
        [Display(Name = "DATRUCK", ResourceType = typeof(Resource))]
        public string DATRUCK { get; set; }
        [Display(Name = "DARMK20", ResourceType = typeof(Resource))]
        public string DARMK20 { get; set; }
        [Display(Name = "DAFHPDT", ResourceType = typeof(Resource))]
        public string DAFHPDT { get; set; }
        [Display(Name = "DAUPUHF", ResourceType = typeof(Resource))]
        public string DAUPUHF { get; set; }
        [Display(Name = "DAUPHFR", ResourceType = typeof(Resource))]
        public Nullable<double> DAUPHFR { get; set; }
        [Display(Name = "DAPURAT", ResourceType = typeof(Resource))]
        public Nullable<double> DAPURAT { get; set; }
        [Display(Name = "DAPUFAT", ResourceType = typeof(Resource))]
        public Nullable<double> DAPUFAT { get; set; }
        [Display(Name = "DAHCGAT", ResourceType = typeof(Resource))]
        public Nullable<double> DAHCGAT { get; set; }
        [Display(Name = "DATRFAT", ResourceType = typeof(Resource))]
        public Nullable<double> DATRFAT { get; set; }
        [Display(Name = "DACSTHC", ResourceType = typeof(Resource))]
        public Nullable<double> DACSTHC { get; set; }
        [Display(Name = "DAYLDAT", ResourceType = typeof(Resource))]
        public Nullable<double> DAYLDAT { get; set; }
        [Display(Name = "DAINTPC", ResourceType = typeof(Resource))]
        public Nullable<double> DAINTPC { get; set; }
        [Display(Name = "DAPRATB", ResourceType = typeof(Resource))]
        public Nullable<double> DAPRATB { get; set; }
        [Display(Name = "DAINTPK", ResourceType = typeof(Resource))]
        public Nullable<double> DAINTPK { get; set; }
        [Display(Name = "DAPKATB", ResourceType = typeof(Resource))]
        public Nullable<double> DAPKATB { get; set; }

    }
}