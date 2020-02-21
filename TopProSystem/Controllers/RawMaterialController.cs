using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using TopProSystem.Areas.MasterSetting.DAL.INV001;
using TopProSystem.Areas.MasterSetting.DAL.PUR001;
using TopProSystem.Areas.MasterSetting.DAL.RawMaterialDal;
using TopProSystem.Areas.MasterSetting.Models;
using TopProSystem.Extension.AccountRole;
namespace TopProSystem.Controllers
{
    public class RawMaterialController : BaseRawMaterialController
    {
        private string RawMaterialPDFFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["SaveReportRawMaterialPdfURL"];
        private string SaveExcelURL = System.Web.Configuration.WebConfigurationManager.AppSettings["SaveExcelURL"];
        private Areas.MasterSetting.Models.TopProSystemEntities MasterdataEntities = new Areas.MasterSetting.Models.TopProSystemEntities();
        public void CreateSpecialFolder()
        {
            //string[] arrayRootUrl = RawMaterialPDFFilePath.Split('/');
            //var RootFolder = arrayRootUrl[0] + "/";
            //for (int i = 1; i < arrayRootUrl.Length; i++)
            //{
            //    var FolderName = arrayRootUrl[i];
            //    if (!Directory.Exists(RootFolder + FolderName))
            //    {
            //        Directory.CreateDirectory(RootFolder + FolderName);
            //    }
            //    RootFolder += FolderName + "/";
            //}
            Directory.CreateDirectory(RawMaterialPDFFilePath);
        }
        #region  Raw Material WarehousingEntry_PO
        public ActionResult GetRawMaterialWarehousingEntry_PO()
        {
            PUR001_DAL PUR001_DALs = new PUR001_DAL();
            RawMaterial model = new RawMaterial();
            model.PurchaseContract = PUR001_DALs.GetReferences(model.PurchaseContract);
            return View(model);
        }
        [HttpPost]
        public ActionResult GetRawMaterialWarehousingEntry_PO(FormCollection form, RawMaterial rawmaterial)
        {
            if (rawmaterial.PurchaseContract.AAPURNO == null && rawmaterial.PurchaseContract.ABCTITM == null)
            {
                TempData["Error"] = "Purchase Contract No Item No incorrect";
                return RedirectToAction("GetRawMaterialWarehousingEntry_PO", "RawMaterial");
            }
            PUR001_DAL PUR001_DALs = new PUR001_DAL();
            var PurchaseContractModel = PUR001_DALs.GetPUR001sByPurchaseContractNoAndItemNo(rawmaterial.PurchaseContract.AAPURNO, rawmaterial.PurchaseContract.ABCTITM);
            if (PurchaseContractModel == null)
            {
                TempData["Error"] = "Purchase Contract No Item No incorrect";
                return RedirectToAction("GetRawMaterialWarehousingEntry_PO", "RawMaterial");
            }
            var model = new RawMaterial();
            model.PurchaseContractNo = rawmaterial.PurchaseContract.AAPURNO;
            model.ShippersInvoiceNo = rawmaterial.ShippersInvoiceNo;
            model.StockEntryDate = rawmaterial.StockEntryDate;
            model.DepositAsset = rawmaterial.DepositAsset;
            model.VesselName = rawmaterial.VesselName;
            model.PODate = rawmaterial.PODate;
            model.ShippersInvoiceDate = rawmaterial.ShippersInvoiceDate;
            model.ShippingDate = rawmaterial.ShippingDate;
            model.ItemNo = rawmaterial.PurchaseContract.ABCTITM;
            MasterdataEntities.RawMaterials.Add(model);
            MasterdataEntities.SaveChanges();
            List<InspectionItemNoMaterial> _ListItemMaterial = new List<InspectionItemNoMaterial>();
            #region adddatafromexcel
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                var file = Request.Files[0];
                string filePath = string.Empty;
                if (Request.Files != null)
                {
                    string path = Server.MapPath("~/FileCreated/RawMaterial/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = System.IO.Path.GetFileName("InspectionItemNoMaterial-" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss-ff") + System.IO.Path.GetExtension(file.FileName));
                    //  string extension = System.IO.Path.GetExtension("InspectionItemNoMaterial-" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss-ff") + System.IO.Path.GetExtension(file.FileName));
                    file.SaveAs(path + filePath);
                    ViewBag.File = filePath;
                    FileInfo fileexcel = new FileInfo(System.IO.Path.Combine(path, filePath));
                    using (ExcelPackage package = new ExcelPackage(fileexcel))
                    {
                        // ExcelWorksheet workSheet = package.Workbook.Worksheets["PurchaseContractEntry"];
                        ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
                        int totalRows = workSheet.Dimension.Rows;
                        for (int i = 2; i <= totalRows; i++)
                        {
                            _ListItemMaterial.Add(new InspectionItemNoMaterial
                            {
                                InspectionNo = workSheet.Cells[i, 1].Value.ToString(),
                                RawMaterialId = model.Id,
                                RawMaterialNo = workSheet.Cells[i, 2].Value.ToString(),
                                Quantity = double.Parse(workSheet.Cells[i, 3].Value.ToString()),
                                Weight = double.Parse(workSheet.Cells[i, 4].Value.ToString()),
                                StatusCode = workSheet.Cells[i, 5].Value.ToString(),
                            });
                        }
                    }
                }
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
                {
                    TopProSystemEntities context = null;
                    try
                    {
                        context = new TopProSystemEntities();
                        context.Configuration.AutoDetectChangesEnabled = false;
                        int count = 0;
                        foreach (var entityToInsert in _ListItemMaterial)
                        {
                            ++count;
                            context = AddToContextForInspectionItemNoMaterial(context, entityToInsert, count, 100, true);
                        }
                        context.SaveChanges();
                    }
                    finally
                    {
                        if (context != null)
                            context.Dispose();
                    }
                    scope.Complete();
                }
            }
            #endregion
            #region additemfromform
            List<InspectionItemNoMaterial> item = new List<InspectionItemNoMaterial>();
            int count_item;
            string last_key = form.AllKeys.Last(); // lay ra key cuoi cung de xem co bao nhieu item
            try
            {
                count_item = int.Parse(last_key.Substring(last_key.IndexOf('_') + 1));
            }
            catch
            {
                count_item = 1;
            }
            for (int i = 1; i <= count_item; i++)
            {
                if ((form["InspectionNo_" + i.ToString()].ToString()) != "" && form["RawMaterialNo_" + i.ToString()].ToString() != ""
              )
                {
                    item.Add(new InspectionItemNoMaterial
                    {
                        InspectionNo = form["InspectionNo_" + i.ToString()],
                        RawMaterialNo = form["RawMaterialNo_" + i.ToString()],
                        Quantity = ToDouble(form["Quantity_" + i.ToString()]),
                        Weight = ToDouble(form["Weight_" + i.ToString()]),
                        StatusCode = form["StatusCode_" + i.ToString()],
                    });
                }
            }
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
            {
                TopProSystemEntities context = null;
                try
                {
                    context = new TopProSystemEntities();
                    context.Configuration.AutoDetectChangesEnabled = false;
                    int count = 0;
                    foreach (var entityToInsert in item)
                    {
                        ++count;
                        context = AddToContextForInspectionItemNoMaterial(context, entityToInsert, count, 100, true);
                    }
                    context.SaveChanges();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }
                scope.Complete();
            }
            #endregion
            TempData["Success"] = "Add Success";
            return RedirectToAction("GetRawMaterialWarehousingEntry", "RawMaterial");
        }
        [HttpPost]
        public ActionResult AjaxRawMaterialWarehousingEntry_PO(string SAAPURNO, int? SABCTITM)
        {
            PUR001_DAL PUR001_DALs = new PUR001_DAL();
            RawMaterialDAL RawMaterialDALs = new RawMaterialDAL();
            RawMaterial model = new RawMaterial();
            var PurchaseContractModel = PUR001_DALs.GetPUR001sByPurchaseContractNoAndItemNo(SAAPURNO, SABCTITM);
            model.PurchaseContract = PUR001_DALs.GetReferences(model.PurchaseContract);
            if (PurchaseContractModel != null)
            {
                model.PurchaseContract = PurchaseContractModel;
                model.PurchaseContract = PUR001_DALs.GetReferences(model.PurchaseContract);
                model = RawMaterialDALs.GetNameAjax(model);
                return PartialView("_GetRawMaterialWarehousing", model);
            }
            return Json(new { error = true, message = RenderRazorViewToString("_GetRawMaterialWarehousing", model) }, JsonRequestBehavior.AllowGet);
        }
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion
        public void CreateSelectListItem()
        {
            ViewBag.MakerCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("005")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.UserCode = MasterdataEntities.MA002.OrderByDescending(x => x.MBRGSDT).Select(x => new { x.MBUSRCD, x.MBUSRNM }).ToList();
            ViewBag.CommodityCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("006")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.PersonInCharge = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("005")).ToList();
            ViewBag.ContractType = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("025")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.Spec = MasterdataEntities.MA006.OrderByDescending(x => x.MFRGSDT).ToList();
            ViewBag.Coating = MasterdataEntities.MA005.OrderByDescending(x => x.MERGSDT).ToList();
            ViewBag.Grade = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("015")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
        }
        #region RawMaterial Warehousing Entry Add
        public ActionResult GetRawMaterialWarehousingEntry_Add()
        {
            CreateSelectListItem();
            return View();
        }
        #endregion
        public ActionResult GetRawMaterialWarehousingEntry_Change()
        {
            CreateSelectListItem();
            return View();
        }
        public ActionResult GetRawMaterialWarehousingEntry_Delete()
        {
            CreateSelectListItem();
            return View();
        }
        public ActionResult GetRegisterPurchaseAmount()
        {
            return View();
        }
        #region Register Tariff Amount
        public ActionResult GetRegisterTariffAmount()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AjaxHandlerRegisterTariffAmount(jQueryDataTableParamModel param, decimal tt_tariffAmount = 0)
        {
            var dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            string[] array = null;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                array = param.sSearch.Split('|');
            }
            string sinvoice = array != null ? array[0] : string.Empty;
            string orderNo = array != null ? array[1] : string.Empty;
            int total = dal.GetTotalRecordByshipperInvoiceAndOrderNo(sinvoice, orderNo);
            IEnumerable<TopProSystem.Areas.MasterSetting.Models.INV001> Data_Rs = dal.GetTotalDisplayRecordByshipperInvoiceAndOrderNo(sinvoice, orderNo, param.iDisplayStart, param.iDisplayLength);
            IEnumerable<TopProSystem.Areas.MasterSetting.Models.INV001> data = null;
            IEnumerable<TopProSystem.Areas.MasterSetting.Models.INV001> Data_Rs_NoPaging = dal.GetTotalDisplayRecordByshipperInvoiceAndOrderNo(sinvoice, orderNo, param.iDisplayStart, param.iDisplayLength, paging: false);
            decimal totalWeight = Math.Round((decimal)dal.GetTotalWeightOfInventoryByInvoiceNoAndOrderNo(sinvoice, orderNo), 2);
            decimal totalCurrentTariff = Convert.ToDecimal((double)Data_Rs_NoPaging.Sum(x => x.CATRFAT));
            decimal totalExceptLastOne = SumAllAboutFomularForTariffAmountExceptLastOne(Data_Rs_NoPaging.Take(Data_Rs_NoPaging.Count() - 1), tt_tariffAmount, totalWeight);
            bool flag = false;
            if (((param.iDisplayStart / param.iDisplayLength) + 1) * param.iDisplayLength >= total)
            {
                data = Data_Rs.Take(Data_Rs.Count() - 1);
                flag = true;
            }
            else
            {
                data = Data_Rs;
            }
            var result = data.Select(x => new
            {
                CACTRTP = x.CACTRTP,
                CAINVST = x.CAINVST,
                CAINVNO = x.CAINVNO,
                CASPEC = x.CASPEC,
                CABSZT = x.CABSZT,
                CABSZW = x.CABSZW,
                CABSZL = x.CABSZL,
                CAPRDNM = x.CAPRDNM,
                CAPRDDIA = x.CAPRDDIA,
                CASTLGR = x.CASTLGR,
                CAQTY = x.CAQTY,
                CAWT = x.CAWT,
                CATRFAT = x.CATRFAT,
                new_CATRFAT = Convert.ToDecimal(Math.Round(Convert.ToDecimal(x.CAWT) / totalWeight * tt_tariffAmount, 2))
            }).ToList();
            if (flag)
            {
                var last = Data_Rs.Skip(Data_Rs.Count() - 1).Take(1).Select(x => new
                {
                    CACTRTP = x.CACTRTP,
                    CAINVST = x.CAINVST,
                    CAINVNO = x.CAINVNO,
                    CASPEC = x.CASPEC,
                    CABSZT = x.CABSZT,
                    CABSZW = x.CABSZW,
                    CABSZL = x.CABSZL,
                    CAPRDNM = x.CAPRDNM,
                    CAPRDDIA = x.CAPRDDIA,
                    CASTLGR = x.CASTLGR,
                    CAQTY = x.CAQTY,
                    CAWT = x.CAWT,
                    CATRFAT = x.CATRFAT,
                    new_CATRFAT = Convert.ToDecimal(tt_tariffAmount) - totalExceptLastOne
                }).ToList();
                result.AddRange(last);
            }
            return Json(new
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = result,
                _total = totalWeight,
                _totalCurrentTariff = totalCurrentTariff
            }, JsonRequestBehavior.AllowGet);
        }
        public decimal SumAllAboutFomularForTariffAmountExceptLastOne(IEnumerable<TopProSystem.Areas.MasterSetting.Models.INV001> data, decimal tariffAmount, decimal totaWeight)
        {
            decimal total = 0;
            foreach (var row in data)
            {
                total += Math.Round(Convert.ToDecimal(row.CAWT) / totaWeight * tariffAmount, 2);
            }
            return total;
        }
        private enum AnounceResult
        {
            error = 0,
            success = 1,
            nodatafound = 2
        }
        public JsonResult UpdateRegisterTariffAmount(string sSearch, decimal tt_tariffAmount)
        {
            var dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            var tra_dal = new Areas.MasterSetting.DAL.TRA001.TRA001_DAL();
            string[] array = null;
            if (!string.IsNullOrEmpty(sSearch))
            {
                array = sSearch.Split('|');
            }
            string sinvoice = array != null ? array[0] : string.Empty;
            string orderNo = array != null ? array[1] : string.Empty;
            int total = dal.GetTotalRecordByshipperInvoiceAndOrderNo(sinvoice, orderNo);
            if (total == 0)
            {
                return Json(AnounceResult.nodatafound, JsonRequestBehavior.AllowGet);
            }
            using (var scope = new TransactionScope())
            {
                if (dal.UpdateRegisterTariffAmount(sinvoice, orderNo, tt_tariffAmount))
                {
                    scope.Complete();
                    return Json(AnounceResult.success, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    scope.Dispose();
                    return Json(AnounceResult.error, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion
        #region Register Custom Handling Amount 
        public ActionResult RegisterCustomHandlingAmount()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SearchRegisterCustomHandlingAmount(string CAPIVNO, string CAORDNO)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO);
            if (countRecord != 0)
            {
                return Json(true);
            }
            return Json(false);
        }
        [HttpPost]
        public ActionResult AjaxHandlerRegisterCustomHandlingAmount(string CAPIVNO, string CAORDNO, double? TotalHandling, jQueryDataTablePurchase param)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO);
            var displayItem = Dal.GetTotalDisplayRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO, param.start, param.length);
            var allItem = Dal.GetTotalDisplayRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO, param.start, param.length, false);
            double? totalCAQTY = allItem.Sum(x => x.CAQTY);
            double? totalCAWT = MathRound(allItem.Sum(x => x.CAWT));
            decimal? totalCACSTHC = Convert.ToDecimal(allItem.Sum(x => x.CACSTHC));
            long IdLastItem = 0;
            decimal total = 0;
            foreach (var item in allItem.Take(countRecord - 1))
            {
                total += Convert.ToDecimal(MathRound(item.CAWT / totalCAWT * TotalHandling));
            }
            if (countRecord > 0)
            {
                IdLastItem = allItem.LastOrDefault().ID;
            }
            List<INV001> result = displayItem.Select(x =>
            {
                var model = new INV001();
                model.CACTRTP = x.CACTRTP;
                model.CAINVST = x.CAINVST;
                model.CAINVNO = x.CAINVNO;
                model.CASPEC = x.CASPEC;
                model.CABSZT = x.CABSZT;
                model.CABSZW = x.CABSZW;
                model.CABSZL = x.CABSZL;
                model.CAPRDNM = x.CAPRDNM;
                model.CAPRDDIA = x.CAPRDDIA;
                model.CASTLGR = x.CASTLGR;
                model.CAQTY = x.CAQTY;
                model.CAWT = x.CAWT;
                model.CACSTHC = x.CACSTHC.GetValueOrDefault();
                if (x.ID != IdLastItem)
                {
                    model.NCACSTHC = MathRound(x.CAWT / totalCAWT * TotalHandling);
                }
                else
                {
                    model.NCACSTHC = Convert.ToDouble((Convert.ToDecimal(TotalHandling) - total));
                }
                return model;
            }).ToList();
            return Json(new
            {
                recordsTotal = countRecord,
                recordsFiltered = countRecord,
                draw = param.draw,
                data = result,
                totalCAQTY = totalCAQTY,
                totalCAWT = totalCAWT,
                totalCACSTHC = totalCACSTHC.GetValueOrDefault(),
                totalNCACSTHC = TotalHandling.GetValueOrDefault(),
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateRegisterCustomHandlingAmount(string CAPIVNO, string CAORDNO, decimal TotalHandling = 0)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO);
            if (countRecord == 0)
            {
                return Json(AnounceResult.nodatafound, JsonRequestBehavior.AllowGet);
            }
            using (var scope = new TransactionScope())
            {
                if (Dal.UpdateCustomHandlingAmount(CAPIVNO, CAORDNO, TotalHandling))
                {
                    scope.Complete();
                    return Json(AnounceResult.success, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    scope.Dispose();
                    return Json(AnounceResult.error, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public double? MathRound(double? number)
        {
            return Math.Round(number.GetValueOrDefault(), 2);
        }
        #endregion
        #region Register Freight Amount 
        public ActionResult GetRegisterFreightAmount()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SearchRegisterFreightAmount(string CAPIVNO, string CAORDNO)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO);
            if (countRecord != 0)
            {
                return Json(true);
            }
            return Json(false);
        }
        [HttpPost]
        public ActionResult AjaxHandlerRegisterFreightAmount(string CAPIVNO, string CAORDNO, double? TotalFreight, jQueryDataTablePurchase param)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO);
            var displayItem = Dal.GetTotalDisplayRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO, param.start, param.length);
            var allItem = Dal.GetTotalDisplayRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO, param.start, param.length, false);
            double? totalCAQTY = allItem.Sum(x => x.CAQTY);
            double? totalCAWT = MathRound(allItem.Sum(x => x.CAWT));
            decimal? totalCAPUFAT = Convert.ToDecimal(allItem.Sum(x => x.CAPUFAT));
            long IdLastItem = 0;
            decimal total = 0;
            foreach (var item in allItem.Take(countRecord - 1))
            {
                total += Convert.ToDecimal(MathRound(item.CAWT / totalCAWT * TotalFreight));
            }
            if (countRecord > 0)
            {
                IdLastItem = allItem.LastOrDefault().ID;
            }
            List<INV001> result = displayItem.Select(x =>
            {
                var model = new INV001();
                model.CACTRTP = x.CACTRTP;
                model.CAINVST = x.CAINVST;
                model.CAINVNO = x.CAINVNO;
                model.CASPEC = x.CASPEC;
                model.CABSZT = x.CABSZT;
                model.CABSZW = x.CABSZW;
                model.CABSZL = x.CABSZL;
                model.CAPRDNM = x.CAPRDNM;
                model.CAPRDDIA = x.CAPRDDIA;
                model.CASTLGR = x.CASTLGR;
                model.CAQTY = x.CAQTY;
                model.CAWT = x.CAWT;
                model.CAPUFAT = x.CAPUFAT.GetValueOrDefault();
                if (x.ID != IdLastItem)
                {
                    model.NCAPUFAT = MathRound(x.CAWT / totalCAWT * TotalFreight);
                }
                else
                {
                    model.NCAPUFAT = Convert.ToDouble((Convert.ToDecimal(TotalFreight) - total));
                }
                return model;
            }).ToList();
            return Json(new
            {
                recordsTotal = countRecord,
                recordsFiltered = countRecord,
                draw = param.draw,
                data = result,
                totalCAQTY = totalCAQTY,
                totalCAWT = totalCAWT,
                totalCAPUFAT = totalCAPUFAT,
                totalNCAPUFAT = TotalFreight.GetValueOrDefault(),
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateRegisterFreightAmount(string CAPIVNO, string CAORDNO, decimal TotalFreight = 0)
        {
            INV001_DAL Dal = new INV001_DAL();
            var allItem = Dal.GetTotalDisplayRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO, 0, 0, false);
            var countRecord = Dal.GetTotalRecordByshipperInvoiceAndOrderNo(CAPIVNO, CAORDNO);
            if (countRecord == 0)
            {
                return Json(AnounceResult.nodatafound);
            }
            using (var scope = new TransactionScope())
            {
                if (Dal.UpdateFreightAmount(CAPIVNO, CAORDNO, TotalFreight))
                {
                    scope.Complete();
                    return Json(AnounceResult.success, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    scope.Dispose();
                    return Json(AnounceResult.error, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion
        public ActionResult GetRawMaterialRequestEntry()
        {
            return View();
        }
        public ActionResult GetRawMaterialRequestResult()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetRawMaterialRequestResult(FormCollection form, string Command)
        {
            if (form["Inventory"] == null)
            {
                ViewBag.Page = true;
                TempData["MaterialRequestNo"] = form["MaterialRequestNo"].ToString();
                TempData["Date"] = form["Date"].ToString();
                return View();
            }
            else
            {
                string Date;
                string MaterialRequestNo;
                if (TempData.ContainsKey("Date"))
                    Date = TempData["Date"].ToString();
                if (TempData.ContainsKey("MaterialRequestNo"))
                    MaterialRequestNo = TempData["MaterialRequestNo"].ToString();
                return View();
            }
        }
        #region Print Raw Material Label
        public ActionResult GetPrintRawMaterialLabel()
        {
            var Ma012Dal = new Areas.MasterSetting.DAL.MA012.MA012_DAL();
            ViewBag.LabelType = Ma012Dal.GetAll().Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE036));
            return View();
        }
        public PrintStampMS PrintStamp(string path)
        {
            //var server = new PrintServer();
            //var queue = server.GetPrintQueues(new[]
            //{ EnumeratedPrintQueueTypes.Shared, EnumeratedPrintQueueTypes.Connections });
            ////var printerlist = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
            //string a = string.Empty;
            //foreach (var s in queue)
            //{
            //    a = string.Concat(a, s.FullName);
            //}
            //System.IO.File.WriteAllText(System.IO.Path.Combine(RawMaterialPDFFilePath, "machinelist.txt"), a);
            string PrinterLabel = System.Web.Configuration.WebConfigurationManager.AppSettings["LabelMachinePrinter"];
            string accountShareFolder = System.Web.Configuration.WebConfigurationManager.AppSettings["AccountShareFolder"];
            string uname = accountShareFolder.Split('|')[0], psw = accountShareFolder.Split('|')[1];
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(uname, psw);
            using (new Extension.Printer.ConnectToSharedFolder(PrinterLabel, credentials))
            {
                var dectectPrinter = Extension.Printer.PrinterMachine.DetectPrinterMachine(PrinterLabel);
                if (dectectPrinter)
                {
                    string stockName = System.Web.Configuration.WebConfigurationManager.AppSettings["StockName"];
                    Extension.Printer.PrinterMachine.PrintPDF(PrinterLabel, stockName, path, 1, printLabel: true);
                    System.IO.File.Delete(path);
                    return PrintStampMS.Success;
                }
                return PrintStampMS.NoPrinterMachingFound;
            }
        }
        public static PdfPCell FormatCell(int border = 0, Paragraph work = null, int alignMent = PdfPCell.ALIGN_LEFT, float padding = 0f)
        {
            PdfPCell cell = new PdfPCell(work);
            cell.Border = border;
            cell.HorizontalAlignment = alignMent;
            cell.Padding = padding;
            return cell;
        }
        public static PdfPCell FormatCell(int border = 0, Image img = null, int alignMent = PdfPCell.ALIGN_LEFT, float padding = 0f)
        {
            PdfPCell cell = new PdfPCell(img);
            cell.Border = border;
            cell.HorizontalAlignment = alignMent;
            cell.Padding = padding;
            return cell;
        }
        public enum PrintStampMS
        {
            Success = 0,
            Error = 1,
            NoMatch = 2,
            NoPrinterMachingFound = 3
        }
        [HttpGet]
        public ActionResult CreateStamp1(string myString)
        {
            var message = "";
            try
            {
                if (myString == "") return Json(message = "fail", JsonRequestBehavior.AllowGet);
                CreateSpecialFolder();
                string[] tmpArray = myString.Split(' ');
                Rectangle RETC = new Rectangle(312, 199);
                RETC.Border = Rectangle.BOX;
                RETC.BorderWidth = 1;
                RETC.BorderColor = BaseColor.BLACK;
                Document doc = new Document(RETC, 10f, 1f, 0, 0);
                string filename = "Stamp1_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.Millisecond + ".pdf";
                //String path = Server.MapPath('~' + RawMaterialPDFFilePath + filename);
                string path = System.IO.Path.GetFullPath(RawMaterialPDFFilePath + filename);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                string barcode_img_path = Server.MapPath("~/Images/pdffileimg/barcode.jpg");
                doc.Open();
                for (int i = int.Parse(tmpArray[0]); i <= int.Parse(tmpArray[1]); i++)
                {
                    doc.NewPage();
                    Font FHeader = FontFactory.GetFont("Franklin Gothic", 8, Font.BOLD, BaseColor.BLACK);
                    Font FCode = FontFactory.GetFont("Franklin Gothic", 19, Font.BOLD, BaseColor.BLACK);
                    PdfPTable myTable = new PdfPTable(1); // table bự
                    myTable.WidthPercentage = 97f;
                    PdfPTable Table = new PdfPTable(1);
                    Table.WidthPercentage = 100f;
                    PdfPCell cell = FormatCell(work: new Paragraph("Mother Coil No", FHeader));
                    Table.AddCell(cell);
                    Paragraph CodeCreated = new Paragraph("SS18168" + i, FCode);
                    cell = FormatCell(work: CodeCreated, alignMent: PdfPCell.ALIGN_CENTER);
                    Table.AddCell(cell);
                    Image barcode_img = Image.GetInstance(barcode_img_path); // barcode
                    barcode_img.ScaleAbsolute(200f, 30f);
                    cell = FormatCell(img: barcode_img, alignMent: PdfPCell.ALIGN_CENTER);
                    cell.PaddingTop = 3f;
                    cell.PaddingBottom = 2f;
                    Table.AddCell(cell);
                    PdfPCell myCell = new PdfPCell(Table); // cell bự
                    myCell.Padding = 3f;
                    myTable.AddCell(myCell); // end cell 1;
                    Table = new PdfPTable(1);
                    Table.WidthPercentage = 100f;
                    cell = FormatCell(work: new Paragraph("Grade", FHeader));
                    Table.AddCell(cell);
                    CodeCreated = new Paragraph("M4/M120-27S", FCode);
                    cell = FormatCell(work: CodeCreated);
                    Table.AddCell(cell);
                    myCell = new PdfPCell(Table); // cell bự
                    myCell.Padding = 3f;
                    myTable.AddCell(myCell); // end cell 2;
                    Table = new PdfPTable(2);
                    Table.WidthPercentage = 100f;
                    cell = FormatCell(work: new Paragraph("Net Weight(Kgs)", FHeader));
                    Table.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("Size (mm)", FHeader));
                    Table.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("4070", FCode), alignMent: PdfPCell.ALIGN_CENTER);
                    Table.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("0.5", FCode), alignMent: PdfPCell.ALIGN_CENTER);
                    Table.AddCell(cell);
                    myCell = new PdfPCell(Table); // cell bự
                    myCell.Padding = 3f;
                    myTable.AddCell(myCell); // end cell 3;
                    Table = new PdfPTable(1);
                    Table.WidthPercentage = 100f;
                    cell = FormatCell(work: new Paragraph("Mill Inpection No", FHeader));
                    Table.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("10436-5812457", FCode));
                    Table.AddCell(cell);
                    myCell = new PdfPCell(Table); // cell bự
                    myCell.Padding = 3f;
                    myTable.AddCell(myCell); // end cell 4;
                    doc.Add(myTable);
                }
                doc.Close();
                // PrintStamp(path);
                message = "success";
            }
            catch (Exception)
            {
                message = "Printer not found !";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult CreateStamp2(string myString)
        {
            var message = "";
            try
            {
                if (myString == null)
                {
                    message = "Fail";
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
                CreateSpecialFolder();
                string[] tmpArray = myString.Split(' ');
                Rectangle RETC = new Rectangle(312, 540);
                Document doc = new Document(RETC, 20f, 20f, 10f, 0f);
                string filename = "stamp2_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.Millisecond + ".pdf";
                // string path = Server.MapPath('~' + RawMaterialPDFFilePath + filename);
                string path = System.IO.Path.GetFullPath(RawMaterialPDFFilePath + filename);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                string barcode_img_path = Server.MapPath("~/Images/pdffileimg/barcode.jpg");
                doc.Open();
                for (int i = int.Parse(tmpArray[0]); i <= int.Parse(tmpArray[1]); i++)
                {
                    doc.NewPage();
                    //Font FHeader = FontFactory.GetFont("Franklin Gothic", 10, Font.BOLD, BaseColor.BLACK);
                    //Font FCode = FontFactory.GetFont("Franklin Gothic", 22, Font.BOLD, BaseColor.BLACK);
                    //Font FBarCode = FontFactory.GetFont("Franklin Gothic", 38, Font.BOLD, BaseColor.BLACK);
                    //Font FUnderBarCode = FontFactory.GetFont("Franklin Gothic", 8, Font.BOLD, BaseColor.BLACK);
                    string FontFamily = @"";
                    Font FHeader = FontFactory.GetFont(FontFamily, 10, Font.BOLD, BaseColor.BLACK);
                    Font FCode = FontFactory.GetFont(FontFamily, 22, Font.BOLD, BaseColor.BLACK);
                    Font FBarCode = FontFactory.GetFont(FontFamily, 38, Font.BOLD, BaseColor.BLACK);
                    Font FUnderBarCode = FontFactory.GetFont(FontFamily, 8, Font.BOLD, BaseColor.BLACK);
                    PdfPTable BigTable = new PdfPTable(1);
                    BigTable.WidthPercentage = 100f;
                    PdfPTable SmallTable = new PdfPTable(1); // small table of cell 1 belong to big table | row 1
                    SmallTable.WidthPercentage = 100f;
                    PdfPCell cell = FormatCell(work: new Paragraph("Mother Coil No", FHeader));
                    SmallTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("SS18168" + i, FBarCode), alignMent: PdfPCell.ALIGN_CENTER);
                    SmallTable.AddCell(cell);
                    Image barcode_img = Image.GetInstance(barcode_img_path); // barcode
                    barcode_img.ScaleAbsolute(240f, 50f);
                    cell = FormatCell(img: barcode_img, alignMent: PdfPCell.ALIGN_CENTER);
                    cell.PaddingTop = 10f;
                    SmallTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("SS18168" + i, FUnderBarCode), alignMent: PdfPCell.ALIGN_CENTER);
                    SmallTable.AddCell(cell);
                    cell = new PdfPCell(SmallTable); //cell of big table 
                    cell.PaddingBottom = 5f;
                    cell.FixedHeight = 143f;
                    BigTable.AddCell(cell);
                    SmallTable = new PdfPTable(1);//row 2
                    cell = FormatCell(work: new Paragraph("Grade", FHeader));
                    SmallTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("M4/M120-27S", FCode));
                    SmallTable.AddCell(cell);
                    cell = new PdfPCell(SmallTable);
                    cell.PaddingBottom = 10f;
                    cell.FixedHeight = 56.6f;
                    BigTable.AddCell(cell);
                    SmallTable = new PdfPTable(1);//row 3
                    cell = FormatCell(work: new Paragraph("Size (mm)", FHeader));
                    SmallTable.AddCell(cell);
                    string value1 = "0.27";
                    string value2 = "970";
                    string value3 = "Coil";
                    cell = FormatCell(work: new Paragraph(value1 + "   X   " + value2 + "   X   " + value3, FCode), alignMent: PdfPCell.ALIGN_CENTER);
                    SmallTable.AddCell(cell);
                    cell = new PdfPCell(SmallTable);
                    cell.PaddingBottom = 10f;
                    cell.FixedHeight = 56.6f;
                    BigTable.AddCell(cell);
                    SmallTable = new PdfPTable(2);//row 4
                    cell = FormatCell(work: new Paragraph("Quantity/Pcs", FHeader), border: PdfPCell.RIGHT_BORDER);
                    SmallTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("Net Weight(Kgs)", FHeader));
                    SmallTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("1", FCode), border: PdfPCell.RIGHT_BORDER, alignMent: PdfPCell.ALIGN_CENTER);
                    SmallTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("4070", FCode), alignMent: PdfPCell.ALIGN_CENTER);
                    cell.PaddingBottom = 10f;
                    SmallTable.AddCell(cell);
                    cell = new PdfPCell(SmallTable);
                    cell.Padding = 0;
                    cell.FixedHeight = 56.6f;
                    BigTable.AddCell(cell);
                    SmallTable = new PdfPTable(1);//row 5
                    cell = FormatCell(work: new Paragraph("Mill Inspection No", FHeader));
                    SmallTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("10436-5812457", FCode));
                    SmallTable.AddCell(cell);
                    cell = new PdfPCell(SmallTable);
                    cell.PaddingBottom = 10f;
                    BigTable.AddCell(cell);
                    SmallTable = new PdfPTable(1);// row 6
                    cell = FormatCell(work: new Paragraph("SS18168" + i, FBarCode), alignMent: PdfPCell.ALIGN_CENTER);
                    SmallTable.AddCell(cell);
                    var barcode_img2 = Image.GetInstance(barcode_img_path);
                    barcode_img2.ScaleAbsolute(240f, 40f);
                    cell = FormatCell(img: barcode_img2, alignMent: PdfPCell.ALIGN_CENTER);
                    cell.PaddingTop = 3f;
                    SmallTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("SS18168" + i, FUnderBarCode), alignMent: PdfPCell.ALIGN_CENTER);
                    SmallTable.AddCell(cell);
                    cell = new PdfPCell(SmallTable);
                    cell.PaddingBottom = 3f;
                    BigTable.AddCell(cell);
                    doc.Add(BigTable);
                    //barcode under
                    BigTable = new PdfPTable(2);
                    BigTable.WidthPercentage = 100f;
                    BigTable.SpacingBefore = 30f;
                    Image img_under1 = Image.GetInstance(barcode_img_path);
                    img_under1.ScaleAbsolute(120, 25);
                    cell = FormatCell(img: img_under1);
                    BigTable.AddCell(cell);
                    Image img_under2 = Image.GetInstance(barcode_img_path);
                    img_under2.ScaleAbsolute(120, 25);
                    cell = FormatCell(img: img_under2, alignMent: PdfPCell.ALIGN_RIGHT);
                    BigTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("SS18168" + i, FUnderBarCode), alignMent: PdfPCell.ALIGN_CENTER);
                    BigTable.AddCell(cell);
                    cell = FormatCell(work: new Paragraph("SS18168" + i, FUnderBarCode), alignMent: PdfPCell.ALIGN_CENTER);
                    BigTable.AddCell(cell);
                    doc.Add(BigTable);
                }
                doc.Close();
                // PrintStamp(path);
                message = "success";
            }
            catch (Exception)
            {
                message = "Printer not found";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        private byte[] turnImageToByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }
        private string ImageStringFormat(System.Drawing.Image img)
        {
            byte[] imgBytes = turnImageToByteArray(img);
            string imgString = Convert.ToBase64String(imgBytes);
            string rs = String.Format("img src=\"data:image/Bmp;base64,{0}\">", imgString);
            return rs;
        }
        public JsonResult GetReviewLabel(string fromR, string toR, int? skip, int? take)
        {
            if (!string.IsNullOrEmpty(fromR) && !string.IsNullOrEmpty(toR))
            {
                var inv001Dal = new INV001_DAL();
                var INVList = inv001Dal.GetListInventoryNoToPrintLabel(fromR, toR);
                if (INVList.Any())
                {
                    int _skip = skip ?? 0, _take = take ?? INVList.Count();
                    var resultLst = INVList.Skip(_skip).Take(_take);
                    return Json(new
                    {
                        status = PrintStampMS.Success,
                        list = resultLst.Select(x => new
                        {
                            CAINVNO = x.CAINVNO,
                            CAWT = x.CAWT,
                            CASTLGR = x.CASTLGR,
                            CAPRDDIA = x.CAPRDDIA,
                            BarCode = ImageStringFormat(GenCode128.Code128Rendering.MakeBarcodeImage(x.CAINVNO, 2, true))
                        }),
                        dataLength = INVList.Count
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = PrintStampMS.NoMatch }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = PrintStampMS.NoMatch }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRawMaterialLabel(string fromR, string toR)
        {
            if (!string.IsNullOrEmpty(fromR) && !string.IsNullOrEmpty(toR))
            {
                try
                {
                    var inv001Dal = new INV001_DAL();
                    var INVList = inv001Dal.GetListInventoryNoToPrintLabel(fromR, toR);
                    if (INVList.Any())
                    {
                        CreateSpecialFolder();
                        string path = CreateLabelRawMaterial(INVList);
                        if (PrintStamp(path) == PrintStampMS.NoPrinterMachingFound)
                        {
                            return Json(new { status = PrintStampMS.NoPrinterMachingFound, error = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { status = PrintStampMS.Success, error = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = PrintStampMS.NoMatch, error = 0 }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { status = PrintStampMS.Error, error = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = PrintStampMS.NoMatch, error = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        public string CreateLabelRawMaterial(List<INV001> INVList)
        {
            List<string> FilePaths = new List<string>();
            Rectangle RETC = new Rectangle(289, 197);
            Document doc = new Document(RETC, 5f, 5f, 15f, 0f);
            string filename = "RawMaterialLabel_" + string.Format("{0:ddMMyyyyhhmmss}", DateTime.Now) + ".pdf";
            string path = System.IO.Path.GetFullPath(RawMaterialPDFFilePath + filename);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.Open();
            foreach (var data in INVList)
            {
                doc.NewPage();
                iTextSharp.text.pdf.BaseFont VietBaseFont = iTextSharp.text.pdf.BaseFont.CreateFont(Server.MapPath("~/Font/RobotoCondensed-Regular.ttf"), iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
                //iTextSharp.text.pdf.BaseFont VietBaseFont = iTextSharp.text.pdf.BaseFont.CreateFont(BaseFont.TIMES_ROMAN, iTextSharp.text.pdf.BaseFont.CP1250, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                Font FHeader = new Font(VietBaseFont, 9, Font.NORMAL, BaseColor.BLACK);
                Font FCode = new Font(VietBaseFont, 15, Font.NORMAL, BaseColor.BLACK);
                PdfPTable myTable = new PdfPTable(1); // table bự
                myTable.WidthPercentage = 100f;
                PdfPTable Table = new PdfPTable(2);
                Table.WidthPercentage = 100f;
                PdfPCell cell = new PdfPCell();
                cell.Padding = 0;
                cell.Border = 0;
                cell.AddElement(new Paragraph("Raw Material Number", FHeader) { Leading = 6 });
                Table.AddCell(cell);
                cell = new PdfPCell();
                cell.Padding = 0;
                cell.Border = 0;
                cell.AddElement(new Paragraph("Formosa Gear", FHeader) { Alignment = Element.ALIGN_RIGHT, Leading = 6 });
                Table.AddCell(cell);
                string barcode = data.CAINVNO;
                System.Drawing.Image BarCodeGenergate = GenCode128.Code128Rendering.MakeBarcodeImage(barcode, 2, true);
                //System.Drawing.Image BarCodeGenergate = ResizeImage(BarCodeGen, 200, 40);
                Paragraph CodeCreated = new Paragraph(barcode, FCode);
                cell = FormatCell(work: CodeCreated, alignMent: PdfPCell.ALIGN_CENTER);
                cell.Colspan = 2;
                Table.AddCell(cell);
                Image barcode_img = Image.GetInstance(BarCodeGenergate, BaseColor.BLACK, true);
                barcode_img.ScaleAbsolute(240f, 40f);
                cell = FormatCell(img: barcode_img, alignMent: PdfPCell.ALIGN_CENTER);
                cell.Colspan = 2;
                cell.PaddingTop = 3f;
                cell.PaddingBottom = 2f;
                Table.AddCell(cell);
                PdfPCell myCell = new PdfPCell(Table); // cell bự
                myCell.Padding = 3f;
                myTable.AddCell(myCell); // end cell 1;
                Table = new PdfPTable(1);
                Table.WidthPercentage = 100f;
                cell = FormatCell(work: new Paragraph("Steal Grade", FHeader));
                Table.AddCell(cell);
                CodeCreated = new Paragraph(data.CASTLGR, FCode);
                cell = FormatCell(work: CodeCreated);
                Table.AddCell(cell);
                myCell = new PdfPCell(Table); // cell bự
                myCell.Padding = 3f;
                myTable.AddCell(myCell); // end cell 2;
                Table = new PdfPTable(2);
                Table.WidthPercentage = 100f;
                cell = FormatCell(work: new Paragraph("Weight(Kgs)", FHeader));
                Table.AddCell(cell);
                cell = FormatCell(work: new Paragraph("Size(mm)", FHeader));
                Table.AddCell(cell);
                cell = FormatCell(work: new Paragraph(data.CAWT.ToString(), FCode), alignMent: PdfPCell.ALIGN_CENTER);
                Table.AddCell(cell);
                cell = FormatCell(work: new Paragraph(data.CAPRDDIA, FCode), alignMent: PdfPCell.ALIGN_CENTER);
                Table.AddCell(cell);
                myCell = new PdfPCell(Table); // cell bự
                myCell.Padding = 3f;
                myTable.AddCell(myCell); // end cell 3;
                Table = new PdfPTable(2);
                var re_barcode = Image.GetInstance(BarCodeGenergate, BaseColor.BLACK, true);
                re_barcode.ScaleAbsolute(140f, 40f);
                cell = new PdfPCell();
                cell.AddElement(re_barcode);
                cell.AddElement(new Paragraph(barcode, FHeader) { Leading = 8, Alignment = Element.ALIGN_CENTER });
                cell.Border = 0;
                cell.PaddingTop = 5f;
                cell.PaddingBottom = 5f;
                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                Table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.PaddingTop = 5f;
                cell.PaddingBottom = 5f;
                re_barcode.Alignment = Element.ALIGN_CENTER;
                cell.AddElement(re_barcode);
                cell.AddElement(new Paragraph(barcode, FHeader) { Leading = 8, Alignment = Element.ALIGN_CENTER });
                Table.AddCell(cell);
                myTable.AddCell(Table);
                doc.Add(myTable);
            }
            doc.Close();
            return path;
        }
        #endregion
        //public ActionResult UploadExcel()
        //     {
        //         var _ListItemMaterial = new List<InspectionItemNoMaterial>();
        //         if (Request.Files.Count > 0)
        //             {
        //             var file = Request.Files[0];
        //             string filePath = string.Empty;
        //             if (Request.Files != null)
        //             {
        //                 string path = Server.MapPath("~/FileCreated/RawMaterial/");
        //                 if (!Directory.Exists(path))
        //                 {
        //                     Directory.CreateDirectory(path);
        //                 }
        //                 filePath =  System.IO.Path.GetFileName("InspectionItemNoMaterial-" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss-ff") + System.IO.Path.GetExtension(file.FileName));
        //               //  string extension = System.IO.Path.GetExtension("InspectionItemNoMaterial-" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss-ff") + System.IO.Path.GetExtension(file.FileName));
        //                 file.SaveAs(path+filePath);
        //                 ViewBag.File = filePath;
        //                 FileInfo fileexcel = new FileInfo(System.IO.Path.Combine(path, filePath));
        //                 using (ExcelPackage package = new ExcelPackage(fileexcel))
        //                 {
        //                     // ExcelWorksheet workSheet = package.Workbook.Worksheets["PurchaseContractEntry"];
        //                     ExcelWorksheet workSheet = package.Workbook.Worksheets.First() ;
        //                     int totalRows = workSheet.Dimension.Rows;
        //                     for (int i = 2; i <= 17; i++)
        //                     {
        //                         _ListItemMaterial.Add(new InspectionItemNoMaterial
        //                         {
        //                             InspectionNo = workSheet.Cells[i, 1].Value.ToString(),
        //                             RawMaterialNo = workSheet.Cells[i, 2].Value.ToString(),
        //                             Quantity = workSheet.Cells[i, 3].Value.ToString(),
        //                             Weight = workSheet.Cells[i, 4].Value.ToString(),
        //                             StatusCode = workSheet.Cells[i, 5].Value.ToString(),
        //                         });                                                                          
        //                     }
        //                 }  
        //             }
        //          }
        //            return PartialView("_ImportExcelItemNo", _ListItemMaterial);
        //     }
        [HttpPost]
        public ActionResult GetRawMaterialWarehousingEntry_Add(RawMaterial model, string command)
        {
            switch (command)
            {
                case "additem":
                    CreateSelectListItem();
                    return View(model);
                default:
                    MasterdataEntities.RawMaterials.Add(model);
                    MasterdataEntities.SaveChanges();
                    var _ListItemMaterial = new List<InspectionItemNoMaterial>();
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        string filePath = string.Empty;
                        if (Request.Files != null)
                        {
                            string path = Server.MapPath("~/FileCreated/RawMaterial/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            filePath = System.IO.Path.GetFileName("InspectionItemNoMaterial-" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss-ff") + System.IO.Path.GetExtension(file.FileName));
                            //  string extension = System.IO.Path.GetExtension("InspectionItemNoMaterial-" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss-ff") + System.IO.Path.GetExtension(file.FileName));
                            file.SaveAs(path + filePath);
                            ViewBag.File = filePath;
                            FileInfo fileexcel = new FileInfo(System.IO.Path.Combine(path, filePath));
                            using (ExcelPackage package = new ExcelPackage(fileexcel))
                            {
                                // ExcelWorksheet workSheet = package.Workbook.Worksheets["PurchaseContractEntry"];
                                ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
                                int totalRows = workSheet.Dimension.Rows;
                                for (int i = 2; i <= totalRows; i++)
                                {
                                    _ListItemMaterial.Add(new InspectionItemNoMaterial
                                    {
                                        InspectionNo = workSheet.Cells[i, 1].Value.ToString(),
                                        RawMaterialId = model.Id,
                                        RawMaterialNo = workSheet.Cells[i, 2].Value.ToString(),
                                        Quantity = double.Parse(workSheet.Cells[i, 3].Value.ToString()),
                                        Weight = double.Parse(workSheet.Cells[i, 4].Value.ToString()),
                                        StatusCode = workSheet.Cells[i, 5].Value.ToString(),
                                    });
                                }
                            }
                        }
                    }
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
                    {
                        TopProSystemEntities context = null;
                        try
                        {
                            context = new TopProSystemEntities();
                            context.Configuration.AutoDetectChangesEnabled = false;
                            int count = 0;
                            foreach (var entityToInsert in _ListItemMaterial)
                            {
                                ++count;
                                context = AddToContextForInspectionItemNoMaterial(context, entityToInsert, count, 100, true);
                            }
                            context.SaveChanges();
                        }
                        finally
                        {
                            if (context != null)
                                context.Dispose();
                        }
                        scope.Complete();
                    }
                    return RedirectToAction("GetRawMaterialWarehousingEntry", "RawMaterial");
            }
        }
        public double ToDouble(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }
            return Double.Parse(input);
        }
        private TopProSystemEntities AddToContextForInspectionItemNoMaterial(TopProSystemEntities context, InspectionItemNoMaterial entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<InspectionItemNoMaterial>().Add(entity);
            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new TopProSystemEntities();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }
            return context;
        }
        //public ActionResult UploadExcelLinqToExcel()
        //{
        //    var _ListItemMaterial = new List<InspectionItemNoMaterial>();
        //    if (Request.Files.Count > 0)
        //    {
        //        var file = Request.Files[0];
        //        string filePath = string.Empty;
        //        if (Request.Files != null)
        //        {
        //            string path = Server.MapPath("~/FileCreated/RawMaterial/");
        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }
        //            filePath = path + System.IO.Path.GetFileName("InspectionItemNoMaterial-" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss-ff") + System.IO.Path.GetExtension(file.FileName));
        //            //  string extension = System.IO.Path.GetExtension("InspectionItemNoMaterial-" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss-ff") + System.IO.Path.GetExtension(file.FileName));
        //            file.SaveAs(filePath);
        //            ViewBag.File = filePath;
        //            string sheetName = "PurchaseContractEntry";
        //            var excelFile = new ExcelQueryFactory(filePath);
        //            var artistAlbums = from a in excelFile.Worksheet<InspectionItemNoMaterial>(sheetName) select a;
        //            foreach (var q in artistAlbums)
        //            {
        //                if (!String.IsNullOrEmpty(q.InspectionNo))
        //                {
        //                    InspectionItemNoMaterial a = new InspectionItemNoMaterial();
        //                    a.InspectionNo = q.InspectionNo;
        //                    a.RawMaterialNo = q.RawMaterialNo;
        //                    a.Quantity = q.Quantity;
        //                    a.Weight = q.Weight;
        //                    a.StatusCode = q.StatusCode;
        //                    _ListItemMaterial.Add(a);
        //                }
        //            }
        //        }
        //    }
        //    return PartialView("_ImportExcelItemNo", _ListItemMaterial);
        //}         
        #region Raw Material Stock Entry Result // scaning
        public ActionResult GetRawMaterialWarehousingResult()
        {
            var Ma004_dal = new Areas.MasterSetting.DAL.MA004.MA004_DAL();
            ViewBag.locationCode = Ma004_dal.GetAll();
            return View();
        }
        [HttpPost]
        public ActionResult GetRawMaterialWarehousingResult(FormCollection form, string Command)
        {
            if (form["Inventory"] == null)
            {
                ViewBag.Page = true;
                TempData["location_code"] = form["location_code"].ToString();
                TempData["Date"] = form["Date"].ToString();
                return View();
            }
            else
            {
                string Date;
                string locationcode;
                if (TempData.ContainsKey("Date"))
                    Date = TempData["Date"].ToString();
                if (TempData.ContainsKey("location_code"))
                    locationcode = TempData["location_code"].ToString();
                return View();
            }
        }
        public ActionResult BackToFirstStep()
        {
            return RedirectToAction("GetRawMaterialWarehousingResult");
        }
        public JsonResult CheckInventoryNoExists(string inventory)
        {
            INV001_DAL iNV001_DAL = new INV001_DAL();
            return Json(iNV001_DAL.CheckInventoryNoExists(inventory), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckInspectionScaningExists(string inspectionNo)
        {
            INV001_DAL iNV001_DAL = new INV001_DAL();
            return Json(iNV001_DAL.CheckInspectionScaningExists(inspectionNo), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AjaxHandlerRawMaterialStockEntryResult(Areas.MasterSetting.Models.jQueryDataTableParamModel param)
        {
            var dal = new Areas.MasterSetting.DAL.RM0001.RM0001_DAL();
            int total = dal.GetTotalRecord(param.sSearch);
            IEnumerable<RM0001> rM0001s = dal.GetTotalDisplayRecord(param.sSearch, param.iDisplayStart, param.iDisplayLength);
            var rs = rM0001s.Select(x => new
            {
                ID = x.ID,
                Date = string.Format("{0:dd/MM/yyyy}", x.SERGSDT),
                Location_code = x.SELCTCD,
                Inventory = x.SEINVNO,
                Inspection = x.SEISPNO,
            });
            return Json(new
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = rs
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult InsertRawMaterialStockEntryResult(RM0001 model)
        {
            try
            {
                var dal = new Areas.MasterSetting.DAL.RM0001.RM0001_DAL();
                var msg = dal.Insert(model);
                switch ((int)msg)
                {
                    case 0:
                        return Json(Models.ConstantData.SuccessMessage, JsonRequestBehavior.AllowGet);
                    case 1:
                        return Json(Models.ConstantData.FailMessage, JsonRequestBehavior.AllowGet);
                    case 2:
                        return Json("inv_exists", JsonRequestBehavior.AllowGet);
                    case 3:
                        return Json("ins_exists", JsonRequestBehavior.AllowGet);
                    case 4:
                        return Json("completed", JsonRequestBehavior.AllowGet);
                    default:
                        return Json(Models.ConstantData.FailMessage, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(Models.ConstantData.FailMessage, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete_RawMaterialStockEntryResult(string _array)
        {
            var array = _array.Split('|');
            var dal = new Areas.MasterSetting.DAL.RM0001.RM0001_DAL();
            foreach (var id in array)
            {
                if (!dal.Delete(int.Parse(id)))
                {
                    return Json(Models.ConstantData.FailMessage, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(Models.ConstantData.SuccessMessage, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConfirmStockEntry()
        {
            var dal = new Areas.MasterSetting.DAL.RM0001.RM0001_DAL();
            dal.UpdateSomthingBeforeInsert();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Register Purchase Amount
        public ViewResult RegisterPurchaseAmount()
        {
            return View();
        }
        public ActionResult UpdateRegisterPurchaseAmount(string purchaseNo, double itemNo, decimal purchasePrice)
        {
            var dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            var data = dal.GetINV001byPurchaseNoandItemNo(purchaseNo, itemNo);
            if (data == null)
            {
                return Json(AnounceResult.nodatafound, JsonRequestBehavior.AllowGet);
            }
            using (var scope = new TransactionScope())
            {
                if (dal.UpdateRegisterPurchaseAmount(purchaseNo, itemNo.ToString(), purchasePrice))
                {
                    scope.Complete();
                    return Json(AnounceResult.success, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    scope.Dispose();
                    return Json(AnounceResult.error, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpPost]
        public ActionResult SearchINV001(string CACTRNO, double? CACTITM)
        {
            INV001_DAL Dal = new INV001_DAL();
            var INV001 = Dal.GetINV001byPurchaseNoandItemNoSearch(CACTRNO, CACTITM);
            if (INV001 == null)
            {
                return Json(false);
            }
            return Json(INV001);
        }
        [HttpPost]
        public ActionResult AjaxHandlerRegisterPurchaseAmount(jQueryDataTablePurchase param, string CACTRNO, double? CACTITM, double? CAPRUP)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordRegisterPurchaseAmount(CACTRNO, CACTITM);
            var displayItem = Dal.GetTotalDisplayRecordRegisterPurchaseAmount(CACTRNO, CACTITM, param.start, param.length);
            var ma001_dal = new Areas.MasterSetting.DAL.MA001.MA001_DAL();
            var totalWeight = Dal.sumWeight(CACTRNO, CACTITM);
            var totalQty = Dal.sumQty(CACTRNO, CACTITM);
            var result = displayItem.Select(x => new
            {
                CAINVNO = x.CAINVNO,
                CAMCSPC = x.CAMCSPC,
                CABSZT = x.CABSZT,
                CABSZW = x.CABSZW,
                CABSZL = x.CABSZL,
                CAPRDNM = x.CAPRDNM,
                CAPRDDIA = x.CAPRDDIA,
                CASTLGR = x.CASTLGR,
                CAQTY = x.CAQTY,
                CAWT = x.CAWT,
                CAPRUP = CAPRUP,
                CAPRUPD = Rouding(CAPRUP * x.CAEXRT),
                CAPURAT = Rouding(CAPRUP * x.CAWT),
                CAPURATD = Rouding(CAPRUP * x.CAEXRT * x.CAWT),
                CAPTXAT = Rouding(x.CATXRT / 100 * (CAPRUP * x.CAWT)),
                CAPTXAD = Rouding(x.CATXRT / 100 * (CAPRUP * x.CAEXRT * x.CAWT)),
            });
            return Json(new
            {
                recordsTotal = countRecord,
                recordsFiltered = countRecord,
                draw = param.draw,
                data = result,
                TotalQty = totalQty,
                TotalWeight = totalWeight,
            }, JsonRequestBehavior.AllowGet);
        }
        public double Rouding(double? number)
        {
            return Math.Round(number.GetValueOrDefault(), 2);
        }
        #endregion
        #region Raw material receiving list
        public ActionResult GetPrintRawMaterialRecevingList()
        {
            return View();
        }
        // ajax table
        [HttpPost]
        public JsonResult AjaxhandlerReceivingList(DateTime? StockEntryDate, string VesselName, string Status, jQueryDataTablePurchase param)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordByStockEntryDateAndVesselName(StockEntryDate, VesselName, Status);
            var displayItem = Dal.GetTotalDisplayRecordByStockEntryDateAndVesselName(StockEntryDate, VesselName, Status, param.start, param.length);
            // var allItem = Dal.GetTotalDisplayRecordByStockEntryDateAndVesselName(StockEntryDate, VesselName, Status, 0, 0, false);
            var totalCAQTY = Dal.totalQtybyStockandVessl(StockEntryDate, VesselName, Status);
            var totalCAWT = Dal.totalWtbyStockandVessl(StockEntryDate, VesselName, Status);
            var paging = displayItem.Select(x => new
            {
                CASEDT = x.CASEDT.GetValueOrDefault().ToString("dd/MM/yyyy"),
                CACMDCD = x.CACMDCD,
                CASPEC = x.CASPEC,
                CABSZT = x.CABSZT,
                CABSZW = x.CABSZW,
                CABSZL = x.CABSZL,
                CAPRDNM = x.CAPRDNM,
                CAPRDDIA = x.CAPRDDIA,
                CAVESEL = x.CAVESEL,
                CAINVNO = x.CAINVNO,
                CAISPNO = x.CAISPNO,
                CAORDNO = x.CAORDNO,// NOTE
                CAQTY = x.CAQTY,
                CAWT = x.CAWT,
                CALCTCD = x.CALCTCD
            }).ToList();
            return Json(new
            {
                recordsTotal = countRecord,
                recordsFiltered = countRecord,
                data = paging,
                totalCAQTY = totalCAQTY.GetValueOrDefault(),
                totalCAWT = Math.Round(totalCAWT.GetValueOrDefault(), 2),
            });
        }
        //confirm 
        [HttpPost]
        public ActionResult PrinPdfRawMaterialReceivingList(DateTime? StockEntryDate, string VesselName, string Status)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordByStockEntryDateAndVesselName(StockEntryDate, VesselName, Status);
            if (countRecord == 0)
            {
                return Json(new { result = "0" });
            }
            var allItem = Dal.GetTotalDisplayRecordByStockEntryDateAndVesselName(StockEntryDate, VesselName, Status, 0, 0, false);
            var totalqty = Dal.totalQtybyStockandVessl(StockEntryDate, VesselName, Status).GetValueOrDefault();
            var totalwt = Math.Round((Dal.totalWtbyStockandVessl(StockEntryDate, VesselName, Status)).GetValueOrDefault(), 2);
            string path = CreatePrintPdfRawMaterialRecevingList(allItem, totalqty, totalwt);
            return Json(new { result = "1", stringpath = path });
        }
        //pdf method
        public string CreatePrintPdfRawMaterialRecevingList(IEnumerable<INV001> ListItem, double totalqty, double totalwt)
        {
            if (!Directory.Exists(RawMaterialPDFFilePath))
            {
                Directory.CreateDirectory(RawMaterialPDFFilePath);
            }
            string filePath = RawMaterialPDFFilePath + System.IO.Path.GetFileName("RawMaterialRecevingList_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-ff") + ".pdf");
            Document doc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 0);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();
            #region header
            PdfPTable tableheader = new PdfPTable(2);
            tableheader.SetWidths(new float[] { 70f, 30f });
            tableheader.DefaultCell.Border = Rectangle.NO_BORDER;
            tableheader.WidthPercentage = 100;
            PdfPTable headerlefttable = new PdfPTable(2);
            headerlefttable.DefaultCell.Border = Rectangle.NO_BORDER;
            headerlefttable.SetWidths(new float[] { 15, 85 });
            #region Logo
            string imageURL = Server.MapPath("~/Images/PdfFileImg/Logo.png");
            iTextSharp.text.Image jpglogo = iTextSharp.text.Image.GetInstance(imageURL);
            //Resize image depend upon your need
            jpglogo.ScaleToFit(70f, 70f);
            //Give space before image
            jpglogo.Alignment = Element.ALIGN_LEFT;
            #endregion
            PdfPCell celllogoimg = new PdfPCell();
            celllogoimg.Border = 0;
            celllogoimg.AddElement(jpglogo);
            headerlefttable.AddCell(celllogoimg);
            var FontColor = new BaseColor(0, 168, 89);
            var baseFont = BaseFont.CreateFont(Server.MapPath("~/Font/RobotoCondensed-Regular.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font FontCompany = new Font(baseFont, 23, Font.BOLD, FontColor);
            PdfPCell celltitlecompany = new PdfPCell();
            celltitlecompany = new PdfPCell(new Paragraph("Formosa Gear Machine Co., Ltd", FontCompany));
            celltitlecompany.VerticalAlignment = Element.ALIGN_MIDDLE;
            celltitlecompany.HorizontalAlignment = 0;
            celltitlecompany.Border = 0;
            headerlefttable.AddCell(celltitlecompany);
            Font fontaddress = new Font(baseFont, 12, Font.NORMAL, BaseColor.BLACK);
            PdfPCell cellAddressAndPhone = new PdfPCell();
            cellAddressAndPhone = new PdfPCell(new Paragraph("My Xuan A2 Industrial Zone, Tan Thanh District, Ba Ria - Vung Tau Province", fontaddress));
            cellAddressAndPhone.HorizontalAlignment = Element.ALIGN_LEFT;
            cellAddressAndPhone.Colspan = 2;
            cellAddressAndPhone.Border = 0;
            headerlefttable.AddCell(cellAddressAndPhone);
            cellAddressAndPhone.Phrase = new Paragraph("Tel: +84 239 3722 123 Fax: 84 239 3722 112", fontaddress);
            headerlefttable.AddCell(cellAddressAndPhone);
            tableheader.AddCell(headerlefttable);
            PdfPTable headerrighttable = new PdfPTable(1);
            headerrighttable.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPCell celldate = new PdfPCell();
            celldate = new PdfPCell(new Paragraph("Date: " + DateTime.Now.ToString("dd/MM/yyyy"), fontaddress));
            celldate.HorizontalAlignment = Element.ALIGN_RIGHT;
            celldate.Border = 0;
            headerrighttable.AddCell(celldate);
            PdfPCell cellpage = new PdfPCell();
            cellpage = new PdfPCell(new Paragraph("Page: XX/XX", fontaddress));
            cellpage.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellpage.Border = 0;
            headerrighttable.AddCell(cellpage);
            PdfPTable tableSignatures = new PdfPTable(5);
            tableSignatures.SpacingBefore = 5f;
            tableSignatures.SetWidths(new float[] { 30f, 3f, 30f, 3f, 30f });
            tableSignatures.WidthPercentage = 100;
            tableSignatures.DefaultCell.Border = Rectangle.NO_BORDER;
            PdfPCell cellSignatures1 = new PdfPCell();
            cellSignatures1.FixedHeight = 40f;
            tableSignatures.AddCell(cellSignatures1);
            tableSignatures.AddCell(new Paragraph(" "));
            tableSignatures.AddCell(cellSignatures1);
            tableSignatures.AddCell(new Paragraph(" "));
            tableSignatures.AddCell(cellSignatures1);
            headerrighttable.AddCell(tableSignatures);
            PdfPCell cellSignaturestext = new PdfPCell();
            cellSignaturestext = new PdfPCell(new Paragraph("Signatures", fontaddress));
            cellSignaturestext.HorizontalAlignment = Element.ALIGN_CENTER;
            cellSignaturestext.Border = 0;
            headerrighttable.AddCell(cellSignaturestext);
            tableheader.AddCell(headerrighttable);
            doc.Add(tableheader);
            #endregion
            #region title
            Font FontTitle = new Font(baseFont, 20, Font.BOLD, BaseColor.BLACK);
            Paragraph paragraphtitle = new Paragraph("Raw Material Receving List", FontTitle);
            paragraphtitle.Alignment = Element.ALIGN_CENTER;
            paragraphtitle.SpacingBefore = 5f;
            doc.Add(paragraphtitle);
            #endregion
            #region body
            #region tbody
            PdfPTable tablebody = CreatePdfPTableReceving(new PdfPTable(15), 20f);
            List<string> columnname = new List<string>() { "Stock Entry Date", "Commodity Code", "Spec", "Thick", "Width", "Length", "Product Name", "Product Diameter", "Vessel", " Inventory No", " PO No", "Inspection No", "Qty", "Wt", "Location" };
            PdfPCell cellcolumnnametitle = new PdfPCell();
            cellcolumnnametitle.BackgroundColor = new BaseColor(216, 216, 216);
            cellcolumnnametitle.HorizontalAlignment = 1;
            cellcolumnnametitle.FixedHeight = 35f;
            cellcolumnnametitle.VerticalAlignment = Element.ALIGN_MIDDLE;
            PdfPCell cellcolumnname = new PdfPCell();
            cellcolumnname.HorizontalAlignment = 1;
            cellcolumnname.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellcolumnname.FixedHeight = 18f;
            cellcolumnname.BackgroundColor = new BaseColor(255, 255, 255);
            var fonttable = new Font(baseFont, 11, Font.NORMAL, BaseColor.BLACK);
            foreach (string item in columnname)
            {
                cellcolumnnametitle.Phrase = new Paragraph(item, fonttable);
                tablebody.AddCell(cellcolumnnametitle);
            }
            foreach (var item in ListItem.Take(20))
            {
                AddItemOnTable(tablebody, item, cellcolumnname, fonttable);
            }
            doc.Add(tablebody);
            tablebody = CreatePdfPTableReceving(tablebody, 20f);
            int i = 0;
            foreach (var item in ListItem.Skip(20))
            {
                if (i % 30 == 0)
                {
                    if (i != 0)
                    {
                        doc.Add(tablebody);
                        tablebody = CreatePdfPTableReceving(tablebody, 20f);
                    }
                    doc.NewPage();
                    foreach (string itemtitle in columnname)
                    {
                        cellcolumnnametitle.Phrase = new Paragraph(itemtitle, fonttable);
                        tablebody.AddCell(cellcolumnnametitle);
                    }
                }
                AddItemOnTable(tablebody, item, cellcolumnname, fonttable);
                i++;
            }
            doc.Add(tablebody);
            #endregion
            #region tfoot
            PdfPTable tablebodytotal = new PdfPTable(5);
            tablebodytotal.SetWidths(new float[] { 76f, 6f, 5f, 7f, 6f });
            tablebodytotal.WidthPercentage = 100;
            tablebodytotal.SpacingBefore = 17f;
            //set colspan 11 row total
            cellcolumnname = new PdfPCell(new Phrase(""));
            cellcolumnname.HorizontalAlignment = 1;
            cellcolumnname.Border = 0;
            cellcolumnname.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellcolumnname.FixedHeight = 18f;
            tablebodytotal.AddCell(cellcolumnname);
            //title total
            cellcolumnname = new PdfPCell(new Paragraph("Total", fontaddress));
            cellcolumnname.HorizontalAlignment = 1;
            cellcolumnname.BackgroundColor = new BaseColor(216, 216, 216);
            cellcolumnname.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellcolumnname.FixedHeight = 18f;
            tablebodytotal.AddCell(cellcolumnname);
            // sum qty
            cellcolumnname = new PdfPCell(new Paragraph(totalqty.ToString(), fontaddress));
            cellcolumnname.HorizontalAlignment = 1;
            cellcolumnname.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellcolumnname.FixedHeight = 18f;
            tablebodytotal.AddCell(cellcolumnname);
            //sum wt
            cellcolumnname = new PdfPCell(new Paragraph(totalwt.ToString(), fontaddress));
            cellcolumnname.HorizontalAlignment = 1;
            cellcolumnname.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellcolumnname.FixedHeight = 18f;
            tablebodytotal.AddCell(cellcolumnname);
            //column finish
            cellcolumnname = new PdfPCell();
            cellcolumnname.HorizontalAlignment = 1;
            cellcolumnname.Border = 0;
            cellcolumnname.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellcolumnname.FixedHeight = 18f;
            tablebodytotal.AddCell(cellcolumnname);
            #endregion
            doc.Add(tablebodytotal);
            #endregion
            doc.Close();
            return filePath;
        }
        //excel
        [HttpPost]
        public ActionResult ExportExcelRawMaterialReceivingList(DateTime? StockEntryDate, string VesselName, string Status)
        {
            INV001_DAL Dal = new INV001_DAL();
            var countRecord = Dal.GetTotalRecordByStockEntryDateAndVesselName(StockEntryDate, VesselName, Status);
            if (countRecord == 0)
            {
                return Json(new { result = "0" });
            }
            var allItem = Dal.GetTotalDisplayRecordByStockEntryDateAndVesselName(StockEntryDate, VesselName, Status, 0, 0, false);
            string path = CreateExportExcelRawMaterialRecevingList(allItem);
            return Json(new { result = "1", stringpath = path });
        }
        public string CreateExportExcelRawMaterialRecevingList(IEnumerable<INV001> ListItem)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.DefaultRowHeight = 18;
            ws.Cells["A1"].Value = "Stock Entry Date";
            ws.Cells["B1"].Value = "Commodity Code";
            ws.Cells["C1"].Value = "Spec";
            ws.Cells["D1"].Value = "Thickness";
            ws.Cells["E1"].Value = "Width";
            ws.Cells["F1"].Value = "Length";
            ws.Cells["G1"].Value = "Product Name";
            ws.Cells["H1"].Value = "Prod Diameter";
            ws.Cells["I1"].Value = "Vessel";
            ws.Cells["J1"].Value = "Inventory No";
            ws.Cells["K1"].Value = "PO No";
            ws.Cells["L1"].Value = "Inspection No";
            ws.Cells["M1"].Value = "Qty";
            ws.Cells["N1"].Value = "Wt";
            ws.Cells["O1"].Value = "Location";
            using (var range = ws.Cells["A1:O1"])
            {
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(0, 221, 235, 247);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10));
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }
            int rowgroup = 2;
            foreach (var item in ListItem)
            {
                ws.Cells[string.Format("A{0}:O{1}", rowgroup, rowgroup)].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[string.Format("A{0}:O{1}", rowgroup, rowgroup)].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[string.Format("A{0}:O{1}", rowgroup, rowgroup)].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[string.Format("A{0}:O{1}", rowgroup, rowgroup)].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[string.Format("A{0}", rowgroup)].Value = item.CASEDT;
                ws.Cells[string.Format("B{0}", rowgroup)].Value = item.CACMDCD;
                ws.Cells[string.Format("C{0}", rowgroup)].Value = item.CASPEC;
                ws.Cells[string.Format("D{0}", rowgroup)].Value = item.CABSZT;
                ws.Cells[string.Format("E{0}", rowgroup)].Value = item.CABSZW;
                ws.Cells[string.Format("F{0}", rowgroup)].Value = item.CABSZL;
                ws.Cells[string.Format("G{0}", rowgroup)].Value = item.CAPRDNM;
                ws.Cells[string.Format("H{0}", rowgroup)].Value = item.CAPRDDIA;
                ws.Cells[string.Format("I{0}", rowgroup)].Value = item.CAVESEL;
                ws.Cells[string.Format("J{0}", rowgroup)].Value = item.CAINVNO;
                ws.Cells[string.Format("K{0}", rowgroup)].Value = item.CAORDNO;
                ws.Cells[string.Format("L{0}", rowgroup)].Value = item.CAISPNO;
                ws.Cells[string.Format("M{0}", rowgroup)].Value = item.CAQTY;
                ws.Cells[string.Format("N{0}", rowgroup)].Value = item.CAWT;
                ws.Cells[string.Format("0{0}", rowgroup)].Value = item.CALCTCD;
                rowgroup++;
            }
            ws.Cells[rowgroup, 12].Value = "-- Total --";
            ws.Cells[rowgroup, 13].Formula = "=SUM(M2" + ":M" + (rowgroup - 1) + ")";
            ws.Cells[rowgroup, 14].Formula = "=SUM(N2" + ":N" + (rowgroup - 1) + ")";
            ws.Cells["A:AZ"].AutoFitColumns();
            if (!Directory.Exists(SaveExcelURL))
            {
                Directory.CreateDirectory(SaveExcelURL);
            }
            string pathToExcelFile = SaveExcelURL + "RawMaterialReceiving_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-ff") + ".xlsx";
            FileInfo fi = new FileInfo(pathToExcelFile);
            pck.SaveAs(fi);
            return pathToExcelFile;
        }
        [HttpGet]
        public virtual ActionResult DownloadFileReceiving(string file)
        {
            string FileNameExtension = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
            string extension = System.IO.Path.GetExtension(file);
            if (extension == ".pdf")
            {
                return File(file, "application/pdf", "PdfRawMaterialReceiving_" + FileNameExtension + ".pdf");
            }
            return File(file, "application/vnd.ms-excel", "ExcelRawMaterialReceiving_" + FileNameExtension + ".xlsx");
        }
        public PdfPCell CreateCellForBody(string value, PdfPCell cellcolumnname, Font font)
        {
            // cellcolumnname = new PdfPCell(new Paragraph(value, font));
            cellcolumnname.Phrase = new Paragraph(value, font);
            return cellcolumnname;
        }
        public PdfPTable CreatePdfPTableReceving(PdfPTable tablebody, float SpacingBefore)
        {
            tablebody = new PdfPTable(15);
            tablebody.SetWidths(new float[] { 8f, 7f, 8f, 4f, 4f, 5f, 7f, 8f, 7f, 8f, 7f, 9f, 5f, 7f, 6f });
            tablebody.WidthPercentage = 100;
            tablebody.SpacingBefore = SpacingBefore;
            return tablebody;
        }
        public PdfPTable AddItemOnTable(PdfPTable tablebody, INV001 item, PdfPCell cellcolumnname, Font fonttable)
        {
            tablebody.AddCell(CreateCellForBody(item.CASEDT.GetValueOrDefault().ToString("dd/MM/yyyy"), cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CACMDCD, cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CASPEC, cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CABSZT.ToString(), cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CABSZW.ToString(), cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CABSZL.ToString(), cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CAPRDNM, cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CAPRDDIA, cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CAVESEL, cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CAINVNO, cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CAORDNO, cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CAISPNO, cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CAQTY.ToString(), cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CAWT.ToString(), cellcolumnname, fonttable));
            tablebody.AddCell(CreateCellForBody(item.CALCTCD, cellcolumnname, fonttable));
            return tablebody;
        }
        #endregion
        #region Tuan
        public enum ExportFileResult
        {
            Success = 0,
            Fail = 1,
            Nodata = 2
        }
        [HttpPost]
        public ActionResult AjaxHandler(jQueryDataTableInventory param, InventorySearchModel searchmodel)
        {
            var dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            var countRecord = dal.GetTotalRecord(searchmodel);
            var displayItem = dal.GetTotalDisplayRecord(param, searchmodel);
            var ma001_dal = new Areas.MasterSetting.DAL.MA001.MA001_DAL();
            var ma002_dal = new Areas.MasterSetting.DAL.MA002.MA002_DAL();
            var ma003_dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();
            var result = displayItem.Select(x => new
            {
                CADSTNC = x.CADSTNC, // primary key
                CACTRNO = x.CACTRNO,
                CAMKCD = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE005, x.CAMKCD),
                CASPPNO = x.CASPPNO,
                CASPLCD = ma001_dal.GetSalePurchase(x.CASPLCD) != null ? ma001_dal.GetSalePurchase(x.CASPLCD).MASPNM : string.Empty,
                CACMDCD = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE006, x.CACMDCD),
                CAUSRCD = ma002_dal.GetUserName(x.CAUSRCD),
                CACTRTP = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE025, x.CACTRTP),
                CASMNTH = x.CASMNTH,
                CAIDCD = ma003_dal.GetMA003(x.CAIDCD) != null ? ma003_dal.GetMA003(x.CAIDCD).MCIDNM : string.Empty,
                CACTITM = x.CACTITM
            });
            return Json(new
            {
                recordsTotal = countRecord,
                recordsFiltered = countRecord,
                draw = param.draw,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ViewResult GetRawMaterialWarehousingEntry()
        {
            var dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            return View(dal.GetReferences(new INV001()));
        }
        public ViewResult GetRawMaterialStockEntry_Add(string purContractNo, int? item)
        {
            var _model = MasterdataEntities.PUR001.Where(m => m.AAPURNO == purContractNo && m.ABCTITM == item && m.Deleted != 1).FirstOrDefault();
            ViewBag.Key = "";
            ViewBag.PIC = "";
            ViewBag.StatusCode = "";
            ViewBag.PurUP = "";
            ViewBag.PurUPD = "";
            ViewBag.LastestRaw = 1;
            ViewBag.ListInspection = "";
            try
            {
                if (_model != null)
                {
                    string Comodity = string.IsNullOrEmpty(_model.AACMDCD) ? "" : _model.AACMDCD.Trim().Substring(0, 1);
                    string MarkerCode = string.IsNullOrEmpty(_model.AAMKCD) ? "" : _model.AAMKCD.Trim().Substring(0, 1);
                    string year = DateTime.Now.ToString("yyyy").Substring(2, 2);
                    string month = DateTime.Now.ToString("MM");
                    if (int.Parse(month) < 10)
                    {
                        month = month.Substring(1, 1);
                    }
                    else
                    {
                        if (month == "10")
                        {
                            month = "X";
                        }
                        if (month == "11")
                        {
                            month = "Y";
                        }
                        if (month == "12")
                        {
                            month = "Z";
                        }
                    }
                    string rawCode = Comodity + MarkerCode + year + month;
                    string getList = MasterdataEntities.INV001.Where(m => m.CAINVNO.Contains(rawCode)).OrderByDescending(m => m.CAINVNO.Substring(5)).Select(m => m.CAINVNO).FirstOrDefault();
                    if (!string.IsNullOrEmpty(getList))
                    {
                        getList = getList.Substring(5);
                        int newResult = int.TryParse(getList, out newResult) ? newResult : 0;
                        ViewBag.LastestRaw = newResult + 1;
                    }
                    ViewBag.PIC = Session[TopProSystem.Models.ConstantData.SessionUserID];
                    ViewBag.StatusCode = _model.AARMTP;
                    ViewBag.PurUP = _model.ABPRUP;
                    ViewBag.PurUPD = _model.ABPRUPD;
                    var chkInINV = MasterdataEntities.INV001.Where(m => m.CACTRNO == _model.AAPURNO && m.CACTITM == _model.ABCTITM).FirstOrDefault();
                    if (chkInINV != null)
                    {
                        ViewBag.Key = chkInINV.CADSTNC;
                        ViewBag.PIC = chkInINV.CAIDCD;
                        string[] lstInspec = MasterdataEntities.INV001.Where(m => m.CADSTNC != chkInINV.CADSTNC).Select(m => m.CAISPNO).ToArray();
                        ViewBag.ListInspection = lstInspec;
                    }
                    else
                    {
                        string[] lstInspec = MasterdataEntities.INV001.Select(m => m.CAISPNO).ToArray();
                        ViewBag.ListInspection = lstInspec;
                    }
                    return View(_model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mess = ex.Message;
                return View(_model);
            }
            return View(_model);
        }
        [HttpPost]
        public ActionResult GetRawMaterialStockEntry_Add(PUR001 model, FormCollection collection)
        {
            int totalExcelRecord = collection["totalExcelRecord"] == null ? 0 : int.Parse(collection["totalExcelRecord"].ToString());
            string codeforIVPUR = CreateCode();
            if (totalExcelRecord > 0)
            {
                #region DataTBINV001 INSERT
                var tableINV = new DataTable();
                tableINV.Columns.Add("CADSTNC", typeof(string));
                tableINV.Columns.Add("CACTRNO", typeof(string));
                tableINV.Columns.Add("CACTITM", typeof(float));
                tableINV.Columns.Add("CASEDT", typeof(DateTime));
                tableINV.Columns.Add("CAPIVNO", typeof(string));
                tableINV.Columns.Add("CAPIVDT", typeof(DateTime));
                tableINV.Columns.Add("CAVESEL", typeof(string));
                tableINV.Columns.Add("CAMKCD", typeof(string));
                tableINV.Columns.Add("CASPPNO", typeof(string));
                tableINV.Columns.Add("CASPLCD", typeof(string));
                tableINV.Columns.Add("CAUSRCD", typeof(string));
                tableINV.Columns.Add("CACRRCD", typeof(string));
                tableINV.Columns.Add("CATXCD", typeof(string));
                tableINV.Columns.Add("CASMNTH", typeof(string));
                tableINV.Columns.Add("CACMDCD", typeof(string));
                tableINV.Columns.Add("CACTRTP", typeof(string));
                tableINV.Columns.Add("CAIDCD", typeof(string));
                tableINV.Columns.Add("CAEXRTT", typeof(string));
                tableINV.Columns.Add("CAEXRT", typeof(float));
                tableINV.Columns.Add("CATEXRT", typeof(string));
                tableINV.Columns.Add("CATXEXR", typeof(float));
                tableINV.Columns.Add("CATXRT", typeof(float));
                tableINV.Columns.Add("CAMCSPC", typeof(string));
                tableINV.Columns.Add("CASPEC", typeof(string));
                tableINV.Columns.Add("CACOAT", typeof(string));
                tableINV.Columns.Add("CABSZT", typeof(float));
                tableINV.Columns.Add("CABSZW", typeof(string));
                tableINV.Columns.Add("CABSZL", typeof(string));
                tableINV.Columns.Add("CAPRDNM", typeof(string));
                tableINV.Columns.Add("CAPRDDIA", typeof(string));
                tableINV.Columns.Add("CASTLGR", typeof(string));
                tableINV.Columns.Add("CAORDNO", typeof(string));
                tableINV.Columns.Add("CAISPNO", typeof(string));
                tableINV.Columns.Add("CAINVNO", typeof(string));
                tableINV.Columns.Add("CAQTY", typeof(float));
                tableINV.Columns.Add("CAWT", typeof(float));
                tableINV.Columns.Add("CASTSCD", typeof(string));
                tableINV.Columns.Add("CAPRUP", typeof(float));
                tableINV.Columns.Add("CAPRUPD", typeof(float));
                tableINV.Columns.Add("CAPURAT", typeof(float));
                tableINV.Columns.Add("CAPURATD", typeof(float));
                tableINV.Columns.Add("CAPTXAT", typeof(float));
                tableINV.Columns.Add("CAPTXATD", typeof(float));
                tableINV.Columns.Add("CARGSDT", typeof(DateTime));
                tableINV.Columns.Add("CARGSTM", typeof(string));
                tableINV.Columns.Add("CAINVTP", typeof(string));
                tableINV.Columns.Add("CAINVST", typeof(string));
                #endregion
                #region DataTBINV001 UPDATE
                var tableUpdateINV = new DataTable();
                tableUpdateINV.Columns.Add("CADSTNC", typeof(string));
                tableUpdateINV.Columns.Add("CACTRNO", typeof(string));
                tableUpdateINV.Columns.Add("CACTITM", typeof(float));
                tableUpdateINV.Columns.Add("CASEDT", typeof(DateTime));
                tableUpdateINV.Columns.Add("CAPIVNO", typeof(string));
                tableUpdateINV.Columns.Add("CAPIVDT", typeof(DateTime));
                tableUpdateINV.Columns.Add("CAVESEL", typeof(string));
                tableUpdateINV.Columns.Add("CAMKCD", typeof(string));
                tableUpdateINV.Columns.Add("CASPPNO", typeof(string));
                tableUpdateINV.Columns.Add("CASPLCD", typeof(string));
                tableUpdateINV.Columns.Add("CAUSRCD", typeof(string));
                tableUpdateINV.Columns.Add("CACRRCD", typeof(string));
                tableUpdateINV.Columns.Add("CATXCD", typeof(string));
                tableUpdateINV.Columns.Add("CASMNTH", typeof(string));
                tableUpdateINV.Columns.Add("CACMDCD", typeof(string));
                tableUpdateINV.Columns.Add("CACTRTP", typeof(string));
                tableUpdateINV.Columns.Add("CAIDCD", typeof(string));
                tableUpdateINV.Columns.Add("CAEXRTT", typeof(string));
                tableUpdateINV.Columns.Add("CAEXRT", typeof(float));
                tableUpdateINV.Columns.Add("CATEXRT", typeof(string));
                tableUpdateINV.Columns.Add("CATXEXR", typeof(float));
                tableUpdateINV.Columns.Add("CATXRT", typeof(float));
                tableUpdateINV.Columns.Add("CAMCSPC", typeof(string));
                tableUpdateINV.Columns.Add("CASPEC", typeof(string));
                tableUpdateINV.Columns.Add("CACOAT", typeof(string));
                tableUpdateINV.Columns.Add("CABSZT", typeof(float));
                tableUpdateINV.Columns.Add("CABSZW", typeof(string));
                tableUpdateINV.Columns.Add("CABSZL", typeof(string));
                tableUpdateINV.Columns.Add("CAPRDNM", typeof(string));
                tableUpdateINV.Columns.Add("CAPRDDIA", typeof(string));
                tableUpdateINV.Columns.Add("CASTLGR", typeof(string));
                tableUpdateINV.Columns.Add("CAORDNO", typeof(string));
                tableUpdateINV.Columns.Add("CAISPNO", typeof(string));
                tableUpdateINV.Columns.Add("CAINVNO", typeof(string));
                tableUpdateINV.Columns.Add("CAQTY", typeof(float));
                tableUpdateINV.Columns.Add("CAWT", typeof(float));
                tableUpdateINV.Columns.Add("CASTSCD", typeof(string));
                tableUpdateINV.Columns.Add("CAPRUP", typeof(float));
                tableUpdateINV.Columns.Add("CAPRUPD", typeof(float));
                tableUpdateINV.Columns.Add("CAPURAT", typeof(float));
                tableUpdateINV.Columns.Add("CAPURATD", typeof(float));
                tableUpdateINV.Columns.Add("CAPTXAT", typeof(float));
                tableUpdateINV.Columns.Add("CAPTXATD", typeof(float));
                tableUpdateINV.Columns.Add("CARGSDT", typeof(DateTime));
                tableUpdateINV.Columns.Add("CARGSTM", typeof(string));
                tableUpdateINV.Columns.Add("CAINVTP", typeof(string));
                #endregion
                #region DATATBTRA001 INSERT
                var tableTRA = new DataTable();
                tableTRA.Columns.Add("DADSTNC", typeof(string));
                tableTRA.Columns.Add("DACTRNO", typeof(string));
                tableTRA.Columns.Add("DACTITM", typeof(float));
                tableTRA.Columns.Add("DASEDT", typeof(DateTime));
                tableTRA.Columns.Add("DAPIVNO", typeof(string));
                tableTRA.Columns.Add("DAPIVDT", typeof(DateTime));
                tableTRA.Columns.Add("DAVESEL", typeof(string));
                tableTRA.Columns.Add("DAMKCD", typeof(string));
                tableTRA.Columns.Add("DASPPNO", typeof(string));
                tableTRA.Columns.Add("DASPLCD", typeof(string));
                tableTRA.Columns.Add("DAUSRCD", typeof(string));
                tableTRA.Columns.Add("DACRRCD", typeof(string));
                tableTRA.Columns.Add("DATXCD", typeof(string));
                tableTRA.Columns.Add("DASMNTH", typeof(string));
                tableTRA.Columns.Add("DACMDCD", typeof(string));
                tableTRA.Columns.Add("DACTRTP", typeof(string));
                tableTRA.Columns.Add("DAIDCD", typeof(string));
                tableTRA.Columns.Add("DAEXRTT", typeof(string));
                tableTRA.Columns.Add("DAEXRT", typeof(float));
                tableTRA.Columns.Add("DATEXRT", typeof(string));
                tableTRA.Columns.Add("DATXEXR", typeof(float));
                tableTRA.Columns.Add("DATXRT", typeof(float));
                tableTRA.Columns.Add("DAMCSPC", typeof(string));
                tableTRA.Columns.Add("DASPEC", typeof(string));
                tableTRA.Columns.Add("DACOAT", typeof(string));
                tableTRA.Columns.Add("DABSZT", typeof(float));
                tableTRA.Columns.Add("DABSZW", typeof(string));
                tableTRA.Columns.Add("DABSZL", typeof(string));
                tableTRA.Columns.Add("DAPRDNM", typeof(string));
                tableTRA.Columns.Add("DAPRDDIA", typeof(string));
                tableTRA.Columns.Add("DASTLGR", typeof(string));
                tableTRA.Columns.Add("DAORDNO", typeof(string));
                tableTRA.Columns.Add("DAISPNO", typeof(string));
                tableTRA.Columns.Add("DAINVNO", typeof(string));
                tableTRA.Columns.Add("DAQTY", typeof(float));
                tableTRA.Columns.Add("DAWT", typeof(float));
                tableTRA.Columns.Add("DASTSCD", typeof(string));
                tableTRA.Columns.Add("DAPRUP", typeof(float));
                tableTRA.Columns.Add("DAPRUPD", typeof(float));
                tableTRA.Columns.Add("DAPURAT", typeof(float));
                tableTRA.Columns.Add("DAPURATD", typeof(float));
                tableTRA.Columns.Add("DAPTXAT", typeof(float));
                tableTRA.Columns.Add("DAPTXATD", typeof(float));
                tableTRA.Columns.Add("DAUPDT", typeof(DateTime));
                tableTRA.Columns.Add("DAUPDTM", typeof(string));
                tableTRA.Columns.Add("DATRNDT", typeof(DateTime));
                tableTRA.Columns.Add("DALOGDT", typeof(DateTime));
                tableTRA.Columns.Add("DALOGTP", typeof(string));
                tableTRA.Columns.Add("DAINVTP", typeof(string));
                #endregion
                #region DATATBTRA001 UPDATE
                var tableUPDATETRA = new DataTable();
                tableUPDATETRA.Columns.Add("DADSTNC", typeof(string));
                tableUPDATETRA.Columns.Add("DACTRNO", typeof(string));
                tableUPDATETRA.Columns.Add("DACTITM", typeof(float));
                tableUPDATETRA.Columns.Add("DASEDT", typeof(DateTime));
                tableUPDATETRA.Columns.Add("DAPIVNO", typeof(string));
                tableUPDATETRA.Columns.Add("DAPIVDT", typeof(DateTime));
                tableUPDATETRA.Columns.Add("DAVESEL", typeof(string));
                tableUPDATETRA.Columns.Add("DAMKCD", typeof(string));
                tableUPDATETRA.Columns.Add("DASPPNO", typeof(string));
                tableUPDATETRA.Columns.Add("DASPLCD", typeof(string));
                tableUPDATETRA.Columns.Add("DAUSRCD", typeof(string));
                tableUPDATETRA.Columns.Add("DACRRCD", typeof(string));
                tableUPDATETRA.Columns.Add("DATXCD", typeof(string));
                tableUPDATETRA.Columns.Add("DASMNTH", typeof(string));
                tableUPDATETRA.Columns.Add("DACMDCD", typeof(string));
                tableUPDATETRA.Columns.Add("DACTRTP", typeof(string));
                tableUPDATETRA.Columns.Add("DAIDCD", typeof(string));
                tableUPDATETRA.Columns.Add("DAEXRTT", typeof(string));
                tableUPDATETRA.Columns.Add("DAEXRT", typeof(float));
                tableUPDATETRA.Columns.Add("DATEXRT", typeof(string));
                tableUPDATETRA.Columns.Add("DATXEXR", typeof(float));
                tableUPDATETRA.Columns.Add("DATXRT", typeof(float));
                tableUPDATETRA.Columns.Add("DAMCSPC", typeof(string));
                tableUPDATETRA.Columns.Add("DASPEC", typeof(string));
                tableUPDATETRA.Columns.Add("DACOAT", typeof(string));
                tableUPDATETRA.Columns.Add("DABSZT", typeof(float));
                tableUPDATETRA.Columns.Add("DABSZW", typeof(string));
                tableUPDATETRA.Columns.Add("DABSZL", typeof(string));
                tableUPDATETRA.Columns.Add("DAPRDNM", typeof(string));
                tableUPDATETRA.Columns.Add("DAPRDDIA", typeof(string));
                tableUPDATETRA.Columns.Add("DASTLGR", typeof(string));
                tableUPDATETRA.Columns.Add("DAORDNO", typeof(string));
                tableUPDATETRA.Columns.Add("DAISPNO", typeof(string));
                tableUPDATETRA.Columns.Add("DAINVNO", typeof(string));
                tableUPDATETRA.Columns.Add("DAQTY", typeof(float));
                tableUPDATETRA.Columns.Add("DAWT", typeof(float));
                tableUPDATETRA.Columns.Add("DASTSCD", typeof(string));
                tableUPDATETRA.Columns.Add("DAPRUP", typeof(float));
                tableUPDATETRA.Columns.Add("DAPRUPD", typeof(float));
                tableUPDATETRA.Columns.Add("DAPURAT", typeof(float));
                tableUPDATETRA.Columns.Add("DAPURATD", typeof(float));
                tableUPDATETRA.Columns.Add("DAPTXAT", typeof(float));
                tableUPDATETRA.Columns.Add("DAPTXATD", typeof(float));
                tableUPDATETRA.Columns.Add("DAUPDT", typeof(DateTime));
                tableUPDATETRA.Columns.Add("DAUPDTM", typeof(string));
                tableUPDATETRA.Columns.Add("DATRNDT", typeof(DateTime));
                tableUPDATETRA.Columns.Add("DALOGDT", typeof(DateTime));
                tableUPDATETRA.Columns.Add("DAINVTP", typeof(string));
                #endregion
                #region Comewf
                for (int i = 1; i < totalExcelRecord; i++)
                {
                    string chkExisted = collection["chkExisted_" + i] == null ? "" : collection["chkExisted_" + i].ToString();
                    string inspecNo = collection["inspecNo_" + i] == null ? "" : collection["inspecNo_" + i].ToString();
                    string rawMatNo = collection["rawMatNo_" + i] == null ? "" : collection["rawMatNo_" + i].ToString();
                    double? quantity = collection["quantity_" + i] == null ? 0 : double.Parse(collection["quantity_" + i].ToString());
                    double? weight = collection["weight_" + i] == null ? 0 : double.Parse(collection["weight_" + i].ToString());
                    string statusCode = collection["statusCode_" + i] == null ? "" : collection["statusCode_" + i].ToString();
                    double? purUP = collection["purUP_" + i] == null ? 0 : Math.Round(double.Parse(collection["purUP_" + i].ToString()), 2);
                    double? purUPD = collection["purUPD_" + i] == null ? 0 : Math.Round(double.Parse(collection["purUPD_" + i].ToString()), 2);
                    double? purA = collection["purA_" + i] == null ? 0 : Math.Round(double.Parse(collection["purA_" + i].ToString()), 2);
                    double? purAD = collection["purAD_" + i] == null ? 0 : Math.Round(double.Parse(collection["purAD_" + i].ToString()), 2);
                    double? txA = collection["txA_" + i] == null ? 0 : Math.Round(double.Parse(collection["txA_" + i].ToString()), 2);
                    double? txAD = collection["txAD_" + i] == null ? 0 : Math.Round(double.Parse(collection["txAD_" + i].ToString()), 2);
                    if (chkExisted.Contains("1"))
                    {
                        #region Save to INV
                        tableUpdateINV.Rows.Add
                         (
                         codeforIVPUR,
                         model.AAPURNO,
                         model.ABCTITM,
                         model.ABSEDT,
                         model.ABPIVNO,
                         model.ABPIVDT,
                         model.ABVESEL,
                         model.AAMKCD,
                         model.AASPPNO,
                         model.AASPLCD,
                         model.AAUSRCD,
                         model.AACRRCD,
                         model.AATXCD,
                         model.AASHPDT,
                         model.AACMDCD,
                         model.AACTRTP,
                         model.AAIDCD,
                         model.AAEXRTT,
                         model.AAEXRT,
                         model.AATEXRT,
                         model.AATXEXR,
                         model.AATXRT,
                         model.ABMCSPC,
                         model.ABMCSPC,
                         model.ABCOAT,
                         model.ABBSZT,
                         model.ABBSZW,
                         model.ABBSZL,
                         model.ABPRDNM,
                         model.ABPRDDIA,
                         model.RAPSTLGR,
                         model.ABORDNO,
                         inspecNo,
                         rawMatNo,
                         quantity,
                         weight,
                         statusCode,
                         purUP,
                         purUPD,
                         purA,
                         purAD,
                         txA,
                         txAD,
                         DateTime.Now,
                         DateTime.Now.ToString("HH:mm"),
                         "M"
                         );
                        #endregion
                        #region save to TRA001
                        tableUPDATETRA.Rows.Add
                        (
                          codeforIVPUR,
                          model.AAPURNO,
                          model.ABCTITM,
                          model.ABSEDT,
                          model.ABPIVNO,
                          model.ABPIVDT,
                          model.ABVESEL,
                          model.AAMKCD,
                          model.AASPPNO,
                          model.AASPLCD,
                          model.AAUSRCD,
                          model.AACRRCD,
                          model.AATXCD,
                          model.AASHPDT,
                          model.AACMDCD,
                          model.AACTRTP,
                          model.AAIDCD,
                          model.AAEXRTT,
                          model.AAEXRT,
                          model.AATEXRT,
                          model.AATXEXR,
                          model.AATXRT,
                          model.ABMCSPC,
                          model.ABMCSPC,
                          model.ABCOAT,
                          model.ABBSZT,
                          model.ABBSZW,
                          model.ABBSZL,
                          model.ABPRDNM,
                          model.ABPRDDIA,
                          model.RAPSTLGR,
                          model.ABORDNO,
                          inspecNo,
                          rawMatNo,
                          quantity,
                          weight,
                          statusCode,
                          purUP,
                          purUPD,
                          purA,
                          purAD,
                          txA,
                          txAD,
                          DateTime.Now,
                          DateTime.Now.ToString("HH:mm"),
                          DateTime.Now,
                          DateTime.Now,
                          "M"
                        );
                        #endregion
                    }
                    else
                    {
                        #region Save to INV
                        tableINV.Rows.Add
                         (
                         codeforIVPUR,
                         model.AAPURNO,
                         model.ABCTITM,
                         model.ABSEDT,
                         model.ABPIVNO,
                         model.ABPIVDT,
                         model.ABVESEL,
                         model.AAMKCD,
                         model.AASPPNO,
                         model.AASPLCD,
                         model.AAUSRCD,
                         model.AACRRCD,
                         model.AATXCD,
                         model.AASHPDT,
                         model.AACMDCD,
                         model.AACTRTP,
                         model.AAIDCD,
                         model.AAEXRTT,
                         model.AAEXRT,
                         model.AATEXRT,
                         model.AATXEXR,
                         model.AATXRT,
                         model.ABMCSPC,
                         model.ABMCSPC,
                         model.ABCOAT,
                         model.ABBSZT,
                         model.ABBSZW,
                         model.ABBSZL,
                         model.ABPRDNM,
                         model.ABPRDDIA,
                         model.RAPSTLGR,
                         model.ABORDNO,
                         inspecNo,
                         rawMatNo,
                         quantity,
                         weight,
                         statusCode,
                         purUP,
                         purUPD,
                         purA,
                         purAD,
                         txA,
                         txAD,
                         DateTime.Now,
                         DateTime.Now.ToString("HH:mm"),
                         "M",
                         "0"
                         );
                        #endregion
                        #region save to TRA001
                        tableTRA.Rows.Add
                        (
                          codeforIVPUR,
                          model.AAPURNO,
                          model.ABCTITM,
                          model.ABSEDT,
                          model.ABPIVNO,
                          model.ABPIVDT,
                          model.ABVESEL,
                          model.AAMKCD,
                          model.AASPPNO,
                          model.AASPLCD,
                          model.AAUSRCD,
                          model.AACRRCD,
                          model.AATXCD,
                          model.AASHPDT,
                          model.AACMDCD,
                          model.AACTRTP,
                          model.AAIDCD,
                          model.AAEXRTT,
                          model.AAEXRT,
                          model.AATEXRT,
                          model.AATXEXR,
                          model.AATXRT,
                          model.ABMCSPC,
                          model.ABMCSPC,
                          model.ABCOAT,
                          model.ABBSZT,
                          model.ABBSZW,
                          model.ABBSZL,
                          model.ABPRDNM,
                          model.ABPRDDIA,
                          model.RAPSTLGR,
                          model.ABORDNO,
                          inspecNo,
                          rawMatNo,
                          quantity,
                          weight,
                          statusCode,
                          purUP,
                          purUPD,
                          purA,
                          purAD,
                          txA,
                          txAD,
                          DateTime.Now,
                          DateTime.Now.ToString("HH:mm"),
                          DateTime.Now,
                          DateTime.Now,
                          "10",
                          "M"
                        );
                        #endregion
                    }
                }
                var modelPUR = MasterdataEntities.PUR001.Where(m => m.ID == model.ID).FirstOrDefault();
                modelPUR.ABSEDT = model.ABSEDT;
                modelPUR.ABPIVNO = model.ABPIVNO;
                modelPUR.ABPIVDT = model.ABPIVDT;
                modelPUR.ABVESEL = model.ABVESEL;
                modelPUR.AASPPNO = model.AASPPNO;
                MasterdataEntities.SaveChanges();
                #region Store
                using (var dc = new TopProSystemEntities())
                {
                    //UPDATE 
                    var sqlParamINV_UPDATE = new SqlParameter[]
                            {
                               new SqlParameter("@ItemList",SqlDbType.Structured)
                               {
                                   TypeName = "dbo.MyListINV001_UPDATE",
                                   Value = tableUpdateINV
                               },
                            };
                    var sqlParamTRA_UPDATE = new SqlParameter[]
                            {
                               new SqlParameter("@ItemList",SqlDbType.Structured)
                               {
                                   TypeName = "dbo.MyListTRA001_UPDATE",
                                   Value = tableUPDATETRA
                               },
                            };
                    dc.Database.ExecuteSqlCommand("EXEC proc_UpdateToINV001 @ItemList ", sqlParamINV_UPDATE);
                    dc.Database.ExecuteSqlCommand("EXEC proc_UpdateToTRA001 @ItemList ", sqlParamTRA_UPDATE);
                    //INSERT
                    var sqlParamINV = new SqlParameter[]
                            {
                               new SqlParameter("@ItemList",SqlDbType.Structured)
                               {
                                   TypeName = "dbo.MyListINV001",
                                   Value = tableINV
                               },
                            };
                    var sqlParamTRA = new SqlParameter[]
                            {
                               new SqlParameter("@ItemList",SqlDbType.Structured)
                               {
                                   TypeName = "dbo.MyListTRA001",
                                   Value = tableTRA
                               },
                            };
                    dc.Database.ExecuteSqlCommand("EXEC proc_InsertToINV001 @ItemList ", sqlParamINV);
                    dc.Database.ExecuteSqlCommand("EXEC proc_InsertToTRA001 @ItemList ", sqlParamTRA);
                }
                #endregion
                #endregion
            }
            return RedirectToAction("GetRawMaterialWarehousingEntry");
        }
        public ViewResult GetRawMaterialStockEntry_Edit(string purContractNo, int? item)
        {
            var _model = MasterdataEntities.PUR001.Where(m => m.AAPURNO == purContractNo && m.ABCTITM == item && m.Deleted != 1).FirstOrDefault();
            ViewBag.Key = "";
            ViewBag.PIC = "";
            ViewBag.StatusCode = "";
            ViewBag.PurUP = "";
            ViewBag.PurUPD = "";
            ViewBag.LastestRaw = 1;
            ViewBag.ListInspection = "";
            try
            {
                if (_model != null)
                {
                    string Comodity = string.IsNullOrEmpty(_model.AACMDCD) ? "" : _model.AACMDCD.Trim().Substring(0, 1);
                    string MarkerCode = string.IsNullOrEmpty(_model.AAMKCD) ? "" : _model.AAMKCD.Trim().Substring(0, 1);
                    string year = DateTime.Now.ToString("yyyy").Substring(2, 2);
                    string month = DateTime.Now.ToString("MM");
                    if (int.Parse(month) < 10)
                    {
                        month = month.Substring(1, 1);
                    }
                    else
                    {
                        if (month == "10")
                        {
                            month = "X";
                        }
                        if (month == "11")
                        {
                            month = "Y";
                        }
                        if (month == "12")
                        {
                            month = "Z";
                        }
                    }
                    string rawCode = Comodity + MarkerCode + year + month;
                    string getList = MasterdataEntities.INV001.Where(m => m.CAINVNO.Contains(rawCode)).OrderByDescending(m => m.CAINVNO.Substring(5)).Select(m => m.CAINVNO).FirstOrDefault();
                    if (!string.IsNullOrEmpty(getList))
                    {
                        getList = getList.Substring(5);
                        int newResult = int.TryParse(getList, out newResult) ? newResult : 0;
                        ViewBag.LastestRaw = newResult + 1;
                    }
                    ViewBag.PIC = Session[TopProSystem.Models.ConstantData.SessionUserID];
                    ViewBag.StatusCode = _model.AARMTP;
                    ViewBag.PurUP = _model.ABPRUP;
                    ViewBag.PurUPD = _model.ABPRUPD;
                    var chkInINV = MasterdataEntities.INV001.Where(m => m.CACTRNO == _model.AAPURNO && m.CACTITM == _model.ABCTITM).FirstOrDefault();
                    if (chkInINV != null)
                    {
                        ViewBag.Key = chkInINV.CADSTNC;
                        ViewBag.PIC = chkInINV.CAIDCD;
                        string[] lstInspec = MasterdataEntities.INV001.Where(m => m.CADSTNC != chkInINV.CADSTNC).Select(m => m.CAISPNO).ToArray();
                        ViewBag.ListInspection = lstInspec;
                    }
                    else
                    {
                        string[] lstInspec = MasterdataEntities.INV001.Select(m => m.CAISPNO).ToArray();
                        ViewBag.ListInspection = lstInspec;
                    }
                    return View(_model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mess = ex.Message;
                return View(_model);
            }
            return View(_model);
        }
        [HttpPost]
        public ActionResult GetRawMaterialStockEntry_Edit(PUR001 model, FormCollection collection)
        {
            int totalExcelRecord = collection["totalExcelRecord"] == null ? 0 : int.Parse(collection["totalExcelRecord"].ToString());
            string codeforIVPUR = CreateCode();
            if (totalExcelRecord > 0)
            {
                for (int i = 1; i < totalExcelRecord; i++)
                {
                    string inspection_no = collection["inspecNo_" + i] == null ? "" : collection["inspecNo_" + i].ToString();
                    var lstInv = MasterdataEntities.INV001.Where(m => m.CAISPNO == inspection_no).FirstOrDefault();
                    var lstTra = MasterdataEntities.TRA001.Where(m => m.DAISPNO == inspection_no).FirstOrDefault();
                    if (!string.IsNullOrEmpty(inspection_no) && lstInv != null && lstTra != null)
                    {
                        if (lstInv != null)
                        {
                            #region Save to INV
                            lstInv.CADSTNC = codeforIVPUR;
                            lstInv.CACTRNO = model.AAPURNO;
                            lstInv.CACTITM = model.ABCTITM;
                            lstInv.CASEDT = model.ABSEDT;
                            lstInv.CAPIVNO = model.ABPIVNO;
                            lstInv.CAPIVDT = model.ABPIVDT;
                            lstInv.CAVESEL = model.ABVESEL;
                            lstInv.CAMKCD = model.AAMKCD;
                            lstInv.CASPPNO = model.AASPPNO;
                            lstInv.CASPLCD = model.AASPLCD;
                            lstInv.CAUSRCD = model.AAUSRCD;
                            lstInv.CACRRCD = model.AACRRCD;
                            lstInv.CATXCD = model.AATXCD;
                            lstInv.CASMNTH = model.AASHPDT;
                            lstInv.CACTRNO = model.AAPURNO;
                            lstInv.CACMDCD = model.AACMDCD;
                            lstInv.CACTRTP = model.AACTRTP;
                            lstInv.CAIDCD = model.AAIDCD;
                            lstInv.CAEXRTT = model.AAEXRTT;
                            lstInv.CAEXRT = model.AAEXRT;
                            lstInv.CATEXRT = model.AATEXRT;
                            lstInv.CATXEXR = model.AATXEXR;
                            lstInv.CAMCSPC = model.ABMCSPC;
                            lstInv.CASPEC = model.ABMCSPC;
                            lstInv.CACOAT = model.ABCOAT;
                            lstInv.CABSZT = model.ABBSZT;
                            lstInv.CABSZW = model.ABBSZW;
                            lstInv.CABSZL = model.ABBSZL;
                            lstInv.CAPRDNM = model.ABPRDNM;
                            lstInv.CAPRDDIA = model.ABPRDDIA;
                            lstInv.CASTLGR = model.RAPSTLGR;
                            lstInv.CAORDNO = model.ABORDNO;
                            lstInv.CAISPNO = collection["inspecNo_" + i] == null ? "" : collection["inspecNo_" + i].ToString();
                            lstInv.CAINVNO = collection["rawMatNo_" + i] == null ? "" : collection["rawMatNo_" + i].ToString();
                            lstInv.CAQTY = collection["quantity_" + i] == null ? 0 : double.Parse(collection["quantity_" + i].ToString());
                            lstInv.CAWT = collection["weight_" + i] == null ? 0 : double.Parse(collection["weight_" + i].ToString());
                            lstInv.CASTSCD = collection["statusCode_" + i] == null ? "" : collection["statusCode_" + i].ToString();
                            lstInv.CAPRUP = collection["purUP_" + i] == null ? 0 : double.Parse(collection["purUP_" + i].ToString());
                            lstInv.CAPRUPD = collection["purUPD_" + i] == null ? 0 : double.Parse(collection["purUPD_" + i].ToString());
                            lstInv.CAPURAT = collection["purA_" + i] == null ? 0 : double.Parse(collection["purA_" + i].ToString());
                            lstInv.CAPURATD = collection["purAD_" + i] == null ? 0 : double.Parse(collection["purAD_" + i].ToString());
                            lstInv.CAPTXAT = collection["txA_" + i] == null ? 0 : double.Parse(collection["txA_" + i].ToString());
                            lstInv.CAPTXATD = collection["purA_" + i] == null ? 0 : double.Parse(collection["txAD_" + i].ToString());
                            //Hide
                            lstInv.CARGSDT = DateTime.Now;
                            lstInv.CARGSTM = DateTime.Now.ToString("HH:mm");
                            lstInv.CAINVTP = "M";
                            lstInv.CAINVST = "0";
                            #endregion
                        }
                        if (lstTra != null)
                        {
                            #region save to TRA001
                            lstTra.DADSTNC = codeforIVPUR;
                            lstTra.DACTRNO = model.AAPURNO;
                            lstTra.DACTITM = model.ABCTITM;
                            lstTra.DASEDT = model.ABSEDT;
                            lstTra.DAPIVNO = model.ABPIVNO;
                            lstTra.DAPIVDT = model.ABPIVDT;
                            lstTra.DAVESEL = model.ABVESEL;
                            lstTra.DAMKCD = model.AAMKCD;
                            lstTra.DASPPNO = model.AASPPNO;
                            lstTra.DASPLCD = model.AASPLCD;
                            lstTra.DAUSRCD = model.AAUSRCD;
                            lstTra.DACRRCD = model.AACRRCD;
                            lstTra.DATXCD = model.AATXCD;
                            lstTra.DASMNTH = model.AASHPDT;
                            lstTra.DACTRNO = model.AAPURNO;
                            lstTra.DACMDCD = model.AACMDCD;
                            lstTra.DACTRTP = model.AACTRTP;
                            lstTra.DAIDCD = model.AAIDCD;
                            lstTra.DAEXRTT = model.AAEXRTT;
                            lstTra.DAEXRT = model.AAEXRT;
                            lstTra.DATEXRT = model.AATEXRT;
                            lstTra.DATXEXR = model.AATXEXR;
                            lstTra.DAMCSPC = model.ABMCSPC;
                            lstTra.DASPEC = model.ABMCSPC;
                            lstTra.DACOAT = model.ABCOAT;
                            lstTra.DABSZT = model.ABBSZT;
                            lstTra.DABSZW = model.ABBSZW;
                            lstTra.DABSZL = model.ABBSZL;
                            lstTra.DAPRDNM = model.ABPRDNM;
                            lstTra.DAPRDDIA = model.ABPRDDIA;
                            lstTra.DASTLGR = model.RAPSTLGR;
                            lstTra.DAORDNO = model.ABORDNO;
                            lstTra.DAISPNO = collection["inspecNo_" + i] == null ? "" : collection["inspecNo_" + i].ToString();
                            lstTra.DAINVNO = collection["rawMatNo_" + i] == null ? "" : collection["rawMatNo_" + i].ToString();
                            lstTra.DAQTY = collection["quantity_" + i] == null ? 0 : double.Parse(collection["quantity_" + i].ToString());
                            lstTra.DAWT = collection["weight_" + i] == null ? 0 : double.Parse(collection["weight_" + i].ToString());
                            lstTra.DASTSCD = collection["statusCode_" + i] == null ? "" : collection["statusCode_" + i].ToString();
                            lstTra.DAPRUP = collection["purUP_" + i] == null ? 0 : double.Parse(collection["purUP_" + i].ToString());
                            lstTra.DAPRUPD = collection["purUPD_" + i] == null ? 0 : double.Parse(collection["purUPD_" + i].ToString());
                            lstTra.DAPURAT = collection["purA_" + i] == null ? 0 : double.Parse(collection["purA_" + i].ToString());
                            lstTra.DAPURATD = collection["purAD_" + i] == null ? 0 : double.Parse(collection["purAD_" + i].ToString());
                            lstTra.DAPTXAT = collection["txA_" + i] == null ? 0 : double.Parse(collection["txA_" + i].ToString());
                            lstTra.DAPTXATD = collection["purA_" + i] == null ? 0 : double.Parse(collection["txAD_" + i].ToString());
                            //Hide
                            lstTra.DAUPDT = DateTime.Now;
                            lstTra.DAUPDTM = DateTime.Now.ToString("HH:mm");
                            lstTra.DATRNDT = DateTime.Now;
                            lstTra.DALOGDT = DateTime.Now;
                            lstTra.DALOGTP = "10";
                            lstTra.DAINVTP = "M";
                            #endregion
                        }
                        MasterdataEntities.SaveChanges();
                    }
                    else
                    {
                        INV001 lstInv1 = new INV001();
                        TRA001 lstTra1 = new TRA001();
                        #region Save to INV
                        lstInv1.CADSTNC = codeforIVPUR;
                        lstInv1.CACTRNO = model.AAPURNO;
                        lstInv1.CACTITM = model.ABCTITM;
                        lstInv1.CASEDT = model.ABSEDT;
                        lstInv1.CAPIVNO = model.ABPIVNO;
                        lstInv1.CAPIVDT = model.ABPIVDT;
                        lstInv1.CAVESEL = model.ABVESEL;
                        lstInv1.CAMKCD = model.AAMKCD;
                        lstInv1.CASPPNO = model.AASPPNO;
                        lstInv1.CASPLCD = model.AASPLCD;
                        lstInv1.CAUSRCD = model.AAUSRCD;
                        lstInv1.CACRRCD = model.AACRRCD;
                        lstInv1.CATXCD = model.AATXCD;
                        lstInv1.CASMNTH = model.AASHPDT;
                        lstInv1.CACTRNO = model.AAPURNO;
                        lstInv1.CACMDCD = model.AACMDCD;
                        lstInv1.CACTRTP = model.AACTRTP;
                        lstInv1.CAIDCD = model.AAIDCD;
                        lstInv1.CAEXRTT = model.AAEXRTT;
                        lstInv1.CAEXRT = model.AAEXRT;
                        lstInv1.CATEXRT = model.AATEXRT;
                        lstInv1.CATXEXR = model.AATXEXR;
                        lstInv1.CAMCSPC = model.ABMCSPC;
                        lstInv1.CASPEC = model.ABMCSPC;
                        lstInv1.CACOAT = model.ABCOAT;
                        lstInv1.CABSZT = model.ABBSZT;
                        lstInv1.CABSZW = model.ABBSZW;
                        lstInv1.CABSZL = model.ABBSZL;
                        lstInv1.CAPRDNM = model.ABPRDNM;
                        lstInv1.CAPRDDIA = model.ABPRDDIA;
                        lstInv1.CASTLGR = model.RAPSTLGR;
                        lstInv1.CAORDNO = model.ABORDNO;
                        lstInv1.CAISPNO = collection["inspecNo_" + i] == null ? "" : collection["inspecNo_" + i].ToString();
                        lstInv1.CAINVNO = collection["rawMatNo_" + i] == null ? "" : collection["rawMatNo_" + i].ToString();
                        lstInv1.CAQTY = collection["quantity_" + i] == null ? 0 : double.Parse(collection["quantity_" + i].ToString());
                        lstInv1.CAWT = collection["weight_" + i] == null ? 0 : double.Parse(collection["weight_" + i].ToString());
                        lstInv1.CASTSCD = collection["statusCode_" + i] == null ? "" : collection["statusCode_" + i].ToString();
                        lstInv1.CAPRUP = collection["purUP_" + i] == null ? 0 : double.Parse(collection["purUP_" + i].ToString());
                        lstInv1.CAPRUPD = collection["purUPD_" + i] == null ? 0 : double.Parse(collection["purUPD_" + i].ToString());
                        lstInv1.CAPURAT = collection["purA_" + i] == null ? 0 : double.Parse(collection["purA_" + i].ToString());
                        lstInv1.CAPURATD = collection["purAD_" + i] == null ? 0 : double.Parse(collection["purAD_" + i].ToString());
                        lstInv1.CAPTXAT = collection["txA_" + i] == null ? 0 : double.Parse(collection["txA_" + i].ToString());
                        lstInv1.CAPTXATD = collection["purA_" + i] == null ? 0 : double.Parse(collection["txAD_" + i].ToString());
                        //Hide
                        lstInv1.CARGSDT = DateTime.Now;
                        lstInv1.CARGSTM = DateTime.Now.ToString("HH:mm");
                        lstInv1.CAINVTP = "M";
                        lstInv1.CAINVST = "0";
                        #endregion
                        #region save to TRA001
                        lstTra1.DADSTNC = codeforIVPUR;
                        lstTra1.DACTRNO = model.AAPURNO;
                        lstTra1.DACTITM = model.ABCTITM;
                        lstTra1.DASEDT = model.ABSEDT;
                        lstTra1.DAPIVNO = model.ABPIVNO;
                        lstTra1.DAPIVDT = model.ABPIVDT;
                        lstTra1.DAVESEL = model.ABVESEL;
                        lstTra1.DAMKCD = model.AAMKCD;
                        lstTra1.DASPPNO = model.AASPPNO;
                        lstTra1.DASPLCD = model.AASPLCD;
                        lstTra1.DAUSRCD = model.AAUSRCD;
                        lstTra1.DACRRCD = model.AACRRCD;
                        lstTra1.DATXCD = model.AATXCD;
                        lstTra1.DASMNTH = model.AASHPDT;
                        lstTra1.DACTRNO = model.AAPURNO;
                        lstTra1.DACMDCD = model.AACMDCD;
                        lstTra1.DACTRTP = model.AACTRTP;
                        lstTra1.DAIDCD = model.AAIDCD;
                        lstTra1.DAEXRTT = model.AAEXRTT;
                        lstTra1.DAEXRT = model.AAEXRT;
                        lstTra1.DATEXRT = model.AATEXRT;
                        lstTra1.DATXEXR = model.AATXEXR;
                        lstTra1.DAMCSPC = model.ABMCSPC;
                        lstTra1.DASPEC = model.ABMCSPC;
                        lstTra1.DACOAT = model.ABCOAT;
                        lstTra1.DABSZT = model.ABBSZT;
                        lstTra1.DABSZW = model.ABBSZW;
                        lstTra1.DABSZL = model.ABBSZL;
                        lstTra1.DAPRDNM = model.ABPRDNM;
                        lstTra1.DAPRDDIA = model.ABPRDDIA;
                        lstTra1.DASTLGR = model.RAPSTLGR;
                        lstTra1.DAORDNO = model.ABORDNO;
                        lstTra1.DAISPNO = collection["inspecNo_" + i] == null ? "" : collection["inspecNo_" + i].ToString();
                        lstTra1.DAINVNO = collection["rawMatNo_" + i] == null ? "" : collection["rawMatNo_" + i].ToString();
                        lstTra1.DAQTY = collection["quantity_" + i] == null ? 0 : double.Parse(collection["quantity_" + i].ToString());
                        lstTra1.DAWT = collection["weight_" + i] == null ? 0 : double.Parse(collection["weight_" + i].ToString());
                        lstTra1.DASTSCD = collection["statusCode_" + i] == null ? "" : collection["statusCode_" + i].ToString();
                        lstTra1.DAPRUP = collection["purUP_" + i] == null ? 0 : double.Parse(collection["purUP_" + i].ToString());
                        lstTra1.DAPRUPD = collection["purUPD_" + i] == null ? 0 : double.Parse(collection["purUPD_" + i].ToString());
                        lstTra1.DAPURAT = collection["purA_" + i] == null ? 0 : double.Parse(collection["purA_" + i].ToString());
                        lstTra1.DAPURATD = collection["purAD_" + i] == null ? 0 : double.Parse(collection["purAD_" + i].ToString());
                        lstTra1.DAPTXAT = collection["txA_" + i] == null ? 0 : double.Parse(collection["txA_" + i].ToString());
                        lstTra1.DAPTXATD = collection["purA_" + i] == null ? 0 : double.Parse(collection["txAD_" + i].ToString());
                        //Hide
                        lstTra1.DAUPDT = DateTime.Now;
                        lstTra1.DAUPDTM = DateTime.Now.ToString("HH:mm");
                        lstTra1.DATRNDT = DateTime.Now;
                        lstTra1.DALOGDT = DateTime.Now;
                        lstTra1.DALOGTP = "10";
                        lstTra1.DAINVTP = "M";
                        #endregion
                        MasterdataEntities.INV001.Add(lstInv1);
                        MasterdataEntities.TRA001.Add(lstTra1);
                        var modelPUR = MasterdataEntities.PUR001.Where(m => m.ID == model.ID).FirstOrDefault();
                        modelPUR.ABSEDT = model.ABSEDT;
                        modelPUR.ABPIVNO = model.ABPIVNO;
                        modelPUR.ABPIVDT = model.ABPIVDT;
                        modelPUR.ABVESEL = model.ABVESEL;
                        MasterdataEntities.SaveChanges();
                    }
                }
            }
            return RedirectToAction("GetRawMaterialWarehousingEntry");
        }
        public enum ReferenceType
        {
            Makercode = 0,
            Currencycode = 1,
            Priceterm = 2,
            Typeofterm = 3,
            Commodity = 4,
            Contracttype = 5,
            Settelementterm = 6,
            taxcode = 7,
            exchangeratetype = 8,
            spec = 9,
            coating = 10,
            usercode = 11,
            rawmaterialtype = 12,
            steelgrade = 13,
            personincharge = 14,
            suppliercode = 15,
            deliverycondition = 16,
            grade = 17       //PNB EDIT 
        }
        public JsonResult GetReferenceEachProperties(ReferenceType type)
        {
            var dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            var model = new INV001();
            model = dal.GetReferences(model);
            Dictionary<string, string> dictionnary = new Dictionary<string, string>();
            switch (type)
            {
                case ReferenceType.Makercode:
                    dictionnary = model.MakerCodes;
                    break;
                case ReferenceType.Currencycode:
                    dictionnary = model.CurrencyCodes;
                    break;
                case ReferenceType.Priceterm:
                    dictionnary = model.Priceterms;
                    break;
                case ReferenceType.Typeofterm:
                    dictionnary = model.TypeOfTerms;
                    break;
                case ReferenceType.Commodity:
                    dictionnary = model.CommodityCodes;
                    break;
                case ReferenceType.Contracttype:
                    dictionnary = model.ContractTypes;
                    break;
                case ReferenceType.Settelementterm:
                    dictionnary = model.SettelementTerms;
                    break;
                case ReferenceType.taxcode:
                    dictionnary = model.TaxCodes;
                    break;
                case ReferenceType.exchangeratetype:
                    dictionnary = model.ExchangeRateType;
                    break;
                case ReferenceType.spec:
                    dictionnary = model.Specs;
                    break;
                case ReferenceType.coating:
                    dictionnary = model.Coatings;
                    break;
                case ReferenceType.usercode:
                    dictionnary = model.UserCodes;
                    break;
                case ReferenceType.rawmaterialtype:
                    dictionnary = model.RawMaterialTypes;
                    break;
                case ReferenceType.steelgrade:
                    dictionnary = model.SteelGrades;
                    break;
                case ReferenceType.personincharge:
                    dictionnary = model.PersonIncharges;
                    break;
                case ReferenceType.suppliercode:
                    dictionnary = model.SupplierCodes;
                    break;
                case ReferenceType.deliverycondition:
                    dictionnary = model.DeliveryConditionCodes;
                    break;
                case ReferenceType.grade:  //pnbedit
                    dictionnary = model.Grades;
                    break;
            }
            return Json(dictionnary, JsonRequestBehavior.AllowGet);
        }
        #region LoadCodeName
        public JsonResult GetValueMA0012NameBaseOnCode(string Code, string number)
        {
            string result = "";
            var chkSQL = MasterdataEntities.MA012.Where(m => m.MNCLSCD == number && m.MNSRCD == Code).FirstOrDefault();
            if (chkSQL != null)
            {
                result = chkSQL.MNSRNM;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetValueMA001NameBaseOnCode(string Code)
        {
            string result = "";
            var chkSQL = MasterdataEntities.MA001.Where(m => m.MASPCD == Code).FirstOrDefault();
            if (chkSQL != null)
            {
                result = chkSQL.MASPNM;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetValueMA003NameBaseOnCode(string Code)
        {
            string result = "";
            var chkSQL = MasterdataEntities.MA003.Where(m => m.MCIDCD == Code).FirstOrDefault();
            if (chkSQL != null)
            {
                result = chkSQL.MCIDNM;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetValueMA002NameBaseOnCode(string Code)
        {
            string result = "";
            var chkSQL = MasterdataEntities.MA002.Where(m => m.MBUSRCD == Code).FirstOrDefault();
            if (chkSQL != null)
            {
                result = chkSQL.MBUSRNM;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetValueMA010NameBaseOnCode(string Code)
        {
            string result = "";
            var chkSQL = MasterdataEntities.MA010.Where(m => m.MKTXCD == Code).FirstOrDefault();
            if (chkSQL != null)
            {
                result = chkSQL.MKTXDL;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [HttpPost]
        public JsonResult LoadExcelData()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    string fname = "";
                    HttpFileCollectionBase files = Request.Files;
                    for (int n = 0; n < files.Count; n++)
                    {
                        HttpPostedFileBase file = files[n];
                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        //string path = SaveExcelURL; // or whatever 
                        //string fileDelete = "";
                        //if (!Directory.Exists(path))
                        //{
                        //    DirectoryInfo di = Directory.CreateDirectory(path);
                        //    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;                           
                        //}
                        //fname = System.IO.Path.Combine(SaveExcelURL, fname);
                        //fileDelete = fname;
                        //file.SaveAs(fname);
                        //string path = SaveExcelURL; // or whatever 
                        //string fileDelete = "";
                        //if (!Directory.Exists(path))
                        //{
                        //    DirectoryInfo di = Directory.CreateDirectory(path);
                        //    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                        //}
                        //fname = System.IO.Path.Combine(SaveExcelURL, fname);
                        //fileDelete = fname;
                        //file.SaveAs(fname);
                        //if (!fname.Contains("xlsx"))
                        //{
                        //    var app = new Microsoft.Office.Interop.Excel.Application();
                        //    var xlsFile = fname;
                        //    var wb = app.Workbooks.Open(xlsFile);
                        //    var xlsxFile = xlsFile + "x";
                        //    app.DisplayAlerts = false;
                        //    wb.SaveAs(Filename: xlsxFile, FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
                        //    wb.Close();
                        //    app.Quit();
                        //    fname = xlsxFile;
                        //}
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~/FileCreated/ExcelRawMat/"));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/FileCreated/ExcelRawMat/"));
                        fname = System.IO.Path.Combine(Server.MapPath("~/FileCreated/ExcelRawMat/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json(fname, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json("Fail: " + ex.Message);
                }
            }
            else
            {
                return Json("Dont have select file");
            }
        }
        public ActionResult LoadDataFromExcel(string filename, double? taxRate, double? pUP, double? pUPD, string commodity, string markerCode, string statuscode, string steelGrade, double? producDameter, string contractNo)
        {
            //string statuscode = Request.Form["statuscode"] == null ? "" : Request.Form["statuscode"].ToString();
            //string markerCode = Request.Form["markerCode"] == null ? "" : Request.Form["markerCode"].ToString().Trim();
            //string comodity = Request.Form["comodity"] == null ? "" : Request.Form["comodity"].ToString().Substring(0, 1).Trim();
            //double? taxRate = Request.Form["taxRate"] == null ? 0 : double.Parse(Request.Form["taxRate"].ToString());
            //double? pUP = Request.Form["pUP"] == null ? 0 : double.Parse(Request.Form["pUP"].ToString());
            //double? pUPD = Request.Form["pUPD"] == null ? 0 : double.Parse(Request.Form["pUPD"].ToString());
            string year = DateTime.Now.ToString("yyyy").Substring(2, 2);
            string month = DateTime.Now.ToString("MM");
            if (int.Parse(month) < 10)
            {
                month = month.Substring(1, 1);
            }
            else
            {
                if (month == "10")
                {
                    month = "X";
                }
                if (month == "11")
                {
                    month = "Y";
                }
                if (month == "12")
                {
                    month = "Z";
                }
            }
            int runningNo = 1;
            var _ListItemMaterial = new List<DataInspecExcel>();
            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    FileInfo fileexcel = new FileInfo(filename);
                    using (ExcelPackage package = new ExcelPackage(fileexcel))
                    {
                        // ExcelWorksheet workSheet = package.Workbook.Worksheets["PurchaseContractEntry"];
                        ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault();
                        int totalRows = workSheet.Dimension.Rows;
                        string contracNoExcel = workSheet.Cells[15, 4].Value == null ? "" : workSheet.Cells[15, 4].Value.ToString().Trim();
                        if (contracNoExcel == contractNo)
                        {
                            bool flagChkSizeSG = false;
                            for (int i = 19; i <= totalRows; i++)
                            {
                                string newRN = "";
                                if (runningNo.ToString().Count() == 1)
                                {
                                    newRN = "000" + runningNo;
                                }
                                else if (runningNo.ToString().Count() == 2)
                                {
                                    newRN = "00" + runningNo;
                                }
                                else if (runningNo.ToString().Count() == 3)
                                {
                                    newRN = "0" + runningNo;
                                }
                                else if (runningNo.ToString().Count() == 4)
                                {
                                    newRN = runningNo.ToString();
                                }
                                string no = workSheet.Cells[i, 1].Value == null ? "" : workSheet.Cells[i, 1].Value.ToString();
                                if (IsNumeric(no))
                                {
                                    string getSize = workSheet.Cells[i, 4].Value == null ? "0" : workSheet.Cells[i, 4].Value.ToString().Replace("Φ", "").Trim();
                                    double? size = double.Parse(getSize);
                                    string spec = workSheet.Cells[i, 6].Value == null ? "" : workSheet.Cells[i, 6].Value.ToString().Trim();
                                    if (size == producDameter && spec == steelGrade)
                                    {
                                        DataInspecExcel a = new DataInspecExcel();
                                        a.InspectionNo = workSheet.Cells[i, 5].Value.ToString();
                                        a.RawMaterialNo = commodity + markerCode + year + month + newRN;
                                        a.Quantity = int.Parse(workSheet.Cells[i, 8].Value.ToString());
                                        a.Weight = Math.Round(double.Parse(workSheet.Cells[i, 11].Value.ToString().Replace(",", ".")), 2);
                                        a.StatusCode = statuscode;
                                        a.PurchaseUP = pUP;
                                        a.PurchaseUPD = pUPD;
                                        a.PurAmount = Math.Round(double.Parse((a.Weight * a.PurchaseUP).ToString()), 2);
                                        a.PurAmountDomestic = Math.Round(double.Parse((a.Weight * a.PurchaseUPD).ToString()), 2);
                                        a.TaxAmount = Math.Round(double.Parse(((taxRate / 100) * a.PurAmount).ToString()), 2);
                                        a.TaxAmountDomestic = Math.Round(double.Parse(((taxRate / 100) * a.PurAmountDomestic).ToString()), 2);
                                        _ListItemMaterial.Add(a);
                                        runningNo++;
                                        flagChkSizeSG = true;
                                    }
                                }
                            }
                            if (flagChkSizeSG == false)
                            {
                                string result = "SizeOrSteelGradeNoarenotmatching";
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            string result = "SalesContractNoarenotmatching";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return PartialView("_LoadDataFromExcel", _ListItemMaterial);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LoadDataSQLInEx(string key)
        {
            var chkInINV = MasterdataEntities.INV001.Where(m => m.CADSTNC == key).ToList();
            var _ListItemMaterial = chkInINV.Select(m => new DataInspecExcel()
            {
                Status = m.CAINVST,
                InspectionNo = m.CAISPNO,
                RawMaterialNo = m.CAINVNO,
                Quantity = int.Parse(m.CAQTY.GetValueOrDefault().ToString()),
                Weight = m.CAWT,
                StatusCode = m.CASTSCD,
                PurchaseUP = m.CAPRUP,
                PurchaseUPD = m.CAPRUPD,
                PurAmount = m.CAPURAT,
                PurAmountDomestic = m.CAPURATD,
                TaxAmount = m.CAPTXAT,
                TaxAmountDomestic = m.CAPTXATD,
            });
            return PartialView("_LoadDataFromSQL", _ListItemMaterial);
        }
        #region check number
        public static bool IsNumeric(object Expression)
        {
            double retNum;
            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
        #endregion
        private string CreateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            var rs = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            var dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            if (!dal.CheckCodeCreateExists(rs)) return CreateCode();
            return rs;
        }
        public ActionResult DeleteINVTRA(string keys)
        {
            try
            {
                string[] array = null;
                if (!string.IsNullOrEmpty(keys))
                {
                    array = keys.Split('|');
                }
                if (array != null)
                {
                    foreach (var key in array)
                    {
                        using (var dc = new TopProSystemEntities())
                        {
                            var sqlParamINV = new SqlParameter[]
                            {
                               new SqlParameter("@PrimaryKey",key),
                            };
                            var sqlParamTRA = new SqlParameter[]
                            {
                               new SqlParameter("@PrimaryKey",key),
                            };
                            dc.Database.ExecuteSqlCommand("EXEC proc_deleteINV @PrimaryKey", sqlParamINV);
                            dc.Database.ExecuteSqlCommand("EXEC proc_deleteTRA @PrimaryKey", sqlParamTRA);
                        }
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DelSameInspec(string key, string inspec)
        {
            try
            {
                string[] lstInspec = inspec.TrimStart(',').Split(',');
                var tableINV = new DataTable();
                tableINV.Columns.Add("CADSTNC", typeof(string));
                var tableTRA = new DataTable();
                tableTRA.Columns.Add("DADSTNC", typeof(string));
                foreach (var item in lstInspec)
                {
                    tableINV.Rows.Add(item);
                    tableTRA.Rows.Add(item);
                }
                using (var dc = new TopProSystemEntities())
                {
                    var sqlParamINV_Delete = new SqlParameter[]
                            {
                               new SqlParameter("@ItemList",SqlDbType.Structured)
                               {
                                   TypeName = "dbo.MyListINV001_DEL",
                                   Value = tableINV
                               },
                               new SqlParameter("@key",key),
                            };
                    var sqlParamTRA_Delete = new SqlParameter[]
                            {
                               new SqlParameter("@ItemList",SqlDbType.Structured)
                               {
                                   TypeName = "dbo.MyListTRA001_DEL",
                                   Value = tableINV
                               },
                               new SqlParameter("@key",key),
                            };
                    dc.Database.ExecuteSqlCommand("EXEC proc_deleteListInspecINV @ItemList,@key", sqlParamINV_Delete);
                    dc.Database.ExecuteSqlCommand("EXEC proc_deleteListInspecTRA @ItemList,@key", sqlParamTRA_Delete);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckCodeRaw(string codeCheck)
        {
            int result = 1;
            string getList = MasterdataEntities.INV001.Where(m => m.CAINVNO.Contains(codeCheck)).OrderByDescending(m => m.CAINVNO.Substring(5)).Select(m => m.CAINVNO).FirstOrDefault();
            if (!string.IsNullOrEmpty(getList))
            {
                getList = getList.Substring(5);
                int newResult = int.TryParse(getList, out newResult) ? newResult : 1;
                result = newResult + 1;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}