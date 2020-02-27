using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using TopProSystem.Models;
using TopProSystem.Areas.MasterSetting.Models;
using System.Text;
using System.Transactions;
using TopProSystem.Extension.AccountRole;
using TopProSystem.Areas.MasterSetting;
using System.Data;
using Ionic.Zip;

namespace TopProSystem.Controllers
{
    public class PurchaseController : BasePurchaseController
    {
        private string PurchasePDFFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["SaveReportPurchasePdfURL"];
        private string PurchaseExcelFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["SaveReportPurchaseExcelURL"];
        private string PrinterName = System.Web.Configuration.WebConfigurationManager.AppSettings["PrinterMachine"];
        private string FileNameExtension = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
        private string FailMessage = TopProSystem.Models.ConstantData.FailMessage;
        private string SuccessMessage = TopProSystem.Models.ConstantData.SuccessMessage;
        private TopProSystem.Areas.MasterSetting.Models.TopProSystemEntities MasterdataEntities = new Areas.MasterSetting.Models.TopProSystemEntities();



        [NonAction]
        public void CreateSpecialFolder(string type)
        {

           // string[] arrayRootUrl = null;
            switch (type)
            {
                case "pdf":
                    Directory.CreateDirectory(PurchasePDFFilePath);
                    //arrayRootUrl = PurchasePDFFilePath.Split('/');
                    break;
                case "excel":
                    Directory.CreateDirectory(PurchaseExcelFilePath);
                    //arrayRootUrl = PurchaseExcelFilePath.Split('/');
                    break;
            }

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


        }

        public enum ExportFileResult
        {
            Success = 0,
            Fail = 1,
            Nodata = 2
        }

