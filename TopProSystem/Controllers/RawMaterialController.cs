using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Diagnostics;
using TopProSystem.Extension.AccountRole;
using TopProSystem.Areas.MasterSetting.DAL.PUR001;
using TopProSystem.Areas.MasterSetting.Models;
using OfficeOpenXml;
using System.Transactions;
using TopProSystem.Areas.MasterSetting.DAL.RawMaterialDal;

namespace TopProSystem.Controllers
{
    public class RawMaterialController : BaseRawMaterialController
    {
        private string RawMaterialPDFFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["SaveReportRawMaterialPdfURL"];
        string MachineName = " ";//@"\\phuongGai\Canon LBP2900";
        private Areas.MasterSetting.Models.TopProSystemEntities MasterdataEntities = new Areas.MasterSetting.Models.TopProSystemEntities();
        public void CreateSpecialFolder()
        {
            string[] arrayRootUrl = RawMaterialPDFFilePath.Split('/');
            var RootFolder = arrayRootUrl[0] + "/";
            for (int i = 1; i < arrayRootUrl.Length; i++)
            {
                var FolderName = arrayRootUrl[i];
                if (!Directory.Exists(RootFolder + FolderName))
                {
                    Directory.CreateDirectory(RootFolder + FolderName);
                }
                RootFolder += FolderName + "/";
            }
        }
        public ViewResult GetRawMaterialWarehousingEntry()
        {
            return View();
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
        public ActionResult GetRegisterTariffAmount()
        {
            return View();
        }
        public ActionResult RegisterCustomHandlingAmount()
        {
            return View();
        }
        public ActionResult GetRegisterFreightAmount()
        {
            return View();
        }
        #region
        public ActionResult AjaxHandlerRawMaterialWarehousingResult_Tmptb(Areas.MasterSetting.Models.jQueryDataTableParamModel param)
        {
            int number = 0;
            var displayItem = new List<Areas.MasterSetting.Models.Tmp_MaterialWarehousingResult>();
            number = MasterdataEntities.Tmp_MaterialWarehousingResult.Count();
            displayItem = MasterdataEntities.Tmp_MaterialWarehousingResult.OrderByDescending(x => x.ID).ToList();
            var result = displayItem.Select(c => new
            {
                ID = c.ID,
                Date = String.Format("{0:dd/MM/yyyy}", c.Date),
                Location_code = c.Location_Code,
                Inventory = c.Inventory_No,
                Inspection = c.Inspection_No
            }).ToList();
            return Json(new
            {
                iTotalRecords = number,
                iTotalDisplayRecords = number,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRawMaterialWarehousingResult()
        {
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
        public ActionResult InsertTmp_RawMaterialWarehousingResult(Areas.MasterSetting.Models.Tmp_MaterialWarehousingResult tmp)
        {
            if (tmp != null)
            {
                MasterdataEntities.Tmp_MaterialWarehousingResult.Add(tmp);
                if (MasterdataEntities.SaveChanges() > 0)
                {
                    return Json(Models.ConstantData.SuccessMessage, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Models.ConstantData.FailMessage, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(Models.ConstantData.FailMessage, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete_RawMaterialWarehousingResult(string _array)
        {
            var array = _array.Split(',');
            Areas.MasterSetting.Models.Tmp_MaterialWarehousingResult obj = null;
            foreach (var id in array)
            {
                obj = MasterdataEntities.Tmp_MaterialWarehousingResult.Find(int.Parse(id));
                if (obj != null)
                {
                    MasterdataEntities.Tmp_MaterialWarehousingResult.Remove(obj);
                    if (MasterdataEntities.SaveChanges() <= 0)
                    {
                        return Json(Models.ConstantData.FailMessage, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    continue;
                }
            }
            return Json(Models.ConstantData.SuccessMessage, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetPrintRawMaterialRecevingList()
        {
            return View();
        }
        #region Print Raw Material Label
        public ActionResult GetPrintRawMaterialLabel()
        {
            return View();
        }
        public void PrintStamp(string path)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                Verb = "PrintTo",
                FileName = path,
                Arguments = "\"" + MachineName + "\"",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            p.Start();
        }
        public static PdfPCell FormatCell(int border = 0, Paragraph work = null, int alignMent = PdfPCell.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(work);
            cell.Border = border;
            cell.HorizontalAlignment = alignMent;
            return cell;
        }
        public static PdfPCell FormatCell(int border = 0, Image img = null, int alignMent = PdfPCell.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(img);
            cell.Border = border;
            cell.HorizontalAlignment = alignMent;
            return cell;
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                message = "Printer not found";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region import
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
        #endregion
    }
}