using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopProSystem.Areas.MasterSetting.Models;
namespace TopProSystem.Areas.MasterSetting.ForeignKeyConstraint
{
    public static class Constraint
    {
        private static TopProSystemEntities entities = new TopProSystemEntities();

        public static string CheckConstraintMA001(string Code)
        {
            string messageReturn = String.Empty;
            if (entities.PUR001.AsNoTracking().Count(x => x.AASPLCD.Trim().Equals(Code)) > 0)
            {
                messageReturn = "Purchase Contract";
            }
            return messageReturn;
        }

        public static string CheckConstraintMA002(string srCode)
        {
            string messageReturn = String.Empty;
            if (entities.PUR001.AsNoTracking().Count(x => x.AAUSRCD.Trim().Equals(srCode)) > 0)
            {
                messageReturn = "Purchase Contract";
            }
            return messageReturn;
        }

        public static string CheckConstraintMA003(string srCode)
        {
            string messageReturn = String.Empty;
            if (entities.PUR001.AsNoTracking().Count(x => x.AAIDCD.Trim().Equals(srCode)) > 0)
            {
                messageReturn = "Purchase Contract";
            }
            return messageReturn;
        }
        public static string CheckConstraintMA006(string spec)
        {
            string messageReturn = String.Empty;
            if (entities.PUR001.AsNoTracking().Count(x => x.ABMCSPC.Trim().Equals(spec)) > 0)
            {
                messageReturn = "Purchase Contract";
            }
            return messageReturn;
        }

        public static string CheckConstraintMA005(string coating)
        {
            string messageReturn = String.Empty;
            if (entities.PUR001.AsNoTracking().Count(x => x.ABCOAT.Trim().Equals(coating)) > 0)
            {
                messageReturn = "Purchase Contract";
            }
            return messageReturn;
        }

