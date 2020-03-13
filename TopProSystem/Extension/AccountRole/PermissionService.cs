using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopProSystem.Areas.MasterSetting.Models;
using TopProSystem.Extension.ModelShowAction;

namespace TopProSystem.Extension.AccountRole
{
    public partial class PermissionService
    {

        public bool Authorize(PermissionRecord permission, PermissionAction permissionAction)
        {
            if (permission == null || permissionAction == null || HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID] == null)
                return false;
            try
            {
                TopProSystemEntities db = new TopProSystemEntities();

                var session = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();

                var rolebyma003 = db.MA003.FirstOrDefault(x => x.MCIDCD == session).MCSCTLV;
                var permission_Id = db.PermissionRecords.Where(x => x.SystemName == permission.SystemName).First().Id;
                var permissionAction_Id = db.PermissionActions.Where(x => x.Name == permissionAction.Name).First().Id;
                var mapping = db.Role_Mapping.Where(x => x.CustomerRole_Id == rolebyma003 && x.PermissionRecord_Id == permission_Id).FirstOrDefault();
                var mappingAction = db.Role_Mapping_Action.Where(x => x.CustomerRole_Id == rolebyma003 && x.PermissionRecord_Id == permission_Id && x.Action_Id == permissionAction_Id).FirstOrDefault();
                if (mapping != null && mappingAction != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }


        }
        public bool Authorize(PermissionRecord permission)
        {
            if (permission == null || HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID] == null)
                return false;
            try
            {
                TopProSystemEntities db = new TopProSystemEntities();

                var session = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
                var rolebyma003 = db.MA003.FirstOrDefault(x => x.MCIDCD == session).MCSCTLV;
                var permission_Id = db.PermissionRecords.Where(x => x.SystemName == permission.SystemName).First().Id;
                var mapping = db.Role_Mapping.Where(x => x.CustomerRole_Id == rolebyma003 && x.PermissionRecord_Id == permission_Id).Count() > 0;
                if (mapping == true)
                    return true;
                else
                    return false;

            }
            catch
            {
                return false;
            }

        }
        public virtual bool Authorize(string permissionRecordSystemName)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName) || HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID] == null)
                return false;

            try
            {
                TopProSystemEntities db = new TopProSystemEntities();


                var session = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
                var rolebyma003 = db.MA003.FirstOrDefault(x => x.MCIDCD == session).MCSCTLV;

                var permission_Id = db.PermissionRecords.Where(x => x.SystemName == permissionRecordSystemName).First().Id;

                var mapping = db.Role_Mapping.Where(x => x.CustomerRole_Id == rolebyma003 && x.PermissionRecord_Id == permission_Id).FirstOrDefault();
                if (mapping != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }


        }
        public virtual bool AuthorizeMA012(string classifiCationCode, PermissionAction permissionAction)
        {
            if (permissionAction == null || String.IsNullOrEmpty(classifiCationCode) || permissionAction == null || HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID] == null)
                return false;
            TopProSystemEntities db = new TopProSystemEntities();
            string permissionSystemName = null;
            switch (classifiCationCode)
            {
                case ClassificationCode.CLASSIFICATTIONCODE001:
                    permissionSystemName = "DueDateTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE002:
                    permissionSystemName = "DaysInMonthMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE003:
                    permissionSystemName = "CalculationTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE004:
                    permissionSystemName = "WeightDisplayCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE005:
                    permissionSystemName = "MakerCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE006:
                    permissionSystemName = "CommodityCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE007:
                    permissionSystemName = "StatusCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE008:
                    permissionSystemName = "SectionCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE009:
                    permissionSystemName = "DamageCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE010:
                    permissionSystemName = "CountryCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE011:
                    permissionSystemName = "BusinessTypeCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE012:
                    permissionSystemName = "CurrencyCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE013:
                    permissionSystemName = "ShiftCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE014:
                    permissionSystemName = "MachineNoMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE015:
                    permissionSystemName = "GradeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE016:
                    permissionSystemName = "ReasonForChangingMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE017:
                    permissionSystemName = "BankCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE018:
                    permissionSystemName = "ExchangeRateTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE019:
                    permissionSystemName = "PriceTermMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE020:
                    permissionSystemName = "SettlementTermMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE021:
                    permissionSystemName = "TypeofTermsMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE022:
                    permissionSystemName = "PackingTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE023:
                    permissionSystemName = "InterruptedReasonCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE024:
                    permissionSystemName = "DeliveryConditionMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE025:
                    permissionSystemName = "ContractTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE026:
                    permissionSystemName = "TradeCategoryMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE027:
                    permissionSystemName = "InventoryStatusMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE028:
                    permissionSystemName = "InventoryTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE029:
                    permissionSystemName = "MoneyTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE030:
                    permissionSystemName = "UnitPriceUnitMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE031:
                    permissionSystemName = "LogTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE032:
                    permissionSystemName = "DataTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE033:
                    permissionSystemName = "WeightCalculationCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE034:
                    permissionSystemName = "RawMaterialTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE035:
                    permissionSystemName = "WarehouseCategoryCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE036:
                    permissionSystemName = "RawMaterialLabelTypeMaster";
                    break;
            }

            var session = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
            var rolebyma003 = db.MA003.FirstOrDefault(x => x.MCIDCD == session).MCSCTLV;
            var role = db.SecurityLevels.First(x => x.Id == rolebyma003).Name;
            var permission_Id = db.PermissionRecords.Where(x => x.SystemName == permissionSystemName).First().Id;
            var permissionAction_Id = db.PermissionActions.Where(x => x.Name == permissionAction.Name).First().Id;
            var role_Id = db.SecurityLevels.Where(x => x.Name == role).First().Id;
            var mapping = db.Role_Mapping.Where(x => x.CustomerRole_Id == role_Id && x.PermissionRecord_Id == permission_Id).FirstOrDefault();
            var mappingAction = db.Role_Mapping_Action.Where(x => x.CustomerRole_Id == role_Id && x.PermissionRecord_Id == permission_Id && x.Action_Id == permissionAction_Id).FirstOrDefault();
            if (mapping != null && mappingAction != null)
                return true;
            else
                return false;
        }
        public virtual bool AuthorizeMA012(string classifiCationCode)
        {
            if (string.IsNullOrEmpty(classifiCationCode) || HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID] == null)
                return false;

            TopProSystemEntities db = new TopProSystemEntities();
            string permissionSystemName = null;
            switch (classifiCationCode)
            {
                case ClassificationCode.CLASSIFICATTIONCODE001:
                    permissionSystemName = "DueDateTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE002:
                    permissionSystemName = "DaysInMonthMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE003:
                    permissionSystemName = "CalculationTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE004:
                    permissionSystemName = "WeightDisplayCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE005:
                    permissionSystemName = "MakerCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE006:
                    permissionSystemName = "CommodityCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE007:
                    permissionSystemName = "StatusCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE008:
                    permissionSystemName = "SectionCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE009:
                    permissionSystemName = "DamageCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE010:
                    permissionSystemName = "CountryCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE011:
                    permissionSystemName = "BusinessTypeCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE012:
                    permissionSystemName = "CurrencyCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE013:
                    permissionSystemName = "ShiftCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE014:
                    permissionSystemName = "MachineNoMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE015:
                    permissionSystemName = "GradeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE016:
                    permissionSystemName = "ReasonForChangingMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE017:
                    permissionSystemName = "BankCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE018:
                    permissionSystemName = "ExchangeRateTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE019:
                    permissionSystemName = "PriceTermMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE020:
                    permissionSystemName = "SettlementTermMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE021:
                    permissionSystemName = "TypeofTermsMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE022:
                    permissionSystemName = "PackingTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE023:
                    permissionSystemName = "InterruptedReasonCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE024:
                    permissionSystemName = "DeliveryConditionMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE025:
                    permissionSystemName = "ContractTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE026:
                    permissionSystemName = "TradeCategoryMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE027:
                    permissionSystemName = "InventoryStatusMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE028:
                    permissionSystemName = "InventoryTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE029:
                    permissionSystemName = "MoneyTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE030:
                    permissionSystemName = "UnitPriceUnitMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE031:
                    permissionSystemName = "LogTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE032:
                    permissionSystemName = "DataTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE033:
                    permissionSystemName = "WeightCalculationCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE034:
                    permissionSystemName = "RawMaterialTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE035:
                    permissionSystemName = "WarehouseCategoryCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE036:
                    permissionSystemName = "RawMaterialLabelTypeMaster";
                    break;
            }
            var session = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
            var rolebyma003 = db.MA003.FirstOrDefault(x => x.MCIDCD == session).MCSCTLV;
            var role = db.SecurityLevels.First(x => x.Id == rolebyma003).Name;
            var permission_Id = db.PermissionRecords.Where(x => x.SystemName == permissionSystemName).First().Id;
            var role_Id = db.SecurityLevels.Where(x => x.Name == role).First().Id;
            var mapping = db.Role_Mapping.Where(x => x.CustomerRole_Id == role_Id && x.PermissionRecord_Id == permission_Id).FirstOrDefault();
            if (mapping != null)
                return true;
            else
                return false;
        }

        public BaseActionModel AuthorizeAction(PermissionRecord permission)
        {
            TopProSystemEntities db = new TopProSystemEntities();
            var session = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
            var rolebyma003 = db.MA003.FirstOrDefault(x => x.MCIDCD == session).MCSCTLV;
            var role = db.SecurityLevels.First(x => x.Id == rolebyma003).Name;
            var permission_Id = db.PermissionRecords.Where(x => x.SystemName == permission.SystemName).First().Id;
            var role_Id = db.SecurityLevels.Where(x => x.Name == role).First().Id;
            var model = new BaseActionModel();
            bool add = db.Role_Mapping_Action.Where(x => x.CustomerRole_Id == role_Id && x.PermissionRecord_Id == permission_Id && x.Action_Id == 1).Count() > 0;
            bool edit = db.Role_Mapping_Action.Where(x => x.CustomerRole_Id == role_Id && x.PermissionRecord_Id == permission_Id && x.Action_Id == 2).Count() > 0;
            bool delete = db.Role_Mapping_Action.Where(x => x.CustomerRole_Id == role_Id && x.PermissionRecord_Id == permission_Id && x.Action_Id == 3).Count() > 0;
            if (add)
                model.ShowAdd = true;
            if (edit)
                model.ShowEdit = true;
            if (delete)
                model.ShowDelete = true;
            return model;
        }



        public virtual IList<PermissionRecord> GetAllPermissionRecords()
        {
            TopProSystemEntities db = new TopProSystemEntities();
            var query = from pr in db.PermissionRecords
                        orderby pr.Name
                        select pr;
            var permissions = query.ToList();
            return permissions;
        }


    }
}