        [HttpPost]
        public ActionResult AjaxHandler(jQueryDataTablePurchase param, PurchaseSearchModel searchmodel)
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            var INV001dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            var countRecord = dal.GetTotalRecord(searchmodel);
            var displayItem = dal.GetTotalDisplayRecord(param, searchmodel);
            var ma001_dal = new Areas.MasterSetting.DAL.MA001.MA001_DAL();
            var ma002_dal = new Areas.MasterSetting.DAL.MA002.MA002_DAL();
            var ma003_dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();
            var result = displayItem.Select(x => new
            {
                AADSTNC = x.AADSTNC, // primary key
                ABPURNO = x.AAPURNO,
                AAMKCD = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE005, x.AAMKCD),
                AASPPNO = x.AASPPNO,
                AASPLCD = ma001_dal.GetSalePurchase(x.AASPLCD) != null ? ma001_dal.GetSalePurchase(x.AASPLCD).MASPNM : string.Empty,
                AACMDCD = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE006, x.AACMDCD),
                AAUSRCD = ma002_dal.GetUserName(x.AAUSRCD),
                AACTRTP = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE025, x.AACTRTP),
                AASHPDT = x.AASHPDT,
                AAIDCD = ma003_dal.GetMA003(x.AAIDCD) != null ? ma003_dal.GetMA003(x.AAIDCD).MCIDNM : string.Empty,
                AADLVCD = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE024, x.AADLVCD),
                _canDelete = INV001dal.CheckPurchaseContractReferenceRawMaterial(x.AAPURNO)
            });
            return Json(new
            {
                recordsTotal = countRecord,
                recordsFiltered = countRecord,
                draw = param.draw,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAll()
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            return Json(dal.GetAll(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPurchaseContractEntry()
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            return View(dal.GetReferences(new PUR001()));
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
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            var model = new PUR001();
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
        public ViewResult GetPurchaseContractEntry_Add()
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            return View(dal.GetReferences(new PUR001()));
        }

        [HttpPost]
        public ActionResult InsertPurchaseContract(FormCollection collection)
        {

            Areas.MasterSetting.DAL.PUR001.PUR001_DAL pUR001_DAL = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();


            int count_item;
            string last_key = collection.AllKeys.Last(); // lay ra key cuoi cung de xem co bao nhieu item
            try
            {
                count_item = int.Parse(last_key.Substring(last_key.IndexOf('_') + 1));
            }
            catch
            {
                count_item = 1;
            }

            int n = 1;
            Nullable<double> EmptyDouble = null;
            Nullable<Decimal> EmptyDemical = null;
            double isNumeric;
            decimal _decimal;
            string str;
            DateTime date;

            PUR001 model = new PUR001();
            model.AAPURNO = pUR001_DAL.CreatePurchaseContractNo();
            model.ABRGSDT = DateTime.Now;
            model.AADSTNC = CreateCode();
            model.AAREVNO = "00";

            model.AARMTP = collection["AARMTP"];
            model.AARGSDT = !string.IsNullOrEmpty(collection["AARGSDT"]) && DateTime.TryParse(collection["AARGSDT"], out date) ? DateTime.Parse(collection["AARGSDT"]) : DateTime.Now;
            model.AACMPCD = collection["AACMPCD"];
            //  model.AACMPDT = !string.IsNullOrEmpty(collection["AACMPDT"]) && DateTime.TryParse(collection["AACMPDT"], out date) ? DateTime.Parse(collection["AACMPDT"]) : DateTime.Now;
            model.AAMKCD = collection["AAMKCD"];
            model.AASPPNO = collection["AASPPNO"];
            model.AASPLCD = collection["AASPLCD"];
            model.AAUSRCD = collection["AAUSRCD"];
            model.AASHPDT = collection["AASHPDT"];
            model.AADLVCD = collection["AADLVCD"];
            model.AACRRCD = collection["AACRRCD"];
            model.AATXCD = collection["AATXCD"];
            model.AAPRICE = collection["AAPRICE"];
            model.AAPTTRM = collection["AAPTTRM"];
            model.AACMDCD = collection["AACMDCD"];
            model.AACTRTP = collection["AACTRTP"];
            model.AAIDCD = collection["AAIDCD"];
            model.AAEXRT = !string.IsNullOrEmpty(collection["AAEXRT"]) && double.TryParse(collection["AAEXRT"], out isNumeric) ? double.Parse(collection["AAEXRT"]) : EmptyDouble;
            model.AATXEXR = !string.IsNullOrEmpty(collection["AATXEXR"]) && double.TryParse(collection["AATXEXR"], out isNumeric) ? double.Parse(collection["AATXEXR"]) : EmptyDouble;
            model.AATXRT = !string.IsNullOrEmpty(collection["AATXRT"]) && double.TryParse(collection["AATXRT"], out isNumeric) ? double.Parse(collection["AATXRT"]) : EmptyDouble;
            model.AASETRM = collection["AASETRM"];
            model.AADLPOT = collection["AADLPOT"];
            model.AARMK1 = collection["AARMK1"];
            model.AARMK2 = collection["AARMK2"];
            model.AARMK3 = collection["AARMK3"];
            model.AARMK4 = collection["AARMK4"];
            model.AARMK5 = collection["AARMK5"];

            model.AAEXRTT = collection["AAEXRT-sl"]; // dung cho xuat excel
            model.AATEXRT = collection["AATEXRT"];

            using (var transaction = new TransactionScope())  // su dung giao tac vi phai ep code theo he thong nhu shit nay, neu insert 1 dong loi thi phai rollback. 
            {
                try
                {
                    while (n <= count_item)
                    {
                        str = ConcatString(n);
                        if (string.IsNullOrEmpty(collection["ABCTITM" + str]))// de phong truong hop xoa item dau (name khong thay doi khi xoa)
                        {
                            n++;
                            continue;
                        }

                        model.ABCTITM = int.Parse(collection["ABCTITM" + str]);
                        model.ABMCSPC = collection["ABMCSPC" + str];
                        model.ABGRADE = collection["ABGRADE" + str];
                        model.ABCOAT = collection["ABCOAT" + str];
                        model.RAPSTLGR = collection["RAPSTLGR" + str];
                        model.ABDLVDT = !string.IsNullOrEmpty(collection["ABDLVDT" + str]) && DateTime.TryParse(collection["ABDLVDT"], out date) ? DateTime.Parse(collection["ABDLVDT" + str]) : DateTime.Now; // loi
                                                                                                                                                                                                               // model.ABWT = !string.IsNullOrEmpty(collection["ABWT" + str]) && double.TryParse(collection["ABWT" + str], out isNumeric) ? double.Parse(collection["ABWT" + str]) : EmptyDouble;
                        model.ABWT = !string.IsNullOrEmpty(collection["ABWT" + str]) && decimal.TryParse(collection["ABWT" + str], out _decimal) ? decimal.Parse(collection["ABWT" + str]) : EmptyDemical;


                        //  model.ABPRUP = !string.IsNullOrEmpty(collection["ABPRUP" + str]) && double.TryParse(collection["ABPRUP" + str], out isNumeric) ? double.Parse(collection["ABPRUP" + str]) : EmptyDouble;
                        // model.ABPRAT = !string.IsNullOrEmpty(collection["ABPRAT" + str]) && double.TryParse(collection["ABPRAT" + str], out isNumeric) ? double.Parse(collection["ABPRAT" + str]) : EmptyDouble;
                        model.ABPRUP = !string.IsNullOrEmpty(collection["ABPRUP" + str]) && decimal.TryParse(collection["ABPRUP" + str], out _decimal) ? decimal.Parse(collection["ABPRUP" + str]) : EmptyDemical;
                        model.ABPRAT = !string.IsNullOrEmpty(collection["ABPRAT" + str]) && decimal.TryParse(collection["ABPRAT" + str], out _decimal) ? decimal.Parse(collection["ABPRAT" + str]) : EmptyDemical;

                        model.ABPTXAT = !string.IsNullOrEmpty(collection["ABPTXAT" + str]) && decimal.TryParse(collection["ABPTXAT" + str], out _decimal) ? decimal.Parse(collection["ABPTXAT" + str]) : EmptyDemical;
                        model.ABBSZT = !string.IsNullOrEmpty(collection["ABBSZT" + str]) && double.TryParse(collection["ABBSZT" + str], out isNumeric) ? double.Parse(collection["ABBSZT" + str]) : EmptyDouble;
                        model.ABBSZW = !string.IsNullOrEmpty(collection["ABBSZW" + str]) && double.TryParse(collection["ABBSZW" + str], out isNumeric) ? double.Parse(collection["ABBSZW" + str]) : EmptyDouble;
                        model.ABBSZL = !string.IsNullOrEmpty(collection["ABBSZL" + str]) && double.TryParse(collection["ABBSZL" + str], out isNumeric) ? double.Parse(collection["ABBSZL" + str]) : EmptyDouble;
                        model.ABORDNO = collection["ABORDNO" + str];
                        model.ABPRDNM = collection["ABPRDNM" + str];
                        model.ABPRDDIA = collection["ABPRDDIA" + str];
                        model.ABQTY = !string.IsNullOrEmpty(collection["ABQTY" + str]) && double.TryParse(collection["ABQTY" + str], out isNumeric) ? double.Parse(collection["ABQTY" + str]) : EmptyDouble;
                        //model.ABPRUPD = !string.IsNullOrEmpty(collection["ABPRUPD" + str]) && double.TryParse(collection["ABPRUPD" + str], out isNumeric) ? double.Parse(collection["ABPRUPD" + str]) : EmptyDouble;
                        //model.ABPRATD = !string.IsNullOrEmpty(collection["ABPRATD" + str]) && double.TryParse(collection["ABPRATD" + str], out isNumeric) ? double.Parse(collection["ABPRATD" + str]) : EmptyDouble;
                        model.ABPRUPD = !string.IsNullOrEmpty(collection["ABPRUPD" + str]) && decimal.TryParse(collection["ABPRUPD" + str], out _decimal) ? decimal.Parse(collection["ABPRUPD" + str]) : EmptyDemical;
                        model.ABPRATD = !string.IsNullOrEmpty(collection["ABPRATD" + str]) && decimal.TryParse(collection["ABPRATD" + str], out _decimal) ? decimal.Parse(collection["ABPRATD" + str]) : EmptyDemical;
                        model.ABPTXAD = !string.IsNullOrEmpty(collection["ABPTXAD" + str]) && decimal.TryParse(collection["ABPTXAD" + str], out _decimal) ? decimal.Parse(collection["ABPTXAD" + str]) : EmptyDemical;
                        model.WarehousingWeight = 0;
                        model.CompletetionStatus = 0;

                        if (!pUR001_DAL.Insert(model))
                        {
                            transaction.Dispose();
                            return Content("Cannot insert data, please checking database !");
                        }

                        n++;
                    }

                    transaction.Complete();
                    TempData[ConstantData.Notification_key] = MessageSendView.MessageNotifi("insert");
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return Content("Some validation error is happen, please contact support for error: " + ex.Message);
                }
            }

            return RedirectToAction("GetPurchaseContractEntry");
        }
        public ActionResult DeletePurchaseContract(string keys)
        {
            try
            {
                var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
                string[] array = null;
                if (!string.IsNullOrEmpty(keys))
                {
                    array = keys.Split('|');
                }

                if (array != null)
                {
                    foreach (var key in array)
                    {
                        if (!dal.Delete(key))
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetPurchaseContractEntry_Change(string key)
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            var Inv001Dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            //if (!dal.CheckCompletionStatus(key))
            //{
            //    //return RedirectToAction("NotFound", "ErrorMessage", new { area = "MasterSetting" }); // truong hop nguoi ta dung duong link
            //    return View("NotFound404Page");
            //}

            // Không được sửa {
            var _model = dal.GetReferences(dal.GetPUR001s(key));
            if (dal.CheckCompletionStatus(key)) _model._completed = true; // completed all
            else _model._completed = false;

            if (Inv001Dal.CheckPurchaseContractReferenceRawMaterial(_model.AAPURNO)) // đã import raw material
            {
                _model._completed = true;   
            }

            // }

            ViewBag.Items = dal.GetAllItemOfPurchase(key);

            //try
            //{
            //    ViewBag.Currency = dal.GetExchangeRatetypeCodeByExchangeRate(_model.AACRRCD,(DateTime)_model.AARGSDT,_model.AAEXRTT);
            //}
            //catch
            //{
            //    return View(_model);
            //}

            return View(_model);
        }
        [HttpPost]
        public ActionResult UpdatePurchaseContract(FormCollection collection)
        {
            Areas.MasterSetting.DAL.PUR001.PUR001_DAL pUR001_DAL = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();

            int count_item;
            string last_key = collection.AllKeys.Last();
            try
            {
                count_item = int.Parse(last_key.Substring(last_key.IndexOf('_') + 1));
            }
            catch
            {
                count_item = 1;
            }

            //if (!pUR001_DAL.CheckCompletionStatus(collection["AADSTNC"])) // truong hop user khac complete khi no dang duoc sua.
            //{
            //    return Content("<script>alert('Cannot change this purchase contract because it was completed already.'); window.location.href = '" + Url.Action("GetPurchaseContractEntry") + "';</script>");
            //}

            using (var transaction = new TransactionScope())
            {
                var OldModel = pUR001_DAL.GetPUR001s(collection["AADSTNC"]);
                DateTime register_time = OldModel.ABRGSDT.Value;
                string RevisionNo = OldModel.AAREVNO;
                PUR001[] itemNo = pUR001_DAL.GetAllCompletedItem(collection["AADSTNC"]);

                PUR001 model = null;

                bool f_delete = pUR001_DAL.Delete(collection["AADSTNC"], deleteForUpdate: true); // delete hoan toan cai cu

                if (f_delete)
                {
                    model = new PUR001();
                }
                else
                {
                    return Content("Update fail.");
                }

                int n = 1;
                Nullable<double> EmptyDouble = null;
                Nullable<Decimal> EmptyDemical = null;
                decimal _decimal;
                double isNumeric;

                string str;
                DateTime date;

                model.AADSTNC = collection["AADSTNC"];
                model.ABRGSDT = register_time;
                model.AAUPDT = DateTime.Now;
                model.ABUPDT = DateTime.Now;
                model.AAREVNO = IncreRevisionNo(RevisionNo);

                model.AAPURNO = collection["AAPURNO"]; // purchase contract no
                model.AARMTP = collection["AARMTP"];
                model.AARGSDT = !string.IsNullOrEmpty(collection["AARGSDT"]) && DateTime.TryParse(collection["AARGSDT"], out date) ? DateTime.Parse(collection["AARGSDT"]) : DateTime.Now;
                model.AACMPCD = collection["AACMPCD"];
                //  model.AACMPDT = !string.IsNullOrEmpty(collection["AACMPDT"]) && DateTime.TryParse(collection["AACMPDT"], out date) ? DateTime.Parse(collection["AACMPDT"]) : DateTime.Now;
                model.AAMKCD = collection["AAMKCD"];
                model.AASPPNO = collection["AASPPNO"];
                model.AASPLCD = collection["AASPLCD"];
                model.AAUSRCD = collection["AAUSRCD"];
                model.AASHPDT = collection["AASHPDT"];
                model.AADLVCD = collection["AADLVCD"];
                model.AACRRCD = collection["AACRRCD"];
                model.AATXCD = collection["AATXCD"];
                model.AAPRICE = collection["AAPRICE"];
                model.AAPTTRM = collection["AAPTTRM"];
                model.AACMDCD = collection["AACMDCD"];
                model.AACTRTP = collection["AACTRTP"];
                model.AAIDCD = collection["AAIDCD"];
                model.AAEXRT = !string.IsNullOrEmpty(collection["AAEXRT"]) && double.TryParse(collection["AAEXRT"], out isNumeric) ? double.Parse(collection["AAEXRT"]) : EmptyDouble;
                model.AATXEXR = !string.IsNullOrEmpty(collection["AATXEXR"]) && double.TryParse(collection["AATXEXR"], out isNumeric) ? double.Parse(collection["AATXEXR"]) : EmptyDouble;
                model.AATXRT = !string.IsNullOrEmpty(collection["AATXRT"]) && double.TryParse(collection["AATXRT"], out isNumeric) ? double.Parse(collection["AATXRT"]) : EmptyDouble;
                model.AASETRM = collection["AASETRM"];
                model.AADLPOT = collection["AADLPOT"];
                model.AARMK1 = collection["AARMK1"];
                model.AARMK2 = collection["AARMK2"];
                model.AARMK3 = collection["AARMK3"];
                model.AARMK4 = collection["AARMK4"];
                model.AARMK5 = collection["AARMK5"];


                model.AAEXRTT = collection["AAEXRT-sl"]; // dung cho xuat excel
                model.AATEXRT = collection["AATEXRT"];
                try
                {
                    while (n <= count_item)
                    {
                        str = ConcatString(n);
                        if (string.IsNullOrEmpty(collection["ABCTITM" + str]))
                        {
                            n++;
                            continue;
                        }

                        if (itemNo.Length != 0)
                        {
                            if (Array.Exists(itemNo, x => x.PURCODE.Value.ToString().Equals(collection["PURCODE" + str])))
                            {
                                var obj = itemNo.Where(x => x.PURCODE.Value.ToString().Equals(collection["PURCODE" + str])).Single();
                                model.CompletetionStatus = 1;
                                model.AACMPDT = obj.AACMPDT;
                                model.AACMPCD = "close";
                            }
                            else
                            {
                                model.CompletetionStatus = 0;
                                model.AACMPDT = null;
                                model.AACMPCD = "open";
                            }

                        }
                        else
                        {
                            model.CompletetionStatus = 0;
                            model.AACMPDT = null;
                            model.AACMPCD = "open";
                        }

                        model.ABCTITM = int.Parse(collection["ABCTITM" + str]);
                        model.ABMCSPC = collection["ABMCSPC" + str];
                        model.ABGRADE = collection["ABGRADE" + str];
                        model.ABCOAT = collection["ABCOAT" + str];
                        model.RAPSTLGR = collection["RAPSTLGR" + str];
                        model.ABDLVDT = !string.IsNullOrEmpty(collection["ABDLVDT" + str]) && DateTime.TryParse(collection["ABDLVDT"], out date) ? DateTime.Parse(collection["ABDLVDT" + str]) : DateTime.Now; // loi
                        model.ABWT = !string.IsNullOrEmpty(collection["ABWT" + str]) && decimal.TryParse(collection["ABWT" + str], out _decimal) ? decimal.Parse(collection["ABWT" + str]) : EmptyDemical;

                        //model.ABWT = !string.IsNullOrEmpty(collection["ABWT" + str]) && double.TryParse(collection["ABWT" + str], out isNumeric) ? double.Parse(collection["ABWT" + str]) : EmptyDouble;
                        // model.ABPRUP = !string.IsNullOrEmpty(collection["ABPRUP" + str]) && double.TryParse(collection["ABPRUP" + str], out isNumeric) ? double.Parse(collection["ABPRUP" + str]) : EmptyDouble;
                        // model.ABPRAT = !string.IsNullOrEmpty(collection["ABPRAT" + str]) && double.TryParse(collection["ABPRAT" + str], out isNumeric) ? double.Parse(collection["ABPRAT" + str]) : EmptyDouble;
                        model.ABPRUP = !string.IsNullOrEmpty(collection["ABPRUP" + str]) && decimal.TryParse(collection["ABPRUP" + str], out _decimal) ? decimal.Parse(collection["ABPRUP" + str]) : EmptyDemical;
                        model.ABPRAT = !string.IsNullOrEmpty(collection["ABPRAT" + str]) && decimal.TryParse(collection["ABPRAT" + str], out _decimal) ? decimal.Parse(collection["ABPRAT" + str]) : EmptyDemical;
                        model.ABPTXAT = !string.IsNullOrEmpty(collection["ABPTXAT" + str]) && decimal.TryParse(collection["ABPTXAT" + str], out _decimal) ? decimal.Parse(collection["ABPTXAT" + str]) : EmptyDemical;
                        model.ABBSZT = !string.IsNullOrEmpty(collection["ABBSZT" + str]) && double.TryParse(collection["ABBSZT" + str], out isNumeric) ? double.Parse(collection["ABBSZT" + str]) : EmptyDouble;
                        model.ABBSZW = !string.IsNullOrEmpty(collection["ABBSZW" + str]) && double.TryParse(collection["ABBSZW" + str], out isNumeric) ? double.Parse(collection["ABBSZW" + str]) : EmptyDouble;
                        model.ABBSZL = !string.IsNullOrEmpty(collection["ABBSZL" + str]) && double.TryParse(collection["ABBSZL" + str], out isNumeric) ? double.Parse(collection["ABBSZL" + str]) : EmptyDouble;
                        model.ABORDNO = collection["ABORDNO" + str];
                        model.ABPRDNM = collection["ABPRDNM" + str];
                        model.ABPRDDIA = collection["ABPRDDIA" + str];
                        model.ABQTY = !string.IsNullOrEmpty(collection["ABQTY" + str]) && double.TryParse(collection["ABQTY" + str], out isNumeric) ? double.Parse(collection["ABQTY" + str]) : EmptyDouble;
                        // model.ABPRUPD = !string.IsNullOrEmpty(collection["ABPRUPD" + str]) && double.TryParse(collection["ABPRUPD" + str], out isNumeric) ? double.Parse(collection["ABPRUPD" + str]) : EmptyDouble;
                        //  model.ABPRATD = !string.IsNullOrEmpty(collection["ABPRATD" + str]) && double.TryParse(collection["ABPRATD" + str], out isNumeric) ? double.Parse(collection["ABPRATD" + str]) : EmptyDouble;
                        model.ABPRUPD = !string.IsNullOrEmpty(collection["ABPRUPD" + str]) && decimal.TryParse(collection["ABPRUPD" + str], out _decimal) ? decimal.Parse(collection["ABPRUPD" + str]) : EmptyDemical;
                        model.ABPRATD = !string.IsNullOrEmpty(collection["ABPRATD" + str]) && decimal.TryParse(collection["ABPRATD" + str], out _decimal) ? decimal.Parse(collection["ABPRATD" + str]) : EmptyDemical;
                        model.ABPTXAD = !string.IsNullOrEmpty(collection["ABPTXAD" + str]) && decimal.TryParse(collection["ABPTXAD" + str], out _decimal) ? decimal.Parse(collection["ABPTXAD" + str]) : EmptyDemical;
                        model.WarehousingWeight = !string.IsNullOrEmpty(collection["WarehousingWeight" + str]) && decimal.TryParse(collection["WarehousingWeight" + str], out _decimal) ? decimal.Parse(collection["WarehousingWeight" + str]) : EmptyDemical;
                        if (!pUR001_DAL.Insert(model))
                        {
                            transaction.Dispose();
                            return Content("Cannot update data, please checking database !");
                        }

                        n++;
                    }

                    transaction.Complete();
                    TempData[ConstantData.Notification_key] = MessageSendView.MessageNotifi("update");
                    return RedirectToAction("GetPurchaseContractEntry");
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return Content("Some validation error is happen, please contact support for error: " + ex.Message);
                }
            }
        }
        public string IncreRevisionNo(string revision)
        {
            if (string.IsNullOrEmpty(revision) || string.IsNullOrWhiteSpace(revision))
            {
                revision = "00";
            }
            var num = int.Parse(revision);
            string rs;
            num++;

            if (num < 10)
            {
                rs = string.Concat("0", num);
            }
            else
            {
                rs = num.ToString();
            }
            return rs;
        }
        public ActionResult CheckSpNoExists(string spNo)
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            return Json(dal.CheckSpNoExists(spNo), JsonRequestBehavior.AllowGet);
        }
        private string ConcatString(int n)
        {
            StringBuilder rs;
            if (n > 1)
            {
                rs = new StringBuilder();
                rs.Append("_" + n);
            }
            else
            {
                rs = new StringBuilder();
                rs.Append(string.Empty);
            }
            return rs.ToString();
        }
        private string CreateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            var rs = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            if (!dal.CheckCodeCreateExists(rs)) return CreateCode();
            return rs;
        }

        public ActionResult AddItem()
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            return PartialView("_PurchaseContractEntry", dal.GetReferences(new PUR001()));
        }

        public void CreateSelectListItem()
        {
            ViewBag.Grade = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("015")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.MakerCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("005")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.CurrencyCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("012")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.PriceTerm = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("019")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.TypeOfTerm = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("021")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.CommodityCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("006")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.ContractType = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("025")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.SettelementTerm = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("020")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.TaxCode = MasterdataEntities.MA010.OrderByDescending(x => x.MKRGSDT).Select(x => new { x.MKTXCD, x.MKTXRT }).ToList();
            ViewBag.ExchangeRate = MasterdataEntities.MA009.OrderByDescending(x => x.MJRGSDT).Select(x => new { x.MJEXRT, x.MJTXEXR, x.MJEXRTT }).ToList();
            ViewBag.Spec = MasterdataEntities.MA006.OrderByDescending(x => x.MFRGSDT).Select(x => new { x.MFPRDSP }).ToList();
            ViewBag.Coating = MasterdataEntities.MA005.OrderByDescending(x => x.MERGSDT).Select(x => new { x.MECOAT });
            ViewBag.UserCode = MasterdataEntities.MA002.Select(x => new { x.MBUSRCD, x.MBUSRNM }).ToList();
            ViewBag.RawMaterialType = MasterdataEntities.RawMaterialTypes.ToList();
            ViewBag.SteelGrade = MasterdataEntities.SteelGrades.ToList();
            ViewBag.SupplierCodes = MasterdataEntities.MA001.Select(x => new { x.MASPCD, x.MASPNM }).ToList();

        }

        public ActionResult GetPurchaseOrderBalanceEnquiry()
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            return View(dal.GetReferences(new TopProSystem.Areas.MasterSetting.Models.PUR001()));
        }

        protected Dictionary<string, string> TempdSearch(Dictionary<string, string> dSearch = null)
        {
            if (dSearch != null)
            {
                Session["dSearch"] = dSearch; // bien tam dung cho xuat excel
            }
            return Session["dSearch"] as Dictionary<string, string>;
        }

        [HttpPost]
        public JsonResult AjaxHandlerOrderBalanceEnquiry(jQueryDataTableParamModel param, bool completed = false)
        {
            var _array = !string.IsNullOrEmpty(param.sSearch) ? param.sSearch.Split('|') : new string[18];
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("AACMPCD", _array[0]);
            dictionary.Add("AAMKCD", _array[1]);
            dictionary.Add("AASPPNO", _array[2]);
            dictionary.Add("AASPLCD", _array[3]);
            dictionary.Add("AAUSRCD", _array[4]);
            dictionary.Add("AAPURNO", _array[5]);
            dictionary.Add("AASHPDT", _array[6]);
            dictionary.Add("AACMDCD", _array[7]);
            dictionary.Add("AACTRTP", _array[8]);
            dictionary.Add("AAIDCD", _array[9]);
            dictionary.Add("AARMTP", _array[10]);
            dictionary.Add("RAPSTLGR", _array[11]);
            dictionary.Add("ABMCSPC", _array[12]);
            dictionary.Add("ABPRDNM", _array[13]);
            dictionary.Add("ABPRDDIA", _array[14]);
            dictionary.Add("ABBSZT", _array[15]);
            dictionary.Add("ABBSZW", _array[16]);
            dictionary.Add("ABBSZL", _array[17]);

            TempdSearch(dictionary);

            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            var ma001_dal = new Areas.MasterSetting.DAL.MA001.MA001_DAL();
            var ma002_dal = new Areas.MasterSetting.DAL.MA002.MA002_DAL();
            var ma003_dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();

            var countRecord = dal.GetTotalRecord(dictionary, completed: completed);
            var displayItem = dal.GetTotalDisplayRecord(dictionary, param.iDisplayStart, param.iDisplayLength, completed: completed);
            var result = displayItem.Select(x => new
            {
                AAPURNO = x.AAPURNO,
                ABCTITM = x.ABCTITM,
                AASPPNO = x.AASPPNO,
                AASPLCD = x.AASPLCD,
                AACTRNM = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE025, x.AACTRTP),
                AASPLNM = ma001_dal.GetSalePurchase(x.AASPLCD) != null ? ma001_dal.GetSalePurchase(x.AASPLCD).MASPNM : string.Empty,
                AAUSRNM = ma002_dal.GetUserName(x.AAUSRCD),
                AACMDNM = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE006, x.AACMDCD),
                AAMKNM = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE005, x.AAMKCD),
                ABMCSPC = x.ABMCSPC,
                ABBSZT = x.ABBSZT,
                ABBSZW = x.ABBSZW,
                ABBSZL = x.ABBSZL,
                ABPRDNM = x.ABPRDNM,
                ABPRDDIA = x.ABPRDDIA,
                AASHPDT = x.AASHPDT,
                AAIDCD = ma003_dal.GetMA003(x.AAIDCD) != null ? ma003_dal.GetMA003(x.AAIDCD).MCIDNM : string.Empty,
                ABDLVDT = String.Format("{0:dd/MM/yyyy}", x.ABDLVDT),
                ABWT = x.ABWT,
                ABQTY = x.ABQTY,
                WAREWEIGTH = x.WarehousingWeight,
                BalanceWeight = x.ABWT - x.WarehousingWeight,
                RAPSTLGR = x.RAPSTLGR,
                PURCODE = x.PURCODE // private key for item
            });

            return Json(new
            {
                iTotalRecords = countRecord,
                iTotalDisplayRecords = countRecord,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public enum TypeOfOrderCompletionOrEnquiry
        {
            OrderCompletion = 1,
            OrderBanlanceEnquiry = 2
        }

        [HttpPost]
        public JsonResult ExportPurchaseOrderBalanceEnquiryToExcel(TypeOfOrderCompletionOrEnquiry type, bool completed = false) // ham nay in 2 loai file nhu tren enum
        {
            try
            {
                CreateSpecialFolder("excel");
                var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
                var ma001_dal = new Areas.MasterSetting.DAL.MA001.MA001_DAL();
                var ma002_dal = new Areas.MasterSetting.DAL.MA002.MA002_DAL();
                var ma010_dal = new Areas.MasterSetting.DAL.MA010.MA010_DAL();
                var ma012_dal = new Areas.MasterSetting.DAL.MA012.MA012_DAL();

                IEnumerable<PUR001> dataDisplayNoPaging;
                if (completed == false)
                {
                    dataDisplayNoPaging = dal.GetTotalDisplayRecord(TempdSearch(), 0, 0, paging: false); // lay du lieu hien thi khong phan trang
                }
                else
                {
                    dataDisplayNoPaging = dal.GetTotalDisplayRecord(TempdSearch(), 0, 0, paging: false, completed: true); // lay du lieu hien thi khong phan trang
                }

                if (dataDisplayNoPaging.Count() == 0)
                {
                    return Json(ExportFileResult.Nodata, JsonRequestBehavior.AllowGet);
                }

                var result = dataDisplayNoPaging.Select(x => new
                {
                    AAPURNO = x.AAPURNO,
                    ABCTITM = x.ABCTITM,
                    AACTRTP = x.AACTRTP,
                    AACTRNM = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE025, x.AACTRTP),
                    AASPLCD = x.AASPLCD,
                    AASPLNM = ma001_dal.GetSalePurchase(x.AASPLCD) != null ? ma001_dal.GetSalePurchase(x.AASPLCD).MASPNM : string.Empty,
                    AAUSRCD = x.AAUSRCD,
                    AAUSRNM = ma002_dal.GetUserName(x.AAUSRCD),
                    AACMDCD = x.AACMDCD,
                    AACMDNM = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE006, x.AACMDCD),
                    AAMKCD = x.AAMKCD,
                    AAMKNM = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE005, x.AAMKCD),
                    ABMCSPC = x.ABMCSPC,
                    ABBSZT = x.ABBSZT,
                    ABBSZW = x.ABBSZW,
                    ABBSZL = x.ABBSZL,
                    ABPRDNM = x.ABPRDNM,
                    ABPRDDIA = x.ABPRDDIA,
                    AASHPDT = x.AASHPDT,
                    ABDLVDT = String.Format("{0:dd/MM/yyyy}", x.ABDLVDT),
                    ABWT = x.ABWT,
                    AASPPNO = x.AASPPNO,
                    AACRRCD = x.AACRRCD,
                    AAEXRTT = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE018, x.AAEXRTT),
                    AATXCD = ma010_dal.GetMA010ByMKTXCD(x.AATXCD).MKTXDL,
                    AACMPCD = x.AACMPCD,
                    AADLVCD = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE024, x.AADLVCD),
                    AASETRM = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE020, x.AASETRM),
                    AAPRICE = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE019, x.AAPRICE),
                    AAPTTRM = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE021, x.AAPTTRM),
                    AADLPOT = x.AADLPOT,
                    AARMK1 = x.AARMK1,
                    AARMK2 = x.AARMK2,
                    AARMK3 = x.AARMK3,
                    AARMK4 = x.AARMK4,
                    AARMK5 = x.AARMK5,
                    ABCOAT = x.ABCOAT,
                    ABGRADE = ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE015, x.ABGRADE),
                    ABORDNO = x.ABORDNO,
                    WarehousingWeight = x.WarehousingWeight,
                    BalanceWeight = x.ABWT - x.WarehousingWeight

                });

                var listData = new List<string[]>();
                foreach (var data in result)
                {
                    listData.Add(new string[]
                    {
                    data.AAPURNO,
                    data.ABCTITM.ToString(),
                    data.AACTRTP,
                    data.AACTRNM,
                    data.AASPLCD,
                    data.AASPLNM,
                    data.AAUSRCD,
                    data.AAUSRNM,
                    data.AACMDCD,
                    data.AACMDNM,
                    data.AAMKCD,
                    data.AAMKNM,
                    data.ABMCSPC,
                    data.ABBSZT.ToString(),
                    data.ABBSZW.ToString(),
                    data.ABBSZL.ToString(),
                    data.ABPRDNM,
                    data.ABPRDDIA,
                    data.AASHPDT,
                    data.ABDLVDT,
                    data.ABWT.ToString(),
                    data.WarehousingWeight.ToString(),
                    data.BalanceWeight.ToString(),
                    data.AASPPNO,
                    data.AACRRCD,
                    data.AAEXRTT,
                    data.AATXCD,
                    data.AACMPCD,
                    data.AADLVCD,
                    data.AASETRM,
                    data.AAPRICE,
                    data.AAPTTRM,
                    data.AADLPOT,
                    data.AARMK1,
                    data.AARMK2,
                    data.AARMK3,
                    data.AARMK4,
                    data.AARMK5,
                    data.ABCOAT,
                    data.ABGRADE,
                    data.ABORDNO,

                    });
                }
                string filename = string.Empty;
                switch (type)
                {
                    case TypeOfOrderCompletionOrEnquiry.OrderBanlanceEnquiry:
                        filename = "PurchaseOrderBalanceEnquiry_" + FileNameExtension + ".xlsx";
                        break;
                    case TypeOfOrderCompletionOrEnquiry.OrderCompletion:
                        filename = "PurchaseOrderCompletion_" + FileNameExtension + ".xlsx";
                        break;
                    default:
                        filename = "PurchaseOrderBalanceEnquiry_" + FileNameExtension + ".xlsx";
                        break;
                }

                string path = System.IO.Path.GetFullPath(PurchaseExcelFilePath + filename);
                ExcelPackage ExcelFile = new ExcelPackage(new FileInfo(path));
                var workSheet = ExcelFile.Workbook.Worksheets.Add("Sheet1");


                int row = 1, column = 1;
                var header = new List<string>()
                {
                    "Purchase Contract No",
                    "Item No",
                    "Contract Type",
                    "Contract",
                    "Supplier Code",
                    "Supplier Name",
                    "User Code",
                    "User Name",
                    "Commodity",
                    "Commodity Name",
                    "Maker Code",
                    "Maker Name",
                    "Spec",
                    "Thick",
                    "Width",
                    "Length",
                    "Product Name",
                    "Product Diameter",
                    "Shipping Month",
                    "Delivery Date",
                    "Purchase Weight",
                    "Warehouse Weigth",
                    "Balance Weight",
                    "Sale contract no",
                    "currency code",
                    "exchange rate type",
                    "tax",
                    "Completion code",
                    "delivery condition",
                    "settlement term",
                    "price term",
                    "type of terms",
                    "dischrg & loading port",
                    "remark 1",
                    "remark 2",
                    "remark 3",
                    "remark 4",
                    "remark 5",
                    "coating",
                    "grade",
                    "Order No",

                };

                foreach (var title in header)
                {
                    using (var range = workSheet.Cells[row, column])
                    {
                        range.Value = title.ToUpper();
                        range.AutoFitColumns();
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(0, 221, 235, 247);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        workSheet.Row(row).Height = 20;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.Font.Bold = true;
                    }
                    column++;
                }

                row++;
                foreach (var array in listData)
                {
                    column = 1;
                    foreach (var val in array)
                    {
                        using (var range = workSheet.Cells[row, column])
                        {
                            if (val == null)
                            {
                                range.Value = "";
                            }
                            else
                            {
                                range.Value = val.Trim();
                            }
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                        column++;
                    }
                    row++;
                }

                ExcelFile.Save();
                return Json(path, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(ExportFileResult.Fail, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetPurchaseOrderCompletion()
        {
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            return View(dal.GetReferences(new PUR001()));
        }
        public JsonResult ChangeCompletionStatus(string _array)
        {
            string[] array = null;
            if (!string.IsNullOrEmpty(_array))
            {
                array = _array.Split('|');
            }
            if (array != null)
            {
                var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
                foreach (var key in array)
                {
                    if (!dal.ChangeCompletionStatusOfItem(key))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        //Purchase History Enquiry
        public Image CreateWaterMarkLogoImg(String ImgPath, float xx, float yy, float x, float y)
        {
            Image logo = Image.GetInstance(ImgPath);
            logo.ScaleToFit(xx, yy);
            logo.SetAbsolutePosition(x, y);
            return logo;
        }
        public static PdfPCell FormatCell(int border = 0, Paragraph word = null, Image img = null, int alignMent = PdfPCell.ALIGN_LEFT)
        {
            PdfPCell cell = null;
            if (word == null) cell = new PdfPCell(img);
            else cell = new PdfPCell(word);
            cell.Border = border;
            cell.HorizontalAlignment = alignMent;
            return cell;
        }

        public PdfPTable TabelFormat(float percentWidth = 100f, float[] widthColumn = null, int columns = 1, float spacingBefore = 0)
        {
            PdfPTable table = new PdfPTable(columns);
            table.WidthPercentage = percentWidth;
            table.SetWidths(widthColumn);
            table.SpacingBefore = spacingBefore;
            return table;
        }

        public Paragraph ParagraphFormat(string word, float spacingBefore = 0, int alignMent = Element.ALIGN_LEFT, Font font = null)
        {
            Paragraph par = new Paragraph(word, font);
            par.SpacingBefore = spacingBefore;
            par.Alignment = alignMent;
            return par;
        }

        public ActionResult GetPurchaseHistoryEnquiry()
        {
            return View();
        }

        public void CreatePublicPurchase(Document doc, PdfPTable table, PdfWriter writer, string tieude) // pdf format for purchase
        {
            float[] widths1 = new float[] { 8f, 46f, 46f };

            table = TabelFormat(widthColumn: widths1, columns: 3);

            table.HorizontalAlignment = Element.ALIGN_RIGHT;

            //cell11
            string LogoImgPath = Server.MapPath("~/Images/PdfFileimg/TopProLogo.png");
            Image logo = Image.GetInstance(LogoImgPath);

            PdfPCell celli = FormatCell(alignMent: Element.ALIGN_LEFT, img: logo);

            table.AddCell(celli);

            //cel12
            var FontColor = new BaseColor(101, 100, 255);
            Font FHeader = FontFactory.GetFont("Franklin Gothic", 35, Font.BOLD, FontColor);
            PdfPCell cell11 = FormatCell(word: new Paragraph("Top Pro Steel Group", FHeader));
            table.AddCell(cell11);

            //cell13
            Font FSubHeader = FontFactory.GetFont("Franklin Gothic", 12, Font.BOLD, BaseColor.BLACK);
            PdfPCell cell12 = FormatCell(word: new Paragraph("Date: " + DateTime.Now.ToShortDateString() + "\nPage: XX/XX", FSubHeader), alignMent: PdfPCell.ALIGN_RIGHT);
            table.AddCell(cell12);
            doc.Add(table);

            Paragraph Address = ParagraphFormat(word: "Lot 12, Long Dinh Industrial Zone - Long Cang, H. Can Duoc, T. Long An \nTel: +072 3 637295 Fax: 072 3 634 998", font: FSubHeader);
            doc.Add(Address);

            Font FTieude = FontFactory.GetFont("Franklin Gothic", 20, Font.BOLD, BaseColor.BLACK);
            Paragraph Tieude = ParagraphFormat(word: tieude, font: FTieude, spacingBefore: 10f, alignMent: Element.ALIGN_CENTER);
            doc.Add(Tieude);

            PdfContentByte canvas = writer.DirectContentUnder;

            string SignImgPath = Server.MapPath("~/Images/PdfFileImg/SignBox.png");

            Image signbox = CreateWaterMarkLogoImg(SignImgPath, 280, 180, doc.PageSize.Right - 310, doc.PageSize.Top - 160);

            canvas.AddImage(signbox);
        }

        #region PDF

        public string CreatePurchaseHistoryEnquirybByMakerFile()
        {
            string message = String.Empty;
            try
            {
                int COUNT_DATA = 19;
                int COlUMN = 28;
                //Create a document object
                Document doc = new Document(PageSize.A3.Rotate(), 20f, 20f, 30f, 0);

                string filename = "Purchase_History_By_Maker_" + FileNameExtension + ".pdf";
                // String path = Server.MapPath('~' + PurchasePDFFilePath + filename);
                string path = System.IO.Path.GetFullPath(PurchasePDFFilePath + filename);
                //get a PDFWriter object    
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                //open the document for writting
                doc.Open();

                //table 1
                PdfPTable table = null;
                CreatePublicPurchase(doc, table, writer, "Purchase History By Maker");

                //table 3 
                Font FColumnName = FontFactory.GetFont("Franklin Gothic", 9, Font.NORMAL, BaseColor.BLACK);

                float[] widths3 = new float[] { 3f, 8f, 9f, 9f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f };

                PdfPTable TbThird = TabelFormat(columns: 28, spacingBefore: 40f, widthColumn: widths3, percentWidth: 100f);

                List<string> ColumnName = new List<string>() { "Year", "Maker", "Total Perchase Weight", "Total Perchase Weight", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };

                //PdfPCell cell = null;
                for (int i = 1; i <= COUNT_DATA; i++)
                {
                    foreach (string j in ColumnName)
                    {
                        PdfPCell cell;
                        if (i == 1)
                        {
                            cell = new PdfPCell(new Paragraph(j, FColumnName));
                        }
                        else
                        {
                            cell = new PdfPCell();
                        }
                        cell.FixedHeight = 30f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.BorderWidth = 1;
                        TbThird.AddCell(cell);
                    }
                }
                doc.Add(TbThird);

                //Table 4
                PdfPTable TbFour = TabelFormat(columns: COlUMN, widthColumn: widths3, spacingBefore: 30f);

                PdfPCell cell4_11 = new PdfPCell();
                cell4_11.Border = 0;
                TbFour.AddCell(cell4_11);

                PdfPCell cell4_12 = FormatCell(word: new Paragraph("Total", FColumnName), border: PdfPCell.BOX, alignMent: PdfPCell.ALIGN_CENTER);
                cell4_12.FixedHeight = 30f;
                cell4_12.VerticalAlignment = Element.ALIGN_MIDDLE;
                TbFour.AddCell(cell4_12);

                for (int i = 1; i <= 1; i++)
                {
                    for (int j = 3; j <= COlUMN; j++)
                    {
                        PdfPCell cell = new PdfPCell();
                        cell.BorderWidth = 1;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        TbFour.AddCell(cell);
                    }
                }

                doc.Add(TbFour);
                //close the document
                doc.Close();
                //message = "success" + '|' + PurchasePDFFilePath + filename;
                return message = SuccessMessage + '|' + path;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
        public string CreatePurchaseHistoryByMakerByCommodityFile()
        {
            var message = String.Empty;
            try
            {
                int COUNT_DATA = 21;
                int COlUMN = 29;

                Document doc = new Document(PageSize.A3.Rotate(), 20f, 20f, 30f, 0);

                string filename = "Purchase_History_By_Maker_By_Commodity_" + FileNameExtension + ".pdf";
                // string path = Server.MapPath('~' + PurchasePDFFilePath + filename);
                string path = System.IO.Path.GetFullPath(PurchasePDFFilePath + filename);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));

                doc.Open();

                //table 1
                PdfPTable table = null;
                CreatePublicPurchase(doc, table, writer, "Purchase History By Maker By Commodity");

                //table 3 
                Font FColumnName = FontFactory.GetFont("Franklin Gothic", 9, Font.NORMAL, BaseColor.BLACK);

                float[] widths3 = new float[] { 3f, 8f, 6f, 6f, 6f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f };

                PdfPTable TbThird = TabelFormat(columns: 29, spacingBefore: 40f, widthColumn: widths3, percentWidth: 100f);

                List<string> ColumnName = new List<string>() { "Year", "Maker", "Commodity", "Total Perchase Weight", "Total Perchase Weight", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };

                //PdfPCell cell = null;

                for (int i = 1; i <= COUNT_DATA; i++)
                {
                    foreach (string j in ColumnName)
                    {
                        PdfPCell cell;
                        if (i == 1)
                        {
                            cell = new PdfPCell(new Paragraph(j, FColumnName));
                        }
                        else
                        {
                            cell = new PdfPCell();
                        }
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.BorderWidth = 1;
                        TbThird.AddCell(cell);
                    }
                }
                doc.Add(TbThird);

                //Table 4
                PdfPTable TbFour = TabelFormat(columns: COlUMN, spacingBefore: 30f, widthColumn: widths3, percentWidth: 100f);

                for (int i = 1; i <= 3; i++)
                {
                    for (int j = 1; j <= COlUMN; j++)
                    {

                        PdfPCell cell = new PdfPCell();
                        cell.BorderWidth = 1;
                        if (j == 1 || j == 3)
                        {
                            cell.Border = 0;
                        }
                        else if (j == 2)
                        {
                            switch (i)
                            {
                                case 1:
                                    cell = new PdfPCell(new Paragraph("Commondity Total", FColumnName));
                                    break;
                                case 2:
                                    cell = new PdfPCell(new Paragraph("Maker Total", FColumnName));
                                    break;
                                case 3:
                                    cell = new PdfPCell(new Paragraph("Grand Total", FColumnName));
                                    break;
                            }
                        }
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        TbFour.AddCell(cell);
                    }
                }

                doc.Add(TbFour);
                doc.Close();
                message = SuccessMessage + '|' + PurchasePDFFilePath + filename;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().ToString();
            }
            return message;
        }
        public string CreateHistoryByMakerByCommodityBySpecBySizeFile()
        {
            var message = String.Empty;
            try
            {
                int COUNT_DATA = 19;
                int COlUMN = 31;
                //Create a document object
                Document doc = new Document(PageSize.A3.Rotate(), 20f, 20f, 30f, 0);

                string filename = "Purchase_History_By_Maker_By_Commodity_By_Spec_By_Size_" + FileNameExtension + ".pdf";
                // string path = Server.MapPath('~' + PurchasePDFFilePath + filename);
                string path = System.IO.Path.GetFullPath(PurchasePDFFilePath + filename);

                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();

                //table 1
                PdfPTable table = null;
                CreatePublicPurchase(doc, table, writer, "Purchase History By Maker By Commodity By Spec By Size");

                //table 3 
                Font FColumnName = FontFactory.GetFont("Franklin Gothic", 9, Font.NORMAL, BaseColor.BLACK);

                float[] widths3 = new float[] { 3f, 6f, 5f, 3f, 4f, 4f, 4f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f };

                PdfPTable TbThird = TabelFormat(columns: 31, spacingBefore: 40f, widthColumn: widths3, percentWidth: 100f);

                List<string> ColumnName = new List<string>() { "Year", "Maker", "Commodity", "Spec", "Size", "Total Perchase Weight", "Total Perchase Weight", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };

                //PdfPCell cell = null;

                for (int i = 1; i <= COUNT_DATA; i++)
                {
                    foreach (string j in ColumnName)
                    {
                        PdfPCell cell;
                        if (i == 1)
                        {
                            cell = new PdfPCell(new Paragraph(j, FColumnName));
                            cell.FixedHeight = 35f;
                        }
                        else
                        {
                            cell = new PdfPCell();
                            cell.FixedHeight = 25f;
                        }

                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.BorderWidth = 1;
                        TbThird.AddCell(cell);
                    }
                }
                doc.Add(TbThird);

                //Table 4
                PdfPTable TbFour = TabelFormat(columns: COlUMN, spacingBefore: 30f, widthColumn: widths3, percentWidth: 100f);

                for (int i = 1; i <= 3; i++)
                {
                    for (int j = 1; j <= COlUMN; j++)
                    {

                        PdfPCell cell = new PdfPCell();
                        cell.BorderWidth = 1;
                        if (j == 1 || j == 3 || j == 4 || j == 5)
                        {
                            cell.Border = 0;
                        }
                        else if (j == 2)
                        {
                            switch (i)
                            {
                                case 1:
                                    cell = new PdfPCell(new Paragraph("Commondity Total", FColumnName));
                                    break;
                                case 2:
                                    cell = new PdfPCell(new Paragraph("Maker Total", FColumnName));
                                    break;
                                case 3:
                                    cell = new PdfPCell(new Paragraph("Grand Total", FColumnName));
                                    break;
                            }
                        }
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        TbFour.AddCell(cell);
                    }
                }

                doc.Add(TbFour);
                doc.Close();
                message = SuccessMessage + '|' + PurchasePDFFilePath + filename;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().ToString();
            }
            return message;
        }
        public string CreateHistoryBySupplierFile()
        {
            var message = String.Empty;
            try
            {
                int COUNT_DATA = 19;
                int COlUMN = 28;

                Document doc = new Document(PageSize.A3.Rotate(), 20f, 20f, 30f, 0);

                string filename = "Purchase_History_By_Supplier_" + FileNameExtension + ".pdf";
                //  string path = Server.MapPath('~' + PurchasePDFFilePath + filename);
                string path = System.IO.Path.GetFullPath(PurchasePDFFilePath + filename);

                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();

                //table 1
                PdfPTable table = null;
                CreatePublicPurchase(doc, table, writer, "Purchase History By Supplier");

                //table 3 
                Font FColumnName = FontFactory.GetFont("Franklin Gothic", 9, Font.NORMAL, BaseColor.BLACK);

                float[] widths3 = new float[] { 3f, 8f, 9f, 9f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f };

                PdfPTable TbThird = TabelFormat(columns: 28, spacingBefore: 40f, widthColumn: widths3, percentWidth: 100f);

                List<string> ColumnName = new List<string>() { "Year", "Supplier", "Total Perchase Weight", "Total Perchase Weight", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };

                //PdfPCell cell = null;

                for (int i = 1; i <= COUNT_DATA; i++)
                {
                    foreach (string j in ColumnName)
                    {
                        PdfPCell cell;
                        if (i == 1)
                        {
                            cell = new PdfPCell(new Paragraph(j, FColumnName));
                        }
                        else
                        {
                            cell = new PdfPCell();
                        }
                        cell.FixedHeight = 30f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.BorderWidth = 1;
                        TbThird.AddCell(cell);
                    }
                }
                doc.Add(TbThird);

                //Table 4
                PdfPTable TbFour = TabelFormat(columns: COlUMN, spacingBefore: 30f, widthColumn: widths3, percentWidth: 100f);

                PdfPCell cell4_11 = new PdfPCell();
                cell4_11.Border = 0;
                TbFour.AddCell(cell4_11);

                PdfPCell cell4_12 = FormatCell(word: new Paragraph("Total", FColumnName), alignMent: PdfPCell.ALIGN_CENTER, border: PdfPCell.BOX); new PdfPCell();
                cell4_12.FixedHeight = 30f;
                cell4_12.VerticalAlignment = Element.ALIGN_MIDDLE;
                TbFour.AddCell(cell4_12);

                for (int i = 1; i <= 1; i++)
                {
                    for (int j = 3; j <= COlUMN; j++)
                    {
                        PdfPCell cell = new PdfPCell();
                        cell.BorderWidth = 1;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        TbFour.AddCell(cell);
                    }
                }

                doc.Add(TbFour);
                doc.Close();
                message = SuccessMessage + '|' + PurchasePDFFilePath + filename;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().ToString();
            }
            return message;
        }
        public string CreateHistoryBySupplierByCommodityFile()
        {
            var message = "";
            try
            {
                int COUNT_DATA = 21;
                int COlUMN = 29;

                Document doc = new Document(PageSize.A3.Rotate(), 20f, 20f, 30f, 0);

                string filename = "Purchase_History_By_Supplier_By_Commodity_" + FileNameExtension + ".pdf";
                //string path = Server.MapPath('~' + PurchasePDFFilePath + filename);
                string path = System.IO.Path.GetFullPath(PurchasePDFFilePath + filename);

                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));

                doc.Open();

                //table 1
                PdfPTable table = null;
                CreatePublicPurchase(doc, table, writer, "Purchase History By Supplier By Commodity");

                //table 3 
                Font FColumnName = FontFactory.GetFont("Franklin Gothic", 9, Font.NORMAL, BaseColor.BLACK);

                float[] widths3 = new float[] { 3f, 8f, 6f, 6f, 6f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f };

                PdfPTable TbThird = TabelFormat(columns: 29, spacingBefore: 40f, widthColumn: widths3, percentWidth: 100f);

                List<string> ColumnName = new List<string>() { "Year", "Supplier", "Commodity", "Total Perchase Weight", "Total Perchase Weight", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };

                //PdfPCell cell = null;

                for (int i = 1; i <= COUNT_DATA; i++)
                {
                    foreach (string j in ColumnName)
                    {
                        PdfPCell cell;
                        if (i == 1)
                        {
                            cell = new PdfPCell(new Paragraph(j, FColumnName));
                        }
                        else
                        {
                            cell = new PdfPCell();
                        }
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.BorderWidth = 1;
                        TbThird.AddCell(cell);
                    }
                }
                doc.Add(TbThird);

                //Table 4
                PdfPTable TbFour = TabelFormat(columns: COlUMN, spacingBefore: 30f, widthColumn: widths3, percentWidth: 100f);

                for (int i = 1; i <= 3; i++)
                {
                    for (int j = 1; j <= COlUMN; j++)
                    {

                        PdfPCell cell = new PdfPCell();
                        cell.BorderWidth = 1;
                        if (j == 1 || j == 3)
                        {
                            cell.Border = 0;
                        }
                        else if (j == 2)
                        {
                            switch (i)
                            {
                                case 1:
                                    cell = new PdfPCell(new Paragraph("Commondity Total", FColumnName));
                                    break;
                                case 2:
                                    cell = new PdfPCell(new Paragraph("Maker Total", FColumnName));
                                    break;
                                case 3:
                                    cell = new PdfPCell(new Paragraph("Grand Total", FColumnName));
                                    break;
                            }
                        }
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        TbFour.AddCell(cell);
                    }
                }

                doc.Add(TbFour);
                doc.Close();
                message = SuccessMessage + '|' + PurchasePDFFilePath + filename;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().ToString();
            }
            return message;
        }
        public string CreateHistoryBySupplierByCommodityBySpecBySizeFile()
        {
            var message = "";
            try
            {
                int COUNT_DATA = 19;
                int COlUMN = 31;
                //Create a document object
                Document doc = new Document(PageSize.A3.Rotate(), 20f, 20f, 30f, 0);

                string filename = "Purchase_History_By_Supplier_By_Commodity_By_Spec_By_Size_" + FileNameExtension + ".pdf";

                // String path = Server.MapPath('~' + PurchasePDFFilePath + filename);
                string path = System.IO.Path.GetFullPath(PurchasePDFFilePath + filename);
                //get a PDFWriter object    
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                //open the document for writting
                doc.Open();

                //table 1
                PdfPTable table = null;
                CreatePublicPurchase(doc, table, writer, "Purchase History By Supplier By Commodity By Spec By Size");

                //table 2
                Font FColumnName = FontFactory.GetFont("Franklin Gothic", 9, Font.NORMAL, BaseColor.BLACK);

                float[] widths3 = new float[] { 3f, 6f, 5f, 3f, 4f, 4f, 4f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f };

                PdfPTable TbThird = TabelFormat(columns: 31, spacingBefore: 40f, widthColumn: widths3, percentWidth: 100f);

                List<string> ColumnName = new List<string>() { "Year", "Supplier", "Commodity", "Spec", "Size", "Total Perchase Weight", "Total Perchase Weight", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };

                //PdfPCell cell = null;

                for (int i = 1; i <= COUNT_DATA; i++)
                {
                    foreach (string j in ColumnName)
                    {
                        PdfPCell cell;
                        if (i == 1)
                        {
                            cell = new PdfPCell(new Paragraph(j, FColumnName));
                            cell.FixedHeight = 35f;
                        }
                        else
                        {
                            cell = new PdfPCell();
                            cell.FixedHeight = 25f;
                        }

                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.BorderWidth = 1;
                        TbThird.AddCell(cell);
                    }
                }
                doc.Add(TbThird);

                //Table 4
                PdfPTable TbFour = TabelFormat(columns: COlUMN, spacingBefore: 30f, widthColumn: widths3, percentWidth: 100f);

                for (int i = 1; i <= 3; i++)
                {
                    for (int j = 1; j <= COlUMN; j++)
                    {

                        PdfPCell cell = new PdfPCell();
                        cell.BorderWidth = 1;
                        if (j == 1 || j == 3 || j == 4 || j == 5)
                        {
                            cell.Border = 0;
                        }
                        else if (j == 2)
                        {
                            switch (i)
                            {
                                case 1:
                                    cell = new PdfPCell(new Paragraph("Commondity Total", FColumnName));
                                    break;
                                case 2:
                                    cell = new PdfPCell(new Paragraph("Maker Total", FColumnName));
                                    break;
                                case 3:
                                    cell = new PdfPCell(new Paragraph("Grand Total", FColumnName));
                                    break;
                            }
                        }
                        cell.FixedHeight = 25f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        TbFour.AddCell(cell);
                    }
                }

                doc.Add(TbFour);
                doc.Close();
                message = "success" + '|' + PurchasePDFFilePath + filename;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().ToString();
            }
            return message;
        }
        #endregion

        #region EXCEL
        public void CreatePublicPurchaseExcelFile(ExcelWorksheet workSheet, int columnData, string reportName) // excel format for purchase
        {
            var img = workSheet.Drawings.AddPicture("logo", new FileInfo(Server.MapPath("~/Images/PdfFileImg/logotoppro.png")));
            img.SetPosition(1, 1, 1, 1);

            using (var range = workSheet.Cells[9, 2, 9, columnData])
            {
                range.Merge = true;
                range.Value = reportName;
                workSheet.Row(9).Height = 30;
                range.Style.Font.SetFromFont(new System.Drawing.Font("Myriad Pro", 25, System.Drawing.FontStyle.Bold));
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            using (var range = workSheet.Cells[2, columnData - 1, 2, columnData])
            {
                range.Merge = true;
                range.Value = "Date:" + DateTime.Today.ToShortDateString();
                range.Style.Font.Size = 14f;
                range.Style.Font.Bold = true;
            }

            using (var range = workSheet.Cells[3, columnData - 1, 3, columnData - 1])
            {
                range.Value = "Page: " + 1;
                range.Style.Font.Size = 14f;
                range.Style.Font.Bold = true;
            }

            img = workSheet.Drawings.AddPicture("signbox", new FileInfo(Server.MapPath("~/Images/PdfFileImg/SignBox.png")));
            img.SetPosition(4, 1, columnData - 5, 0);
            img.SetSize(330, 120);
        }
        public string CreatePurchaseHistoryEnquirybByMakerFileExcel()
        {
            try
            {
                string filename = "Purchase_History_By_Maker_" + FileNameExtension + ".xlsx";

                string path = System.IO.Path.GetFullPath(PurchaseExcelFilePath + filename);
                //  string path = Server.MapPath('~' + PurchaseExcelFilePath + filename);
                ExcelPackage ExcelFile = new ExcelPackage(new FileInfo(path));

                var workSheet = ExcelFile.Workbook.Worksheets.Add("Sheet1");
                int columnData = 29;

                CreatePublicPurchaseExcelFile(workSheet, columnData, "Purchase History By Maker");

                //workSheet.DefaultRowHeight = 12;
                workSheet.DefaultColWidth = 10;
                workSheet.Cells.Style.WrapText = true;
                workSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Protection.IsProtected = true;

                List<string> ColumnName = new List<string>() { "Year", "Maker", "Total Perchase Weight", "Total Purchase Amount", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };
                var Model = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }; // data

                int firstRow = 12; //begin from row 12
                foreach (int a in Model)
                {
                    for (int i = 1; i <= ColumnName.Count; i++)
                    {
                        using (var range = workSheet.Cells[firstRow, i + 1])
                        {
                            if (firstRow == 12)
                            {
                                range.Value = ColumnName[i - 1];
                                workSheet.Row(firstRow).Height = 25;
                                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            }
                            if (i >= 3 && i <= 6)
                            {
                                range.Worksheet.Column(i).Width = 13;
                            }
                            if (firstRow > 12)
                            {
                                workSheet.Row(firstRow).Height = 20;
                            }
                            range.Style.Font.Size = 9f;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }

                    }

                    firstRow++;
                }

                for (int i = 3; i <= ColumnName.Count + 1; i++)
                {
                    using (var range = workSheet.Cells[firstRow + 1, i])
                    {
                        if (i == 3)
                        {
                            range.Value = "Total";
                        }
                        range.Style.Font.Size = 9f;
                        workSheet.Row(firstRow + 1).Height = 20;
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                        range.Style.Locked = false;
                    }
                }

                ExcelFile.Save();
                return SuccessMessage + '|' + path;
            }
            catch (Exception)
            {

                return FailMessage;
            }

        }
        public string CreatePurchaseHistoryByMakerByCommodityFileExcel()
        {
            try
            {

                string filename = "Purchase_History_By_Maker_By_Commodity_" + FileNameExtension + ".xlsx";
                string path = System.IO.Path.GetFullPath(PurchaseExcelFilePath + filename);
                ExcelPackage ExcelFile = new ExcelPackage(new FileInfo(path));

                var workSheet = ExcelFile.Workbook.Worksheets.Add("Sheet1");
                int columnData = 30;

                CreatePublicPurchaseExcelFile(workSheet, columnData, "Purchase History By Maker By Commodity");

                //workSheet.DefaultRowHeight = 12;
                workSheet.DefaultColWidth = 10;
                workSheet.Cells.Style.WrapText = true;
                workSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Protection.IsProtected = true;

                List<string> ColumnName = new List<string>() { "Year", "Maker", "Commodity", "Total Perchase Weight", "Total Purchase Amount", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };
                var Model = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }; // data

                int firstRow = 12; //begin from row 12
                foreach (int a in Model)
                {
                    for (int i = 1; i <= ColumnName.Count; i++)
                    {
                        using (var range = workSheet.Cells[firstRow, i + 1])
                        {
                            if (firstRow == 12)
                            {
                                range.Value = ColumnName[i - 1];
                                workSheet.Row(firstRow).Height = 25;
                                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            }
                            if (i >= 3 && i <= 6)
                            {
                                range.Worksheet.Column(i).Width = 13;
                            }
                            if (firstRow > 12)
                            {
                                workSheet.Row(firstRow).Height = 20;
                            }
                            range.Style.Font.Size = 9f;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }

                    }

                    firstRow++;
                }

                for (int n = 1; n <= 3; n++)
                {
                    for (int i = 3; i <= ColumnName.Count + 1; i++)
                    {
                        if (i == 4) continue;
                        using (var range = workSheet.Cells[firstRow + 1, i])
                        {
                            if (n == 1 && i == 3)
                            {
                                range.Value = "Commodity Total";
                            }
                            else if (n == 2 && i == 3)
                            {
                                range.Value = "Maker Total";
                            }
                            else if (n == 3 && i == 3)
                            {
                                range.Value = "Grand Total";
                            }
                            range.Style.Font.Size = 9f;
                            workSheet.Row(firstRow + 1).Height = 20;
                            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }
                    }
                    firstRow++;
                }


                ExcelFile.Save();
                return SuccessMessage + '|' + PurchaseExcelFilePath + filename;
            }
            catch (Exception)
            {

                return FailMessage;
            }

        }
        public string CreateHistoryByMakerByCommodityBySpecBySizeFileExcel()
        {

            try
            {
                string filename = "Purchase_History_By_Maker_By_Commodity_By_Spec_By_Size_" + FileNameExtension + ".xlsx";
                string path = System.IO.Path.GetFullPath(PurchaseExcelFilePath + filename);
                ExcelPackage ExcelFile = new ExcelPackage(new FileInfo(path));

                var workSheet = ExcelFile.Workbook.Worksheets.Add("Sheet1");
                int columnData = 32;

                CreatePublicPurchaseExcelFile(workSheet, columnData, "Purchase History By Maker By Commodity By Spec By Size");

                //workSheet.DefaultRowHeight = 12;
                workSheet.DefaultColWidth = 10;
                workSheet.Cells.Style.WrapText = true;
                workSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Protection.IsProtected = true;

                List<string> ColumnName = new List<string>() { "Year", "Maker", "Commodity", "Spec", "Size", "Total Perchase Weight", "Total Purchase Amount", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };
                var Model = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }; // data

                int firstRow = 12; //begin from row 12
                foreach (int a in Model)
                {
                    for (int i = 1; i <= ColumnName.Count; i++)
                    {
                        using (var range = workSheet.Cells[firstRow, i + 1])
                        {
                            if (firstRow == 12)
                            {
                                range.Value = ColumnName[i - 1];
                                workSheet.Row(firstRow).Height = 25;
                                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            }
                            if (i >= 3 && i <= 6)
                            {
                                range.Worksheet.Column(i).Width = 13;
                            }
                            if (firstRow > 12)
                            {
                                workSheet.Row(firstRow).Height = 20;
                            }
                            range.Style.Font.Size = 9f;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }

                    }

                    firstRow++;
                }

                for (int n = 1; n <= 3; n++)
                {
                    for (int i = 3; i <= ColumnName.Count + 1; i++)
                    {
                        if (i == 4) continue;
                        using (var range = workSheet.Cells[firstRow + 1, i])
                        {
                            if (n == 1 && i == 3)
                            {
                                range.Value = "Commodity Total";
                            }
                            else if (n == 2 && i == 3)
                            {
                                range.Value = "Maker Total";
                            }
                            else if (n == 3 && i == 3)
                            {
                                range.Value = "Grand Total";
                            }
                            range.Style.Font.Size = 9f;
                            workSheet.Row(firstRow + 1).Height = 20;
                            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }
                    }
                    firstRow++;
                }


                ExcelFile.Save();
                return SuccessMessage + '|' + PurchaseExcelFilePath + filename;
            }
            catch (Exception)
            {

                return FailMessage;
            }


        }
        public string CreateHistoryBySupplierFileExcel()
        {
            try
            {
                string filename = "Purchase_History_By_Supplier_" + FileNameExtension + ".xlsx";
                string path = System.IO.Path.GetFullPath(PurchaseExcelFilePath + filename);
                ExcelPackage ExcelFile = new ExcelPackage(new FileInfo(path));

                var workSheet = ExcelFile.Workbook.Worksheets.Add("Sheet1");
                int columnData = 29;

                CreatePublicPurchaseExcelFile(workSheet, columnData, "Purchase History By Supplier");

                //workSheet.DefaultRowHeight = 12;
                workSheet.DefaultColWidth = 10;
                workSheet.Cells.Style.WrapText = true;
                workSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Protection.IsProtected = true;

                List<string> ColumnName = new List<string>() { "Year", "Supplier", "Total Perchase Weight", "Total Purchase Amount", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };
                var Model = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }; // data

                int firstRow = 12; //begin from row 12
                foreach (int a in Model)
                {
                    for (int i = 1; i <= ColumnName.Count; i++)
                    {
                        using (var range = workSheet.Cells[firstRow, i + 1])
                        {
                            if (firstRow == 12)
                            {
                                range.Value = ColumnName[i - 1];
                                workSheet.Row(firstRow).Height = 25;
                                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            }
                            if (i >= 3 && i <= 6)
                            {
                                range.Worksheet.Column(i).Width = 13;
                            }
                            if (firstRow > 12)
                            {
                                workSheet.Row(firstRow).Height = 20;
                            }
                            range.Style.Font.Size = 9f;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }

                    }

                    firstRow++;
                }

                for (int i = 3; i <= ColumnName.Count + 1; i++)
                {
                    using (var range = workSheet.Cells[firstRow + 1, i])
                    {
                        if (i == 3)
                        {
                            range.Value = "Total";
                        }
                        range.Style.Font.Size = 9f;
                        workSheet.Row(firstRow + 1).Height = 20;
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                        range.Style.Locked = false;
                    }
                }

                ExcelFile.Save();
                return SuccessMessage + '|' + PurchaseExcelFilePath + filename;
            }
            catch (Exception)
            {

                return FailMessage;
            }

        }
        public string CreateHistoryBySupplierByCommodityFileExcel()
        {
            try
            {
                string filename = "Purchase_History_By_Supplier_By_Commodity_" + FileNameExtension + ".xlsx";
                string path = System.IO.Path.GetFullPath(PurchaseExcelFilePath + filename);
                ExcelPackage ExcelFile = new ExcelPackage(new FileInfo(path));

                var workSheet = ExcelFile.Workbook.Worksheets.Add("Sheet1");
                int columnData = 30;

                CreatePublicPurchaseExcelFile(workSheet, columnData, "Purchase History By Supplier By Commodity");

                //workSheet.DefaultRowHeight = 12;
                workSheet.DefaultColWidth = 10;
                workSheet.Cells.Style.WrapText = true;
                workSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Protection.IsProtected = true;

                List<string> ColumnName = new List<string>() { "Year", "Supplier", "Commodity", "Total Perchase Weight", "Total Purchase Amount", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };
                var Model = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }; // data

                int firstRow = 12; //begin from row 12
                foreach (int a in Model)
                {
                    for (int i = 1; i <= ColumnName.Count; i++)
                    {
                        using (var range = workSheet.Cells[firstRow, i + 1])
                        {
                            if (firstRow == 12)
                            {
                                range.Value = ColumnName[i - 1];
                                workSheet.Row(firstRow).Height = 25;
                                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            }
                            if (i >= 3 && i <= 6)
                            {
                                range.Worksheet.Column(i).Width = 13;
                            }
                            if (firstRow > 12)
                            {
                                workSheet.Row(firstRow).Height = 20;
                            }
                            range.Style.Font.Size = 9f;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }

                    }

                    firstRow++;
                }

                for (int n = 1; n <= 3; n++)
                {
                    for (int i = 3; i <= ColumnName.Count + 1; i++)
                    {
                        if (i == 4) continue;
                        using (var range = workSheet.Cells[firstRow + 1, i])
                        {
                            if (n == 1 && i == 3)
                            {
                                range.Value = "Commodity Total";
                            }
                            else if (n == 2 && i == 3)
                            {
                                range.Value = "Supplier Total";
                            }
                            else if (n == 3 && i == 3)
                            {
                                range.Value = "Grand Total";
                            }
                            range.Style.Font.Size = 9f;
                            workSheet.Row(firstRow + 1).Height = 20;
                            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }
                    }
                    firstRow++;
                }

                ExcelFile.Save();
                return SuccessMessage + '|' + PurchaseExcelFilePath + filename;
            }
            catch (Exception)
            {

                return FailMessage;
            }

        }
        public string CreateHistoryBySupplierByCommodityBySpecBySizeFileExcel()
        {
            try
            {
                string filename = "Purchase_History_By_Supplier_By_Commodity_By_Spec_By_Size_" + FileNameExtension + ".xlsx";
                string path = System.IO.Path.GetFullPath(PurchaseExcelFilePath + filename);
                ExcelPackage ExcelFile = new ExcelPackage(new FileInfo(path));

                var workSheet = ExcelFile.Workbook.Worksheets.Add("Sheet1");
                int columnData = 32;

                CreatePublicPurchaseExcelFile(workSheet, columnData, "Purchase History By Supplier By Commodity By Spec By Size");

                //workSheet.DefaultRowHeight = 12;
                workSheet.DefaultColWidth = 10;
                workSheet.Cells.Style.WrapText = true;
                workSheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Protection.IsProtected = true;

                List<string> ColumnName = new List<string>() { "Year", "Supplier", "Commodity", "Spec", "Size", "Total Perchase Weight", "Total Purchase Amount", "Jan (Wt)", "Jan (Amt)", "Feb (Wt)", "Feb (Amt)", "Mar (Wt)", "Mar (Amt)", "Apr (Wt)", "Apr (Amt)", "May (Wt)", "May (Amt)", "Jun (Wt)", "Jun (Amt)", "Jul (Wt)", "Jul (Amt)", "Aug (Wt)", "Aug (Amt)", "Sep (Wt)", "Sep (Amt)", "Oct (Wt)", "Oct (Amt)", "Nov (Wt)", "Nov (Amt)", "Dec (Wt)", "Dec (Amt)" };
                var Model = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }; // data

                int firstRow = 12; //begin from row 12
                foreach (int a in Model)
                {
                    for (int i = 1; i <= ColumnName.Count; i++)
                    {
                        using (var range = workSheet.Cells[firstRow, i + 1])
                        {
                            if (firstRow == 12)
                            {
                                range.Value = ColumnName[i - 1];
                                workSheet.Row(firstRow).Height = 25;
                                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            }
                            if (i >= 3 && i <= 6)
                            {
                                range.Worksheet.Column(i).Width = 13;
                            }
                            if (firstRow > 12)
                            {
                                workSheet.Row(firstRow).Height = 20;
                            }
                            range.Style.Font.Size = 9f;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }

                    }

                    firstRow++;
                }

                for (int n = 1; n <= 3; n++)
                {
                    for (int i = 3; i <= ColumnName.Count + 1; i++)
                    {
                        if (i == 4) continue;
                        using (var range = workSheet.Cells[firstRow + 1, i])
                        {
                            if (n == 1 && i == 3)
                            {
                                range.Value = "Commodity Total";
                            }
                            else if (n == 2 && i == 3)
                            {
                                range.Value = "Maker Total";
                            }
                            else if (n == 3 && i == 3)
                            {
                                range.Value = "Grand Total";
                            }
                            range.Style.Font.Size = 9f;
                            workSheet.Row(firstRow + 1).Height = 20;
                            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            range.Style.Locked = false;
                        }
                    }
                    firstRow++;
                }


                ExcelFile.Save();
                return SuccessMessage + '|' + PurchaseExcelFilePath + filename;
            }
            catch (Exception)
            {

                return FailMessage;
            }

        }
        #endregion                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      

        [HttpGet]
        public ActionResult CreatePDFFileForHisToryEnQuiRy(int name)
        {
            var message = String.Empty;
            CreateSpecialFolder("pdf");
            switch (name)
            {
                case (int)ConstantData.ReportFileName.Purchase_History_By_Maker:
                    message = CreatePurchaseHistoryEnquirybByMakerFile();
                    break;
                case (int)ConstantData.ReportFileName.Purchase_History_By_Maker_By_Commodity:
                    message = CreatePurchaseHistoryByMakerByCommodityFile();
                    break;
                case (int)ConstantData.ReportFileName.Purchase_History_By_Maker_By_Commodity_By_Spec_By_Size:
                    message = CreateHistoryByMakerByCommodityBySpecBySizeFile();
                    break;
                case (int)ConstantData.ReportFileName.Purchase_History_By_Supplier:
                    message = CreateHistoryBySupplierFile();
                    break;
                case (int)ConstantData.ReportFileName.Purchase_History_By_Supplier_By_Commodity:
                    message = CreateHistoryBySupplierByCommodityFile();
                    break;
                case (int)ConstantData.ReportFileName.Purchase_History_By_Supplier_By_Commodity_By_Spec_By_Size:
                    message = CreateHistoryBySupplierByCommodityBySpecBySizeFile();
                    break;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportExcelFileForHisToryEnQuiRy(int name)
        {
            var message = String.Empty;
            try
            {
                CreateSpecialFolder("excel");
                switch (name)
                {
                    case (int)ConstantData.ReportFileName.Purchase_History_By_Maker:
                        message = CreatePurchaseHistoryEnquirybByMakerFileExcel();
                        break;
                    case (int)ConstantData.ReportFileName.Purchase_History_By_Maker_By_Commodity:
                        message = CreatePurchaseHistoryByMakerByCommodityFileExcel();
                        break;
                    case (int)ConstantData.ReportFileName.Purchase_History_By_Maker_By_Commodity_By_Spec_By_Size:
                        message = CreateHistoryByMakerByCommodityBySpecBySizeFileExcel();
                        break;
                    case (int)ConstantData.ReportFileName.Purchase_History_By_Supplier:
                        message = CreateHistoryBySupplierFileExcel();
                        break;
                    case (int)ConstantData.ReportFileName.Purchase_History_By_Supplier_By_Commodity:
                        message = CreateHistoryBySupplierByCommodityFileExcel();
                        break;
                    case (int)ConstantData.ReportFileName.Purchase_History_By_Supplier_By_Commodity_By_Spec_By_Size:
                        message = CreateHistoryBySupplierByCommodityFileExcel();
                        break;
                }
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().ToString();
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        // end 
        // Purchase Order
        public ActionResult GetPrintPurchaseOrder()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SearchPurchaseOrder(jQueryDataTableParamModel param)  // search after print
        {
            IEnumerable<PUR001> Data = new List<PUR001>();
            int totalItem = 0;
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            var ma001_dal = new Areas.MasterSetting.DAL.MA001.MA001_DAL();
            var ma002_dal = new Areas.MasterSetting.DAL.MA002.MA002_DAL();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var array = param.sSearch.Split('|');
                string spurchaseFr = array[0], spurchaseTo = array[1];

                Data = dal.GetPurchaseOrderFromTo(spurchaseFr, spurchaseTo, takeFirst: false).Skip(param.iDisplayStart).Take(param.iDisplayLength);
                totalItem = dal.GetPurchaseOrderFromTo(spurchaseFr, spurchaseTo).Count();
            }

            var result = Data.Select(x => new
            {
                AAPURNO = x.AAPURNO,
                ABCTITM = x.ABCTITM,
                AACTRTP = x.AACTRTP,
                AACTRNM = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE003, x.AACTRTP),
                AASPLCD = x.AASPLCD,
                AASPLNM = ma001_dal.GetSalePurchase(x.AASPLCD) != null ? ma001_dal.GetSalePurchase(x.AASPLCD).MASPNM : string.Empty,
                AAUSRCD = x.AAUSRCD,
                AAUSRNM = ma002_dal.GetUserName(x.AAUSRCD),
                AACMDCD = x.AACMDCD,
                AACMDNM = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE006, x.AACMDCD),
                AAMKCD = x.AAMKCD,
                AAMKNM = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE005, x.AAMKCD),
                ABMCSPC = x.ABMCSPC,
                ABBSZT = x.ABBSZT,
                ABBSZW = x.ABBSZW,
                ABBSZL = x.ABBSZL,
                AASHPDT = x.AASHPDT,
                ABDLVDT = String.Format("{0:dd/MM/yyyy}", x.ABDLVDT),
                ABWT = x.ABWT,
                RAPSTLGR = x.RAPSTLGR,
                PURCODE = x.PURCODE // private key for item
            });
            return Json(new
            {
                iTotalRecords = totalItem,
                iTotalDisplayRecords = totalItem,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public enum PrintPurchaseOrderResult
        {
            Success = 1,
            Fail = 2,
            NoPrinter = 3
        }

        [HttpGet]
        public ActionResult PrintPurchaseOrder(string _array)
        {
            //try
            //{
            //    var printerConnect = Extension.Printer.PrinterMachine.DetectPrinterMachine(PrinterName);
            //    if (!printerConnect)
            //    {
            //        return Json(PrintPurchaseOrderResult.NoPrinter, JsonRequestBehavior.AllowGet);
            //    }
            //}
            //catch 
            //{
            //    throw;
            //}

            CreateSpecialFolder("pdf");
            var array = _array.Split('|');
            List<string> paths = new List<string>();
            string spurchaseFr = array[0], spurchaseTo = array[1];
            var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
            var datas = dal.GetPurchaseOrderFromTo(spurchaseFr, spurchaseTo);
            if (datas != null)
            {
                int n = 1;
                foreach (var data in datas)
                {
                    var rs = CreatePurchaseOrDerFile(data, n);
                    if (rs != "fail")
                    {
                        paths.Add(rs);
                        n++;
                    }
                }
            }


            if (paths != null)
            {
                return Json(paths.ToArray(), JsonRequestBehavior.AllowGet);
                //Uri uri;
                //foreach (var s in paths)
                //{
                //    uri = new Uri(s);
                //    Extension.Printer.PrinterMachine.PrintPDF(PrinterName, "A4", uri.LocalPath, 1);
                //}
            }
            else
            {
                return Json(PrintPurchaseOrderResult.Fail, JsonRequestBehavior.AllowGet);
            }

            // return Json(PrintPurchaseOrderResult.Success, JsonRequestBehavior.AllowGet);
        }

        public string CreatePurchaseOrDerFile(TopProSystem.Areas.MasterSetting.Models.PUR001 data, int n)
        {
            try
            {
                var dal = new Areas.MasterSetting.DAL.PUR001.PUR001_DAL();
                var allitem = dal.GetAllItemOfPurchase(data.AADSTNC, true); // khong lay reference

                double countofitem = allitem.Count;
                var pageNumber = Math.Ceiling(countofitem / 8.0);

                Document doc = new Document(PageSize.A4);
                string i_revisionNo;
                if (!string.IsNullOrEmpty(data.AAREVNO))
                {
                    i_revisionNo = data.AAREVNO;
                }
                else
                {
                    i_revisionNo = "00";
                }

                string filename = "PO_" + data.AAPURNO + "_" + i_revisionNo + "_" + n + ".pdf";
                string path = System.IO.Path.GetFullPath(PurchasePDFFilePath + filename);

                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
                doc.Open();

                decimal total = 0;
                int fsdata = 8;
                int fontSize = 10;
                int skip = 0;
                double tax = (double)data.AATXRT;
                int spacing_right = 70, spacing_top = 11;
                int itemSpacing_top = 60, start_pos_number = 540, left_start_pos = 45;
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, BaseFont.EMBEDDED);
                BaseFont VietFont = BaseFont.CreateFont(Server.MapPath("~/Font/RobotoCondensed-Regular.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                for (int i = 1; i <= pageNumber; i++)
                {
                    if (i > 1)
                    {
                        doc.NewPage();
                        start_pos_number = 540;
                        itemSpacing_top = 0;
                    }
                    // add template
                    PdfContentByte canvas = writer.DirectContentUnder;
                    string url_template = Server.MapPath("~/Images/Templates/Purchase_Order.png");
                    var template_background = Image.GetInstance(url_template);
                    template_background.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                    template_background.SetAbsolutePosition(doc.PageSize.Left, doc.PageSize.Bottom);
                    canvas.AddImage(template_background);
                    //end

                    CreateBeginText(writer, baseFont, 10, doc.PageSize.Right - 70, doc.PageSize.Top - 25, data.AAPURNO);
                    CreateBeginText(writer, baseFont, 10, doc.PageSize.Right - 70, doc.PageSize.Top - 37, string.Format("{0:dd/MM/yyyy}", data.AARGSDT));
                    CreateBeginText(writer, baseFont, 10, doc.PageSize.Right - 70, doc.PageSize.Top - 50, i_revisionNo);
                    //CreateBeginText(writer, baseFont, 10, doc.PageSize.Right - 139, doc.PageSize.Top - 60, "Page: " + i);

                    var Ma001_dal = new Areas.MasterSetting.DAL.MA001.MA001_DAL();
                    var Ma012_dal = new Areas.MasterSetting.DAL.MA012.MA012_DAL();


                    var SalePurchaseMasterModel = Ma001_dal.GetSalePurchase(data.AASPLCD);
                    if (SalePurchaseMasterModel != null)
                    {
                        CreateBeginText(writer, VietFont, fontSize, 22, 660 - spacing_top, SalePurchaseMasterModel.MASPNM.ToUpper());
                        CreateBeginText(writer, VietFont, fontSize, 22, 660 - spacing_top - spacing_top, SalePurchaseMasterModel.MASPAD1);
                        CreateBeginText(writer, VietFont, fontSize, 22, 660 - spacing_top - spacing_top - spacing_top, SalePurchaseMasterModel.MASPAD2);
                        CreateBeginText(writer, VietFont, fontSize, 22, 660 - spacing_top - spacing_top - spacing_top - spacing_top, SalePurchaseMasterModel.MASPAD3);
                    }

                    CreateBeginText(writer, baseFont, fontSize, 452, 560, "(" + data.AACRRCD.Trim() + ")");
                    CreateBeginText(writer, baseFont, fontSize, 540, 560, "(" + data.AACRRCD.Trim() + ")");

                    if (i == 1)
                    {
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos, start_pos_number, "Order No:");
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos + spacing_right, start_pos_number, data.ABORDNO);
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos, start_pos_number - spacing_top, "Maker:");
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos + spacing_right, start_pos_number - spacing_top, Ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE005, data.AAMKCD));
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos, start_pos_number - spacing_top - spacing_top, "Commodity:");
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos + spacing_right, start_pos_number - spacing_top - spacing_top, Ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE006, data.AACMDCD));
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos, start_pos_number - spacing_top - spacing_top - spacing_top, "Payment Term:");
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos + spacing_right, start_pos_number - spacing_top - spacing_top - spacing_top, Ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE020, data.AASETRM).TrimEnd() + " - " + Ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE021, data.AAPTTRM).Trim());
                        /*descript tion*/
                        CreateBeginText(writer, baseFont, fsdata, 350, start_pos_number, Ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE019, data.AAPRICE));
                        // CreateBeginText(writer, baseFont, fsdata, 460, start_pos_number, "Per MT");
                    }

                    foreach (var model in allitem.Skip(skip).Take(8))
                    {
                        CreateBeginText(writer, baseFont, fsdata, 18, start_pos_number - itemSpacing_top, model.ABCTITM.ToString());
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos, start_pos_number - itemSpacing_top, model.ABMCSPC);
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos + spacing_right, start_pos_number - itemSpacing_top, model.RAPSTLGR);
                        if (!string.IsNullOrEmpty(model.ABPRDDIA))
                        {
                            CreateBeginText(writer, baseFont, fsdata, left_start_pos, start_pos_number - itemSpacing_top - spacing_top, model.ABPRDDIA + " mm");
                        }
                        CreateBeginText(writer, baseFont, fsdata, left_start_pos + spacing_right, start_pos_number - itemSpacing_top - spacing_top, Ma012_dal.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE015, model.ABGRADE));
                        if (model.ABWT != null)
                        {
                            CreateBeginText(writer, baseFont, fsdata, 375, start_pos_number - itemSpacing_top, MoneyFormat(model.ABWT.ToString()), align: true, alignment: Element.ALIGN_RIGHT);
                        }
                        if (model.ABPRUP != null)
                        {
                            CreateBeginText(writer, baseFont, fsdata, 460, start_pos_number - itemSpacing_top, MoneyFormat(model.ABPRUP.ToString()), align: true, alignment: Element.ALIGN_RIGHT);
                        }
                        if (model.ABPRAT != null)
                        {
                            CreateBeginText(writer, baseFont, fsdata, doc.PageSize.Right - 20, start_pos_number - itemSpacing_top, MoneyFormat(model.ABPRAT.ToString()), align: true, alignment: Element.ALIGN_RIGHT);
                            total += (decimal)model.ABPRAT;
                        }

                        start_pos_number -= 30;
                    }

                    if (i == pageNumber)
                    {

                        CreateBeginText(writer, baseFont, fsdata, doc.PageSize.Right - 20, 217, MoneyFormat(total.ToString()), align: true, alignment: Element.ALIGN_RIGHT);

                        CreateBeginText(writer, baseFont, fsdata, 464, 208, "(" + tax + "%)");

                        decimal addtax = (total * (decimal)tax) / 100;
                        CreateBeginText(writer, baseFont, fsdata, doc.PageSize.Right - 20, 208, MoneyFormat(addtax.ToString()), align: true, alignment: Element.ALIGN_RIGHT);

                        decimal grandtotal = total + addtax;
                        CreateBeginText(writer, baseFont, fsdata, doc.PageSize.Right - 20, 198, MoneyFormat(grandtotal.ToString()), align: true, alignment: Element.ALIGN_RIGHT);

                    }

                    CreateBeginText(writer, baseFont, fsdata, 22, 170, data.AARMK1);
                    CreateBeginText(writer, baseFont, fsdata, 22, 170 - spacing_top, data.AARMK2);
                    CreateBeginText(writer, baseFont, fsdata, 22, 170 - spacing_top - spacing_top, data.AARMK3);
                    CreateBeginText(writer, baseFont, fsdata, 22, 170 - spacing_top - spacing_top - spacing_top, data.AARMK4);
                    CreateBeginText(writer, baseFont, fsdata, 22, 170 - spacing_top - spacing_top - spacing_top - spacing_top, data.AARMK5);
                    /*description*/
                    skip += 8;
                } //end for

                doc.Close();
                return path;
            }
            catch (Exception ex)
            {
                return "fail";
            }

        }

        private string MoneyFormat(string number)
        {
            return string.Format("{0:#,0}", Convert.ToDecimal(number));
        }

        public void CreateBeginText(PdfWriter writer, BaseFont baseFont, int fontSize, float x, float y, string text, bool align = false, int alignment = Element.ALIGN_LEFT)
        {
            if (!string.IsNullOrEmpty(text))
            {
                writer.DirectContent.BeginText();
                writer.DirectContent.MoveText(x, y);
                writer.DirectContent.SetFontAndSize(baseFont, fontSize);
                if (align == false)
                {
                    writer.DirectContent.ShowText(text);
                }
                else
                {
                    writer.DirectContent.ShowTextAligned(alignment, text, x, y, 0);
                }
                writer.DirectContent.EndText();
            }          
        }

        #region download
        public ActionResult DownloadPdfFile(string path)
        {
            Uri uri = new Uri(path);
            return File(path, "application/pdf", System.IO.Path.GetFileName(uri.LocalPath));
        }
        public ActionResult DownloadExcelFile(string path)
        {
            Uri uri = new Uri(path);
            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", System.IO.Path.GetFileName(uri.LocalPath));
        }

        public ActionResult DownloadMultiFile(string filePaths)
        {
            var _filePaths = filePaths.Split(',');
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (var file in _filePaths)
                {
                    zip.AddFile(file, @"\");
                }

                MemoryStream output = new MemoryStream();
                zip.Save(output);
                return File(output.ToArray(), "application/zip", "PO_" + FileNameExtension + ".zip");
            }

        }
        #endregion
    }
}