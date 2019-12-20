using System.Collections.Generic;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Extension.AccountRole
{
    public partial class StandardPermissionProvider
    {  //action category main menu
        public static readonly PermissionRecord Master = new PermissionRecord { Name = "Master area", SystemName = "Master", Category = "Area" };
        public static readonly PermissionRecord Purchase = new PermissionRecord { Name = "Purchase area", SystemName = "Purchase", Category = "Area" };
        public static readonly PermissionRecord GetRawMaterialWarehousingEntry_PO = new PermissionRecord { Name = "Raw Material area", SystemName = "AccessRawMaterial", Category = "Area" };
        public static readonly PermissionRecord Sale = new PermissionRecord { Name = "Sales area", SystemName = "AccessSales", Category = "Area" };
        public static readonly PermissionRecord Account = new PermissionRecord { Name = "Account area", SystemName = "AccessAccount", Category = "Area" };
        public static readonly PermissionRecord Production = new PermissionRecord { Name = "Production area", SystemName = "AccessProduction", Category = "Area" };
        public static readonly PermissionRecord Packing = new PermissionRecord { Name = "Packing area", SystemName = "AccessPacking", Category = "Area" };
        public static readonly PermissionRecord Delivery = new PermissionRecord { Name = "Delivery area", SystemName = "AccessDelivery", Category = "Area" };
        public static readonly PermissionRecord Inventory = new PermissionRecord { Name = "Inventory area", SystemName = "AccessInventory", Category = "Area" };
        public static readonly PermissionRecord Inspection = new PermissionRecord { Name = "Inspection area", SystemName = "AccessInspection", Category = "Area" };
        public static readonly PermissionRecord Planning = new PermissionRecord { Name = "Planning area", SystemName = "AccessPlanning", Category = "Area" };
        public static readonly PermissionRecord SalePurchaseMaster = new PermissionRecord { Name = "Master area. SalePurchaseMaster", SystemName = "SalePurchaseMaster", Category = "Master" };
        public static readonly PermissionRecord UserMaster = new PermissionRecord { Name = "Master area. Manage UserMaster", SystemName = "UserMaster", Category = "Master" };
        public static readonly PermissionRecord UserIdMaster = new PermissionRecord { Name = "Master area. Manage UserIdMaster", SystemName = "UserIdMaster", Category = "Master" };
        public static readonly PermissionRecord LocationMaster = new PermissionRecord { Name = "Master area. Manage LocationMaster", SystemName = "LocationMaster", Category = "Master" };
        public static readonly PermissionRecord CoatingMaster = new PermissionRecord { Name = "Master area. Manage CoatingMaster", SystemName = "CoatingMaster", Category = "Master" };
        public static readonly PermissionRecord SpecMaster = new PermissionRecord { Name = "Master area. Manage SpecMaster", SystemName = "SpecMaster", Category = "Master" };
        public static readonly PermissionRecord ClosingDateMaster = new PermissionRecord { Name = "Master area. Manage ClosingDateMaster", SystemName = "ClosingDateMaster", Category = "Master" };
        public static readonly PermissionRecord ProductMaster = new PermissionRecord { Name = "Master area. Manage ProductMaster", SystemName = "ProductMaster", Category = "Master" };
        public static readonly PermissionRecord ExchangeRateMaster = new PermissionRecord { Name = "Master area. Manage ExchangeRateMaster", SystemName = "ExchangeRateMaster", Category = "Master" };
        public static readonly PermissionRecord TaxMaster = new PermissionRecord { Name = "Master area. Manage TaxMaster", SystemName = "TaxMaster", Category = "Master" };
        public static readonly PermissionRecord CreditMaster = new PermissionRecord { Name = "Master area. Manage CreditMaster", SystemName = "CreditMaster", Category = "Master" };
        public static readonly PermissionRecord SteelGradeMaster = new PermissionRecord { Name = "Master area. Manage SteelGradeMaster", SystemName = "SteelGradeMaster", Category = "Master" };
        public static readonly PermissionRecord DueDateTypeMaster = new PermissionRecord { Name = "Master area. Manage DueDateTypeMaster", SystemName = "DueDateTypeMaster", Category = "Master" };
        public static readonly PermissionRecord DaysInMonthMaster = new PermissionRecord { Name = "Master area. Manage DaysInMonthMaster", SystemName = "DaysInMonthMaster", Category = "Master" };
        public static readonly PermissionRecord CalculationTypeMaster = new PermissionRecord { Name = "Master area. Manage CalculationTypeMaster", SystemName = "CalculationTypeMaster", Category = "Master" };
        public static readonly PermissionRecord WeightDisplayCodeMaster = new PermissionRecord { Name = "Master area. Manage WeightDisplayCodeMaster", SystemName = "WeightDisplayCodeMaster", Category = "Master" };
        public static readonly PermissionRecord MakerCodeMaster = new PermissionRecord { Name = "Master area. Manage MakerCodeMaster", SystemName = "MakerCodeMaster", Category = "Master" };
        public static readonly PermissionRecord CommodityCodeMaster = new PermissionRecord { Name = "Master area. Manage CommodityCodeMaster", SystemName = "CommodityCodeMaster", Category = "Master" };
        public static readonly PermissionRecord StatusCodeMaster = new PermissionRecord { Name = "Master area. Manage StatusCodeMaster", SystemName = "StatusCodeMaster", Category = "Master" };
        public static readonly PermissionRecord SectionCodeMaster = new PermissionRecord { Name = "Master area. Manage SectionCodeMaster", SystemName = "SectionCodeMaster", Category = "Master" };
        public static readonly PermissionRecord DamageCodeMaster = new PermissionRecord { Name = "Master area. Manage DamageCodeMaster", SystemName = "DamageCodeMaster", Category = "Master" };
        public static readonly PermissionRecord CountryCodeMaster = new PermissionRecord { Name = "Master area. Manage CountryCodeMaster", SystemName = "CountryCodeMaster", Category = "Master" };
        public static readonly PermissionRecord BusinessTypeCodeMaster = new PermissionRecord { Name = "Master area. Manage BusinessTypeCodeMaster", SystemName = "BusinessTypeCodeMaster", Category = "Master" };
        public static readonly PermissionRecord CurrencyCodeMaster = new PermissionRecord { Name = "Master area. Manage CurrencyCodeMaster", SystemName = "CurrencyCodeMaster", Category = "Master" };
        public static readonly PermissionRecord ShiftCodeMaster = new PermissionRecord { Name = "Master area. Manage ShiftCodeMaster", SystemName = "ShiftCodeMaster", Category = "Master" };
        public static readonly PermissionRecord MachineNoMaster = new PermissionRecord { Name = "Master area. Manage MachineNoMaster", SystemName = "MachineNoMaster", Category = "Master" };
        public static readonly PermissionRecord GradeMaster = new PermissionRecord { Name = "Master area. Manage GradeMaster", SystemName = "GradeMaster", Category = "Master" };
        public static readonly PermissionRecord ReasonForChangingMaster = new PermissionRecord { Name = "Master area. Manage ReasonForChangingMaster", SystemName = "ReasonForChangingMaster", Category = "Master" };
        public static readonly PermissionRecord BankCodeMaster = new PermissionRecord { Name = "Master area. Manage BankCodeMaster", SystemName = "BankCodeMaster", Category = "Master" };
        public static readonly PermissionRecord ExchangeRateTypeMaster = new PermissionRecord { Name = "Master area. Manage ExchangeRateTypeMaster", SystemName = "ExchangeRateTypeMaster", Category = "Master" };
        public static readonly PermissionRecord PriceTermMaster = new PermissionRecord { Name = "Master area. Manage PriceTermMaster", SystemName = "PriceTermMaster", Category = "Master" };
        public static readonly PermissionRecord SettlementTermMaster = new PermissionRecord { Name = "Master area. Manage SettlementTermMaster", SystemName = "SettlementTermMaster", Category = "Master" };
        public static readonly PermissionRecord TypeofTermsMaster = new PermissionRecord { Name = "Master area. Manage TypeofTermsMaster", SystemName = "TypeofTermsMaster", Category = "Master" };
        public static readonly PermissionRecord PackingTypeMaster = new PermissionRecord { Name = "Master area. Manage PackingTypeMaster", SystemName = "PackingTypeMaster", Category = "Master" };
        public static readonly PermissionRecord InterruptedReasonCodeMaster = new PermissionRecord { Name = "Master area. Manage InterruptedReasonCodeMaster", SystemName = "InterruptedReasonCodeMaster", Category = "Master" };
        public static readonly PermissionRecord DeliveryConditionMaster = new PermissionRecord { Name = "Master area. Manage DeliveryConditionMaster", SystemName = "DeliveryConditionMaster", Category = "Master" };
        public static readonly PermissionRecord ContractTypeMaster = new PermissionRecord { Name = "Master area. Manage ContractTypeMaster", SystemName = "ContractTypeMaster", Category = "Master" };
        public static readonly PermissionRecord TradeCategoryMaster = new PermissionRecord { Name = "Master area. Manage TradeCategoryMaster", SystemName = "TradeCategoryMaster", Category = "Master" };
        public static readonly PermissionRecord InventoryStatusMaster = new PermissionRecord { Name = "Master area. Manage InventoryStatusMaster", SystemName = "InventoryStatusMaster", Category = "Master" };
        public static readonly PermissionRecord InventoryTypeMaster = new PermissionRecord { Name = "Master area. Manage InventoryTypeMaster", SystemName = "InventoryTypeMaster", Category = "Master" };
        public static readonly PermissionRecord MoneyTypeMaster = new PermissionRecord { Name = "Master area. Manage MoneyTypeMaster", SystemName = "MoneyTypeMaster", Category = "Master" };
        public static readonly PermissionRecord UnitPriceUnitMaster = new PermissionRecord { Name = "Master area. Manage UnitPriceUnitMaster", SystemName = "UnitPriceUnitMaster", Category = "Master" };
        public static readonly PermissionRecord LogTypeMaster = new PermissionRecord { Name = "Master area. Manage LogTypeMaster", SystemName = "LogTypeMaster", Category = "Master" };
        public static readonly PermissionRecord DataTypeMaster = new PermissionRecord { Name = "Master area. Manage DataTypeMaster", SystemName = "DataTypeMaster", Category = "Master" };
        public static readonly PermissionRecord WeightCalculationCodeMaster = new PermissionRecord { Name = "Master area. Manage WeightCalculationCodeMaster", SystemName = "WeightCalculationCodeMaster", Category = "Master" };
        public static readonly PermissionRecord RawMaterialTypeMaster = new PermissionRecord { Name = "Master area. Manage RawMaterialTypeMaster", SystemName = "RawMaterialTypeMaster", Category = "Master" };
        //purchase contract
        public static readonly PermissionRecord PurchaseOrderEntry = new PermissionRecord { Name = "Manage Purchase Order Entry", SystemName = "PurchaseOrderEntry", Category = "Purchase" };
        public static readonly PermissionRecord PurchaseOrderBalanceEnquiry = new PermissionRecord { Name = "Manage Purchase OrderBalance Enquiry", SystemName = "PurchaseOrderBalanceEnquiry", Category = "Purchase" };
        public static readonly PermissionRecord PurchaseOrderCompletion = new PermissionRecord { Name = "Manage Purchase Order Completion", SystemName = "PurchaseOrderCompletion", Category = "Purchase" };
        public static readonly PermissionRecord PurchaseHistoryEnquiry = new PermissionRecord { Name = "Manage Purchase History Enquiry", SystemName = "PurchaseHistoryEnquiry", Category = "Purchase" };
        public static readonly PermissionRecord PrintPurchaseOrder = new PermissionRecord { Name = "Manage Print Purchase Order", SystemName = "PrintPurchaseOrder", Category = "Purchase" };

        //action




        public static readonly PermissionAction Add = new PermissionAction { Name = "Add", Action = "Add" };
        public static readonly PermissionAction Edit = new PermissionAction { Name = "Edit", Action = "Edit" };
        public static readonly PermissionAction Delete = new PermissionAction { Name = "Delete", Action = "Delete" };
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                Master,
                SalePurchaseMaster,
                UserMaster,
                UserIdMaster,
                LocationMaster,
                CoatingMaster,
                SpecMaster,
                ClosingDateMaster,
                ProductMaster,
                ExchangeRateMaster,
                TaxMaster,
                CreditMaster,
                DueDateTypeMaster,
                DaysInMonthMaster,
                CalculationTypeMaster,
                WeightDisplayCodeMaster,
                MakerCodeMaster,
                CommodityCodeMaster,
                StatusCodeMaster,
                SectionCodeMaster,
                DamageCodeMaster,
                CountryCodeMaster,
                BusinessTypeCodeMaster,
                CurrencyCodeMaster,
                ShiftCodeMaster,
                MachineNoMaster,
                GradeMaster,
                ReasonForChangingMaster,
                BankCodeMaster,
                ExchangeRateTypeMaster,
                PriceTermMaster,
                SettlementTermMaster,
                TypeofTermsMaster,
                PackingTypeMaster,
                InterruptedReasonCodeMaster,
                DeliveryConditionMaster,
                ContractTypeMaster,
                TradeCategoryMaster,
                InventoryStatusMaster,
                InventoryTypeMaster,
                MoneyTypeMaster,
                UnitPriceUnitMaster,
                LogTypeMaster,
                DataTypeMaster    ,
                WeightCalculationCodeMaster,
                RawMaterialTypeMaster
            };
        }
        public virtual IEnumerable<PermissionAction> GetActionPermissions()
        {
            return new[]
            {
              Add,
              Edit,
              Delete
            };
        }
    }
}