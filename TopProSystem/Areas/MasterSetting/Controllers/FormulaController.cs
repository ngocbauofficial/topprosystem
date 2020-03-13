using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.Controllers
{
    public class FormulaController : Controller
    {
        // // ABPRUPD = ABPRUP * EXRT

        [HttpPost]
        public JsonResult ABPRUPD_Multiply_Result(string ABPRUP, string EXRT)
        {

            if (!string.IsNullOrEmpty(ABPRUP) && !string.IsNullOrEmpty(EXRT))
            {
                double a = double.Parse(ABPRUP, CultureInfo.GetCultureInfo("en-GB")), b = double.Parse(EXRT);
                Int64 result = Rouding(a * b);
                return Json(formatvnd(result), JsonRequestBehavior.DenyGet);
            }
            return Json(-1, JsonRequestBehavior.DenyGet);
        }

        //ABPRATD = ABPRUPD * ABWT
        [HttpPost]
        public JsonResult ABPRATD_Multiply_Result(string ABPRUPD, string ABWT)
        {
            if (!string.IsNullOrEmpty(ABPRUPD) && !string.IsNullOrEmpty(ABWT))
            {
                double a = double.Parse(ABPRUPD),
                 b = double.Parse(ABWT);
                Int64 result = Rouding(a * b);

                return Json(formatvnd(result), JsonRequestBehavior.DenyGet);
            }
            return Json(-1, JsonRequestBehavior.DenyGet);
        }
        //ABPRXATD = AATXRT/100 * ABPRATD
        [HttpPost]
        public JsonResult ABPTXAD_Multiply_Result(string AATXRT, string ABPRATD)
        {
            if (!string.IsNullOrEmpty(AATXRT) && !string.IsNullOrEmpty(ABPRATD))
            {
                double a = double.Parse(AATXRT), b = double.Parse(ABPRATD);
                Int64 result = Rouding(a / 100 * b);

                return Json(formatvnd(result), JsonRequestBehavior.DenyGet);
            }
            return Json(-1, JsonRequestBehavior.DenyGet);
        }
        //ABPRAT = ABPRUP * ABWT
        [HttpPost]
        public JsonResult ABPRAT_Multiply_Result(string ABPRUP, string ABWT)
        {
            if (!string.IsNullOrEmpty(ABPRUP) && !string.IsNullOrEmpty(ABWT))
            {
                double a = double.Parse(ABPRUP), b = double.Parse(ABWT);
                Int64 result = Rouding(a * b);

                return Json(formatvnd(result), JsonRequestBehavior.DenyGet);
            }
            return Json(-1, JsonRequestBehavior.DenyGet);
        }
        //ABPTXAT = AATXRT/100 * ABPRAT
        [HttpPost]
        public JsonResult ABPTXAT_Multiply_Result(string AATXRT, string ABPRAT)
        {
            if (!string.IsNullOrEmpty(AATXRT) && !string.IsNullOrEmpty(ABPRAT))
            {
                double a = double.Parse(AATXRT), b = double.Parse(ABPRAT);
                Int64 result = Rouding(a / 100 * b);

                return Json(formatvnd(result), JsonRequestBehavior.DenyGet);
            }
            return Json(-1, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ABQTY_Multiply_Result(string ABWT)
        {
            if (!string.IsNullOrEmpty(ABWT))
            {
                double a = Convert.ToDouble(ABWT);
                double number = 2.2;
                Int64 result = Rouding(a / number);

                return Json(result, JsonRequestBehavior.DenyGet);
            }
            return Json(-1, JsonRequestBehavior.DenyGet);
        }


        public Int64 Rouding(double number)
        {
            return Convert.ToInt64(number);
        }

        public string formatvnd(Int64 number)
        {
            var a = String.Format("{0:#,0.00}", number);
            return a;
        }
    }
}