using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopProSystem.Extension.AccountRole;
using TopProSystem.Filters;

namespace TopProSystem.Controllers
{
    [CustomAuthorize]
    public class InventoryController : Controller
    {
        public ViewResult InventoryEnquiry()
        {
            Areas.MasterSetting.DAL.INV001.INV001_DAL iNV001_DAL = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            return View(iNV001_DAL.GetReferencesForEnquiry(new Areas.MasterSetting.Models.INV001()));
        }

        public List<Areas.MasterSetting.Models.INV001> TempOfListResult(List<Areas.MasterSetting.Models.INV001> data = null)
        {
            if (data != null)
            {
                Session["DataExportExcel"] = data;
            }
            return Session["DataExportExcel"] as List<Areas.MasterSetting.Models.INV001>;
        }

        [HttpPost]
        public JsonResult ajaxHandler(Areas.MasterSetting.Models.jQueryDataTableParamModel param)
        {

            string[] keys = { "CAINVNO", "CASPEC", "CACOAT", "CABSZT", "CABSZW", "CABSZL", "CAPRDDIA", "CASTLGR", "CAINVTP", "CAMKCD", "CASPPNO", "CASPLCD", "CAUSRCD", "CACTRNO", "CACMDCD", "CACTRTP", "CAINVST", "CACSTCD", "CAJOBNO", "CAPCKNO", "CALCTCD", "CAOENO", "CAOEITM", "CAISPNO" };
            var array = !string.IsNullOrEmpty(param.sSearch) ? param.sSearch.Split('|') : new string[keys.Length];
            var dictionary = new Dictionary<string, string>();
            if (!array.All(x => x == null) && !array.All(x => x == ""))
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    dictionary.Add(keys[i], array[i]);
                }
            }

            Areas.MasterSetting.DAL.MA012.MA012_DAL mA012_DAL = new Areas.MasterSetting.DAL.MA012.MA012_DAL();
            Areas.MasterSetting.DAL.INV001.INV001_DAL iNV001_dal = new Areas.MasterSetting.DAL.INV001.INV001_DAL();
            var data = iNV001_dal.GetTotalDisplayInventoryQuery(dictionary);
            TempOfListResult(data.ToList());
            int count = data.Count();
            var rs = data.Skip(param.iDisplayStart).Take(param.iDisplayLength).Select(x => new
            {
                CACTRTP = x.CACTRTP,
                CAINVST = mA012_DAL.GetSRNameBySRCode(Areas.MasterSetting.Models.ClassificationCode.CLASSIFICATTIONCODE027,x.CAINVST),
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
                CAISPNO = x.CAISPNO
            });
            return Json(new
            {
                iTotalRecords = count,
                iTotalDisplayRecords = count,
                aaData = rs,
                totalQty = Math.Round(Convert.ToDecimal(data.Sum(x=>x.CAQTY)),2),
                totalWeight = Math.Round(Convert.ToDecimal(data.Sum(x=>x.CAWT)),2)
            }, JsonRequestBehavior.AllowGet);
        }

        public enum ExcelExportResult : int
        {
            Success = 0,
            Fail = 1,
            NoData = 2
        }
        public JsonResult CreateExcelFileEnquiry()
        {
            var data = TempOfListResult();
            if (!data.Any())
            {
                return Json(new { status = ExcelExportResult.NoData }, JsonRequestBehavior.AllowGet);
            }
            try
            {

                string SaveFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["SaveReportRawMaterialPdfURL"];
                System.IO.Directory.CreateDirectory(SaveFilePath);
                string filename = "Inventoy_Inquiry_" + System.Web.HttpContext.Current.Server.MachineName + "_"+ Session[Models.ConstantData.SessionUserID]+ "_" +  string.Format("{0:ddMMyyyyhhmmss}", DateTime.Now) + ".xlsx";
                string path = System.IO.Path.GetFullPath(System.IO.Path.Combine(SaveFilePath, filename));
                ExcelPackage ExcelFile = new ExcelPackage(new System.IO.FileInfo(path));
                var workSheet = ExcelFile.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.Style.Font.SetFromFont(new System.Drawing.Font("Calibri Light", 11));
                const int ROW = 5, COLUMN = 2;
                string[] header = {"#", "Inventory Status", "Inventory No", "Inspection No", "Spec", "Thick", "Width", "Length", "Product Name", "Product Diameter", "Steal Grade", "Quantity", "Weight" };

                var titleAddress = "B1:N4";
                using (var range = workSheet.Cells[titleAddress])
                {
                    range.Merge = true;
                    range.Value = "INVENTORY ENQUIRY RESULT LIST";
                    range.Style.Font.Size = 22;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                //fill data
                DataTable dataTable = new DataTable();
                Areas.MasterSetting.DAL.MA012.MA012_DAL mA012_DAL = new Areas.MasterSetting.DAL.MA012.MA012_DAL();

                foreach (var title in header)
                {
                    dataTable.Columns.Add(new DataColumn(title, typeof(string)));
                }
                dataTable.Rows.Add(header);
                int stt = 1;
                foreach (var obj in data)
                {
                    dataTable.Rows.Add(stt,mA012_DAL.GetSRNameBySRCode(Areas.MasterSetting.Models.ClassificationCode.CLASSIFICATTIONCODE027, obj.CAINVST), obj.CAINVNO, obj.CAISPNO, obj.CASPEC, obj.CABSZT, obj.CABSZW, obj.CABSZL, obj.CAPRDNM, obj.CAPRDDIA, obj.CASTLGR, obj.CAQTY, obj.CAWT);
                    stt++;
                }
                // load data to excel from datatable
                workSheet.Cells[ROW, COLUMN].LoadFromDataTable(dataTable, false);
                //total
                for(int i = 3; i >= 1; i--)
                {
                    using (var range = workSheet.Cells[ROW + dataTable.Rows.Count, header.Length + COLUMN - i])
                    {
                        switch (i)
                        {
                            case 3:
                                range.Value = "Total";
                                break;
                            case 2:
                                range.Value = data.Sum(x => x.CAQTY);
                                break;
                            case 1:
                                range.Style.Numberformat.Format = "0.00";
                                range.Value = data.Sum(x => x.CAWT);
                                break;
                        }
                        range.AutoFitColumns();
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(0, 221, 235, 247);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.Font.Bold = true;
                    }
                }
                
                //format style
                var subColumn = COLUMN;
                for (int i = subColumn; i < header.Length + COLUMN; i++)
                {
                    using (var range = workSheet.Cells[ROW, i])
                    {
                        range.AutoFitColumns();
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(0, 221, 235, 247);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.Font.Bold = true;
                    }
                }

                for (int i = ROW + 1; i <= data.Count + ROW; i++)
                {
                    subColumn = COLUMN;
                    for (int j = subColumn; j < header.Length + COLUMN; j++)
                    {
                        using (var range = workSheet.Cells[i, j])
                        {
                            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }
                }

                ExcelFile.Save();
                return Json(new { status = ExcelExportResult.Success, path = path }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { status = ExcelExportResult.Fail, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadExcelFile(string path)
        {
            Uri uri = new Uri(path);
            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", System.IO.Path.GetFileName(uri.LocalPath));
        }
    }
}