using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Areas.MasterSetting.DAL.INV001
{
    public class INV001_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        public IEnumerable<Models.INV001> GetINV001byPurchaseNoandItemNo(string PurchaseContractNo, double? ItemNo)
        {
            using (var dc = new TopProSystemEntities())
            {
                var inv001 = dc.INV001.Where(x => x.CACTRNO == PurchaseContractNo && x.CACTITM == ItemNo).OrderBy(x=>x.CARGSDT).ToList();
                return inv001;
            }

        }

        public Models.INV001 GetINV001byPurchaseNoandItemNoSearch(string PurchaseContractNo, double? ItemNo)
        {
            var inv001 = db.INV001.Where(x => x.CACTRNO == PurchaseContractNo && x.CACTITM == ItemNo).FirstOrDefault();
            return inv001;
        }
        public int GetTotalRecordRegisterPurchaseAmount(string purchaseContractNo, double? ItemNo)
        {
            var a = db.INV001.Where(x => x.CACTRNO == purchaseContractNo && x.CACTITM == ItemNo).ToList();
            return a.Count;
        }

        public IEnumerable<Models.INV001> GetTotalDisplayRecordRegisterPurchaseAmount(string purchaseContractNo, double? ItemNo, int skip, int take)
        {
            var list =GetINV001byPurchaseNoandItemNo(purchaseContractNo,ItemNo).Skip(skip).Take(take).ToList();
            return list;
        }
        public double? sumQty(string purchaseContractNo, double? ItemNo)
        {
            double? sumqty = db.INV001.Where(x => x.CACTRNO == purchaseContractNo && x.CACTITM == ItemNo).Sum(x => x.CAQTY);
            return sumqty;
        }
        public double? sumWeight(string purchaseContractNo, double? ItemNo)
        {
            double? sumWeight = db.INV001.Where(x => x.CACTRNO == purchaseContractNo && x.CACTITM == ItemNo).Sum(x => x.CAWT);
            return sumWeight;
        }

        // Summary:
        //    Search from INV001 by two parameter {shipper invoice no & Order no}
        //    
        //
        // Parameters:
        //   sinvoiceNo:
        //     a string
        //   orderNo:
        //     a string
        public int GetTotalRecordByshipperInvoiceAndOrderNo(string sinvoiceNo, string orderNo)
        {
            if (string.IsNullOrEmpty(sinvoiceNo) && string.IsNullOrEmpty(orderNo))
            {
                return 0;
            }
            else
            {
                using (var dc = new TopProSystemEntities())
                {
                    IQueryable<Models.INV001> data = dc.INV001;
                    if (!string.IsNullOrEmpty(sinvoiceNo))
                    {
                        data = data.Where(x => x.CAPIVNO.Equals(sinvoiceNo));
                    }
                    if (!string.IsNullOrEmpty(orderNo))
                    {
                        data = data.Where(x => x.CAORDNO.Equals(orderNo));
                    }
                    return data.Count();
                }

            }
        }

        public IEnumerable<Models.INV001> GetTotalDisplayRecordByshipperInvoiceAndOrderNo(string sinvoiceNo, string orderNo, int skip = 0, int take = 0, bool paging = true)
        {
            if (string.IsNullOrEmpty(sinvoiceNo) && string.IsNullOrEmpty(orderNo))
            {
                return new List<Models.INV001>();
            }
            else
            {
                IQueryable<Models.INV001> data = db.INV001;
                if (!string.IsNullOrEmpty(sinvoiceNo))
                {
                    data = data.Where(x => x.CAPIVNO.Trim().Equals(sinvoiceNo));
                }
                if (!string.IsNullOrEmpty(orderNo))
                {
                    data = data.Where(x => x.CAORDNO.Trim().Equals(orderNo));
                }
                if (paging)
                {
                    return data.OrderBy(x => x.CARGSDT).Skip(skip).Take(take);
                }
                else
                {
                    return data.OrderBy(x => x.ID);
                }
            }
        }
        public double GetTotalWeightOfInventoryByInvoiceNoAndOrderNo(string sinvoiceNo, string orderNo)
        {
            IQueryable<Models.INV001> data = db.INV001;
            if (!string.IsNullOrEmpty(sinvoiceNo))
            {
                data = data.Where(x => x.CAPIVNO.Trim().Equals(sinvoiceNo));
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                data = data.Where(x => x.CAORDNO.Trim().Equals(orderNo));
            }
            if (data.Count() == 0)
            {
                return 0;
            }
            return (double)data.Sum(x => x.CAWT);

        }

        public bool UpdateRegisterTariffAmount(string shipper_invoice_no, string order_no, decimal total_tariff_amount)
        {
            try
            {
                using (var dc = new TopProSystemEntities())
                {
                    var sqlParam = new SqlParameter[]
                    {
                        new SqlParameter("@SHIPPER_INVOICE_NO",String.IsNullOrEmpty(shipper_invoice_no)? (object)DBNull.Value: shipper_invoice_no),
                        new SqlParameter("@ORDER_NO",String.IsNullOrEmpty(order_no)? (object)DBNull.Value: order_no),
                        new SqlParameter("@TOTAL_TARIFF_AMOUNT",total_tariff_amount),
                    };

                    var rows = dc.Database.ExecuteSqlCommand("EXEC UPDATE_NEW_TARIFF_AMOUNT @SHIPPER_INVOICE_NO,@ORDER_NO,@TOTAL_TARIFF_AMOUNT", sqlParam);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteLogError_DAL writeLogError_DAL = new WriteLogError_DAL();
                writeLogError_DAL.WriteLogErrorException(ex);
                return false;
            }
        }

        public bool UpdateCustomHandlingAmount(string shipper_invoice_no, string order_no, decimal total_handling_amount)
        {
            try
            {
                using (var dc = new TopProSystemEntities())
                {
                    var sqlParam = new SqlParameter[]
                    {
                        new SqlParameter("@SHIPPER_INVOICE_NO",String.IsNullOrEmpty(shipper_invoice_no)? (object)DBNull.Value: shipper_invoice_no),
                        new SqlParameter("@ORDER_NO",String.IsNullOrEmpty(order_no)? (object)DBNull.Value: order_no),
                        new SqlParameter("@TOTAL_HANDLING_AMOUNT",total_handling_amount),
                    };
                    var rows = dc.Database.ExecuteSqlCommand("EXEC UPDATE_CUSTOM_HANDLING_AMOUNT @SHIPPER_INVOICE_NO,@ORDER_NO,@TOTAL_HANDLING_AMOUNT", sqlParam);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteLogError_DAL writeLogError_DAL = new WriteLogError_DAL();
                writeLogError_DAL.WriteLogErrorException(ex);
                return false;
            }

        }

        public bool UpdateFreightAmount(string shipper_invoice_no, string order_no, decimal total_freight_amount)
        {
            try
            {
                using (var dc = new TopProSystemEntities())
                {
                    var sqlParam = new SqlParameter[]
                    {
                        new SqlParameter("@SHIPPER_INVOICE_NO",String.IsNullOrEmpty(shipper_invoice_no)? (object)DBNull.Value: shipper_invoice_no),
                        new SqlParameter("@ORDER_NO",String.IsNullOrEmpty(order_no)? (object)DBNull.Value: order_no),
                        new SqlParameter("@TOTAL_FREIGHT_AMOUNT",total_freight_amount),
                    };
                    var rows = dc.Database.ExecuteSqlCommand("EXEC UPDATE_FREIGHT_AMOUNT @SHIPPER_INVOICE_NO,@ORDER_NO,@TOTAL_FREIGHT_AMOUNT", sqlParam);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteLogError_DAL writeLogError_DAL = new WriteLogError_DAL();
                writeLogError_DAL.WriteLogErrorException(ex);
                return false;
            }

        }
        public bool UpdateRegisterPurchaseAmount(string purchaseNo, string ItemNo, decimal unitPrice) //pnb update04022020
        {
            try
            {
                using (var dc = new TopProSystemEntities())
                {
                    var sqlParam = new SqlParameter[]
                    {
                        new SqlParameter("@PURCHASE_NO",String.IsNullOrEmpty(purchaseNo)? (object)DBNull.Value: purchaseNo),
                        new SqlParameter("@ITEM_NO",String.IsNullOrEmpty(ItemNo)? (object)DBNull.Value: ItemNo),
                        new SqlParameter("@PURCHASE_PRICE",unitPrice),
                    };

                    var rows = dc.Database.ExecuteSqlCommand("EXEC UPDATE_REGISTER_PURCHASE_AMOUNT @PURCHASE_NO,@ITEM_NO,@PURCHASE_PRICE", sqlParam);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteLogError_DAL writeLogError_DAL = new WriteLogError_DAL();
                writeLogError_DAL.WriteLogErrorException(ex);
                return false;
            }
        }

        private TopProSystemEntities dc = new TopProSystemEntities();
        WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();

        protected IQueryable<Models.INV001> INV001s()
        {
            return dc.INV001;
        }
        #region getStockEntryDateVESSL

        public IEnumerable<Models.INV001> GetTotalDisplayRecordByStockEntryDateAndVesselName(DateTime? StockEntryDate, string VesselName, string Status = "1", int skip = 0, int take = 0, bool paging = true)
        {
            if (StockEntryDate != null && string.IsNullOrEmpty(VesselName))
            {
                return new List<Models.INV001>();
            }
            else
            {
                IQueryable<Models.INV001> data = db.INV001;
             
                if (paging)
                {
                    data = data.Where(x => x.CASEDT == StockEntryDate && x.CAVESEL.Trim() == VesselName && x.CAINVST == Status).OrderBy(x => x.CARGSDT).Skip(skip).Take(take);
                    return data;
                }
                else
                {
                    data = data.Where(x => x.CASEDT == StockEntryDate && x.CAVESEL.Trim() == VesselName && x.CAINVST == Status).OrderBy(x => x.ID);
                    return data;
                }
            }
        }
        public int GetTotalRecordByStockEntryDateAndVesselName(DateTime? StockEntryDate, string VesselName, string Status = "1")
        {
          
                using (var dc = new TopProSystemEntities())
                {
                
                  var countdata  = dc.INV001.Where(x => x.CASEDT == StockEntryDate && x.CAVESEL.Trim() == VesselName && x.CAINVST == Status).Count();
                    return countdata;
                }

            
        }
     
        public double? totalQtybyStockandVessl(DateTime? StockEntryDate, string VesselName, string Status = "1")
        {
            using (var dc = new TopProSystemEntities())
            {
                var countdata = dc.INV001.Where(x => x.CASEDT == StockEntryDate && x.CAVESEL.Trim() == VesselName && x.CAINVST == Status).Sum(x=>x.CAQTY);
                return countdata;
            }
        }
        public double? totalWtbyStockandVessl(DateTime? StockEntryDate, string VesselName, string Status = "1")
        {
            using (var dc = new TopProSystemEntities())
            {
                var countdata =dc.INV001.Where(x => x.CASEDT == StockEntryDate && x.CAVESEL.Trim() == VesselName && x.CAINVST == Status).Sum(x => x.CAWT);

             
                return countdata;
            }
        }
        #endregion
        public bool Insert(Models.INV001 model)
        {
            try
            {
                var modelChecking = INV001s().FirstOrDefault(x => x.CAINVNO.Trim().Equals(model.CAINVNO));
                if (modelChecking != null)
                {
                    if (!modelChecking.CADSTNC.Trim().Equals(model.CADSTNC.Trim())) // double click case
                    {
                        return true;
                    }
                }

                //model.PURCODE = CreateGuid();
                //model.Deleted = 0;
                dc.INV001.Add(model);
                if (dc.SaveChanges() > 0)
                {
                    WriteLogError_DAL.WriteLogUserAction("INV001", "INSERT", model.CAINVNO);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                WriteLogError_DAL.WriteLogErrorException(ex);
                return false;
            }
        }

        public Models.INV001 GetReferences(Models.INV001 _model)
        {
            if (_model == null) _model = new Models.INV001();


            //_model.Grades = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE015)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.MakerCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE005)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.CurrencyCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE012)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.Priceterms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE019)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();    //pnb edit
            //_model.TypeOfTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE021)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();    //pnb edit
            //_model.CommodityCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE006)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.ContractTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE025)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.SettelementTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE020)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList(); //pnb edit
            //_model.ExchangeRateType = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE018)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.RawMaterialTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE034)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRNM }).ToList();
            //_model.TaxCodes = dc.MA010.OrderByDescending(x => x.MKRGSDT).ToList().ConvertAll(x =>
            //{
            //    return new SelectListItem()
            //    {
            //        Value = x.MKTXCD,
            //        Text = x.MKTXCD.ToString()
            //    };
            //});

            //_model.ExchangeRates = dc.MA009.OrderByDescending(x => x.MJRGSDT).ToList().ConvertAll(x =>
            //{
            //    return new SelectListItem()
            //    {
            //        Value = x.MJCRRCD,
            //        Text = x.MJEXRTT
            //    };
            //});
            //_model.Specs = dc.MA006.OrderByDescending(x => x.MFRGSDT).ToList().ConvertAll(x =>
            //{
            //    return new SelectListItem()
            //    {
            //        Value = x.MFPRDSP,
            //        Text = x.MFPRDSP
            //    };
            //});

            //_model.Coatings = dc.MA005.OrderByDescending(x => x.MERGSDT).Select(x => new SelectListItem { Value = x.MECOAT, Text = x.MECOAT });
            //_model.UserCodes = dc.MA002.Select(x => new SelectListItem { Value = x.MBUSRCD, Text = x.MBUSRCD }).ToList();

            //_model.SteelGrades = dc.SteelGrades.Select(x => new SelectListItem { Value = x.Grade, Text = x.Grade });
            //_model.PersonIncharges = dc.MA003.OrderByDescending(x => x.MCRGSDT).Select(x => new SelectListItem { Value = x.MCIDCD, Text = x.MCIDCD });
            //_model.SupplierCodes = dc.MA001.OrderByDescending(x => x.MARGSDT).Where(x => x.MASPCTG.Trim().Equals("P")).Select(x => new SelectListItem { Value = x.MASPCD, Text = x.MASPCD });
            //_model.DeliveryConditionCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE024)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //return _model;

            /**********bao edit*************/

            // _model.Grades = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE015)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.MakerCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE005)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.CurrencyCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE012)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.Priceterms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE019)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();    //pnb edit
            //_model.TypeOfTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE021)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();    //pnb edit
            // _model.CommodityCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE006)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.ContractTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE025)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.SettelementTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE020)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList(); //pnb edit
            // _model.RawMaterialTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE034)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            //_model.ExchangeRateType = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE018)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();


            //_model.TaxCodes = dc.MA010.OrderByDescending(x => x.MKRGSDT).ToList().ConvertAll(x =>
            //{
            //    return new SelectListItem()
            //    {
            //        Value = x.MKTXCD,
            //        Text = x.MKTXCD.ToString()
            //    };
            //});

            _model.ExchangeRates = dc.MA009.OrderByDescending(x => x.MJRGSDT).ToList().ConvertAll(x =>
            {
                return new SelectListItem()
                {
                    Value = x.MJCRRCD,
                    Text = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE018, x.MJEXRTT)
                };
            });
            //  _model.Specs = dc.MA006.OrderByDescending(x => x.MFRGSDT).ToList().ConvertAll(x =>
            //    {
            //      return new SelectListItem()
            //      {
            //          Value = x.MFPRDSP,
            //         Text = x.MFPRDSP
            //     };
            //  });

            //  _model.Coatings = dc.MA005.OrderByDescending(x => x.MERGSDT).Select(x => new SelectListItem { Value = x.MECOAT, Text = x.MECOAT });
            // _model.UserCodes = dc.MA002.Select(x => new SelectListItem { Value = x.MBUSRCD, Text = x.MBUSRCD + "  -  " + x.MBUSRNM }).ToList();

            //  _model.SteelGrades = dc.SteelGrades.Select(x => new SelectListItem { Value = x.Grade, Text = x.Grade });
            // _model.PersonIncharges = dc.MA003.OrderByDescending(x => x.MCRGSDT).Select(x => new SelectListItem { Value = x.MCIDCD, Text = x.MCIDCD + "  -  " + x.MCIDNM });
            //  _model.SupplierCodes = dc.MA001.OrderByDescending(x => x.MARGSDT).Where(x => x.MASPCTG.Trim().Equals("P")).Select(x => new SelectListItem { Value = x.MASPCD, Text = x.MASPCD + "  -  " + x.MASPNM });
            //   _model.DeliveryConditionCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE024)).Select(x => new SelectListItem { Value = x.MNSRCD.Trim(), Text = x.MNSRCD + "  -  " + x.MNSRNM }).ToList();
            /*sugar edit for the fucking order like shit */

            _model.Grades = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE015)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.MakerCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE005)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.CurrencyCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE012)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.Priceterms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE019)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.TypeOfTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE021)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.CommodityCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE006)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.ContractTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE025)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.SettelementTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE020)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.RawMaterialTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE034)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.ExchangeRateType = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE018)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());

            _model.TaxCodes = dc.MA010.OrderByDescending(x => x.MKRGSDT).ToDictionary(x => x.MKTXCD.Trim(), x => x.MKTXCD.Trim() + " - " + x.MKTXDL.Trim());

            _model.Specs = dc.MA006.OrderByDescending(x => x.MFRGSDT).ToDictionary(x => x.MFPRDSP, x => x.MFPRDSP);
            _model.Coatings = dc.MA005.OrderByDescending(x => x.MERGSDT).ToDictionary(x => x.MECOAT, x => x.MECOAT);
            _model.UserCodes = dc.MA002.ToDictionary(x => x.MBUSRCD, x => x.MBUSRCD.Trim() + "-" + x.MBUSRNM);

            _model.SteelGrades = dc.SteelGrades.ToDictionary(x => x.Grade, x => x.Grade); //PNBEDIT
            _model.PersonIncharges = dc.MA003.OrderByDescending(x => x.MCRGSDT).ToDictionary(x => x.MCIDCD, x => x.MCIDCD.Trim() + "  -  " + x.MCIDNM);
            _model.SupplierCodes = dc.MA001.OrderByDescending(x => x.MARGSDT).Where(x => x.MASPCTG.Trim().Equals("P")).ToDictionary(x => x.MASPCD, x => x.MASPCD.Trim() + "-" + x.MASPNM.Trim());
            _model.DeliveryConditionCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE024)).ToDictionary(x => x.MNSRCD.Trim(), x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.LabelType = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE036)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            return _model;
        }

        public bool CheckCodeCreateExists(string code)
        {
            return INV001s().FirstOrDefault(x => x.CADSTNC.Trim().Equals(code)) == null;
        }

        public int GetTotalRecord(InventorySearchModel searchmodel)
        {
            var model = from a in INV001s() select a;
            if (!String.IsNullOrEmpty(searchmodel.PurchaseContractNo))
            {
                model = model.Where(x => x.CACTRNO == searchmodel.PurchaseContractNo);
            }
            if (searchmodel.ShippingMonth != null)
            {
                model = model.Where(x => x.CASHPDT.Equals(searchmodel.ShippingMonth));
            }
            if (!String.IsNullOrEmpty(searchmodel.SupplierContractNo))
            {
                model = model.Where(x => x.CASPPNO == searchmodel.SupplierContractNo);
            }
            if (!String.IsNullOrEmpty(searchmodel.MakerCode))
            {
                model = model.Where(x => x.CAMKCD == searchmodel.MakerCode);
            }
            if (!String.IsNullOrEmpty(searchmodel.SupplierCode))
            {
                model = model.Where(x => x.CASPLCD == searchmodel.SupplierCode);
            }

            if (!String.IsNullOrEmpty(searchmodel.CommondityCode))
            {
                model = model.Where(x => x.CACMDCD == searchmodel.CommondityCode);
            }

            if (!String.IsNullOrEmpty(searchmodel.UserCode))
            {
                model = model.Where(x => x.CAUSRCD == searchmodel.UserCode);
            }
            if (!String.IsNullOrEmpty(searchmodel.RawMaterialType))
            {
                model = model.Where(x => x.CARMTP == searchmodel.RawMaterialType);
            }

            return model.GroupBy(x => x.CADSTNC, (e, g) => g.FirstOrDefault()).Count();
        }
        public List<Models.INV001> GetTotalDisplayRecord(jQueryDataTableInventory param, InventorySearchModel searchmodel)
        {
            var model = from a in INV001s() select a;
            if (!String.IsNullOrEmpty(searchmodel.PurchaseContractNo))
            {
                model = model.Where(x => x.CACTRNO.Trim() == searchmodel.PurchaseContractNo.Trim());
            }
            if (!String.IsNullOrEmpty(searchmodel.ShippingMonth))
            {
                model = model.Where(x => x.CASHPDT.Equals(searchmodel.ShippingMonth));
            }
            if (!String.IsNullOrEmpty(searchmodel.SupplierContractNo))
            {
                model = model.Where(x => x.CASPPNO.Trim() == searchmodel.SupplierContractNo.Trim());
            }
            if (!String.IsNullOrEmpty(searchmodel.MakerCode))
            {
                model = model.Where(x => x.CAMKCD.Trim() == searchmodel.MakerCode.Trim());
            }
            if (!String.IsNullOrEmpty(searchmodel.SupplierCode))
            {
                model = model.Where(x => x.CASPLCD.Trim() == searchmodel.SupplierCode.Trim());
            }

            if (!String.IsNullOrEmpty(searchmodel.CommondityCode))
            {
                model = model.Where(x => x.CACMDCD.Trim() == searchmodel.CommondityCode.Trim());
            }

            if (!String.IsNullOrEmpty(searchmodel.UserCode))
            {
                model = model.Where(x => x.CAUSRCD.Trim() == searchmodel.UserCode.Trim());
            }
            if (!String.IsNullOrEmpty(searchmodel.RawMaterialType))
            {
                model = model.Where(x => x.CARMTP == searchmodel.RawMaterialType);
            }

            var list = model.GroupBy(x => x.CADSTNC, (e, g) => g.FirstOrDefault()).OrderByDescending(x => new { x.CARGSDT }).Skip(param.start).Take(param.length).ToList();

            return list;
        }

        public enum ScaningMS
        {
            Instock = 1,
            NoExists = 2,
            Exists = 3
        }
        public ScaningMS CheckInventoryNoExists(string inventory)
        {
            var row = db.INV001.Where(x => x.CAINVNO.Equals(inventory.Trim()));
            if (row.Any())
            {
                if (row.Any(x => x.CAINVST.Equals("1")))
                {
                    return ScaningMS.Instock;
                }
                else
                {
                    return ScaningMS.Exists;
                }
            }
            return ScaningMS.NoExists;
        }

        public ScaningMS CheckInspectionScaningExists(string inspectionNo)
        {
            var row = dc.INV001.SingleOrDefault(x => x.CAISPNO.Equals(inspectionNo.Trim()));
            if (row != null)
            {
                if (row.CAINVST.Equals("1"))
                {
                    return ScaningMS.Instock;
                }
                else
                {
                    return ScaningMS.Exists;
                }
            }
            return ScaningMS.NoExists;
        }
        public bool CheckPurchaseContractReferenceRawMaterial(string purchaseContractNo)
        {
            return dc.INV001.Any(x => x.CACTRNO.Equals(purchaseContractNo));
        }

        public List<Models.INV001> GetListInventoryNoToPrintLabel(string fromI, string toI)
        {
            var PurchaseNo1 = dc.INV001.FirstOrDefault(x => x.CAINVNO.Equals(fromI));
            var PurchaseNo2 = dc.INV001.FirstOrDefault(x => x.CAINVNO.Equals(toI));
            var INVlist = new List<Models.INV001>();
            int number = 0;
            string FnumberOfString = string.Empty, TnumberOfString = string.Empty;
            if (PurchaseNo1 !=null && PurchaseNo2 != null)
            {
                if (PurchaseNo1.CACTRNO == PurchaseNo2.CACTRNO)
                {
                    for (int i = 0; i < fromI.Length; i++)
                    {
                        if (int.TryParse(fromI[i].ToString(), out number))
                        {
                            FnumberOfString = fromI.Substring(i, fromI.Length - i);
                            TnumberOfString = toI.Substring(i, toI.Length - i);
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(FnumberOfString))
                    {
                        int Ifrom = int.Parse(FnumberOfString), Ito = int.Parse(TnumberOfString);
                        if (Ifrom > Ito)
                        {
                            var tmp = Ifrom;
                            Ifrom = Ito;
                            Ito = tmp;
                        }

                        Models.INV001 model;
                        string Sstring = fromI.Substring(0, fromI.IndexOf(number.ToString()));
                        string inventoryNo;
                        for (var j = Ifrom; j <= Ito; j++)
                        {
                            inventoryNo = string.Concat(Sstring, j);
                            model = dc.INV001.FirstOrDefault(x=>x.CAINVNO.Equals(inventoryNo));
                            if (model != null)
                            {
                                INVlist.Add(model);
                            }
                        }
                    }
                }
            }
           
            return INVlist;
        }

    }
}