        public static string CheckConstraintMA009(int srCode)
        {
            var ma009 = entities.MA009.Find(srCode);
            var date = DateTime.ParseExact(ConverMonthYear(ma009.MJEXRTD), "MMyyyy", null);
            string messageReturn = String.Empty;
            if (entities.PUR001.AsNoTracking().Count(x => x.AARGSDT.Value.Month == date.Month && x.AARGSDT.Value.Year == date.Year && x.AAEXRTT == ma009.MJEXRTT && x.AACRRCD == ma009.MJCRRCD && x.Deleted != 1) > 0)
            {
                messageReturn = "Purchase Contract";
            }
            return messageReturn;
        }
        public static string ConverMonthYear(double? MJEXRTD)
        {
            string a = MJEXRTD.ToString();
            if (a.Length == 5)
            {
                a = "0" + a;
                return a;
            }
            else
                return a;
        }
        public static string CheckConstraintMA010(string taxCode)
        {
            string messageReturn = String.Empty;
            if (entities.MA001.AsNoTracking().Count(x => x.MAPTXCD == taxCode || x.MASTXCD == taxCode) > 0)
            {
                messageReturn = StaticResources.ItemMenuMaster.SalePurchaseMaster;
            }
            if (entities.PUR001.AsNoTracking().Count(x => x.AAMKCD.Trim().Equals(taxCode)) > 0)
            {
                if (!String.IsNullOrEmpty(messageReturn))
                {
                    messageReturn = messageReturn + "Purchase Contract";
                }
                else
                {
                    messageReturn = "Purchase Contract";
                }
            }

            return messageReturn;
        }
        public static string CheckMA012Constraint(string classificationCode, string srCode)
        {
            string messageReturn = String.Empty;

            switch (classificationCode)
            {
                case ClassificationCode.CLASSIFICATTIONCODE001:
                    if (entities.MA001.AsNoTracking().Count(x => x.MAPTDUE.Equals(srCode) || x.MASTDUE.Equals(srCode)) > 0)
                        messageReturn = StaticResources.ItemMenuMaster.SalePurchaseMaster;
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE002:
                    if (entities.MA001.AsNoTracking().Count(x => x.MAPDAYS.Equals(srCode) || x.MASDAYS.Equals(srCode)) > 0)
                        messageReturn = StaticResources.ItemMenuMaster.SalePurchaseMaster;
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE003:
                    if (entities.MA001.AsNoTracking().Count(x => x.MAPCALC.Equals(srCode) || x.MASCALC.Equals(srCode)) > 0)
                        messageReturn = StaticResources.ItemMenuMaster.SalePurchaseMaster;
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE004:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE005:
                    if (entities.PUR001.AsNoTracking().Count(x => x.AAMKCD.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE006:
                    if (entities.PUR001.AsNoTracking().Count(x => x.AACMDCD.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE007:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE008:
                    if (entities.MA003.AsNoTracking().Count(x => x.MCSCTNC.Equals(srCode)) > 0)
                        messageReturn = StaticResources.ItemMenuMaster.UserIDMaster;
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE009:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE010:
                    if (entities.MA001.AsNoTracking().Count(x => x.MACNTRC.Equals(srCode)) > 0)
                        messageReturn = StaticResources.ItemMenuMaster.SalePurchaseMaster;
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE011:
                    if (entities.MA001.AsNoTracking().Count(x => x.MABUZCD.Equals(srCode)) > 0)
                        messageReturn = StaticResources.ItemMenuMaster.SalePurchaseMaster;
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE012:
                    if (entities.MA001.AsNoTracking().Count(x => x.MAPCRCD.Equals(srCode) || x.MASCRCD.Equals(srCode)) > 0)
                        messageReturn = StaticResources.ItemMenuMaster.SalePurchaseMaster;
                    if (entities.MA009.AsNoTracking().Count(x => x.MJCRRCD.Trim().Equals(srCode)) > 0)
                        if (!String.IsNullOrEmpty(messageReturn))
                        {
                            messageReturn = messageReturn + "&" + StaticResources.ItemMenuMaster.ExchangeRateMaster;
                        }
                        else
                        {
                            messageReturn = StaticResources.ItemMenuMaster.ExchangeRateMaster;
                        }
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE013:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE014:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE015:
                    if (entities.PUR001.AsNoTracking().Count(x => x.ABGRADE.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE016:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE017:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE018:
                    if (entities.MA009.AsNoTracking().Count(x => x.MJEXRTT.Trim().Equals(srCode)) > 0)
                        messageReturn = StaticResources.ItemMenuMaster.ExchangeRateMaster;
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE019:
                    if (entities.PUR001.AsNoTracking().Count(x => x.AAPRICE.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE020:
                    if (entities.PUR001.AsNoTracking().Count(x => x.AASETRM.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE021:
                    if (entities.PUR001.AsNoTracking().Count(x => x.AAPTTRM.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE022:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE023:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE024:
                    if (entities.PUR001.AsNoTracking().Count(x => x.AADLVCD.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE025:
                    if (entities.PUR001.AsNoTracking().Count(x => x.AACTRTP.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE026:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE027:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE028:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE029:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE030:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE031:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE032:
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE033:
                    if (entities.MA002.AsNoTracking().Count(x => x.MBWTCAL.Trim().Equals(srCode)) > 0)
                        messageReturn = "User Master";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE034:
                    if (entities.PUR001.AsNoTracking().Count(x => x.AARMTP.Trim().Equals(srCode)) > 0)
                        messageReturn = "Purchase Contract";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE035:
                    if (entities.MA004.AsNoTracking().Count(x => x.MDWRCTG.Trim().Equals(srCode)) > 0)
                        messageReturn = "Location Master";
                    break;
                   
            }

            return messageReturn;
        }
        public static string CheckConstraintSteelGrade(string grade)
        {
            string messageReturn = String.Empty;
            if (entities.PUR001.AsNoTracking().Count(x => x.RAPSTLGR.Trim().Equals(grade)) > 0)
            {
                messageReturn = "Purchase Contract";
            }
            return messageReturn;
        }

    }
}