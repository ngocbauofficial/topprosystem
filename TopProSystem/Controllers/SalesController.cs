using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopProSystem.Extension.AccountRole;

namespace TopProSystem.Controllers
{
    public class SalesController : BaseSalesController
    {
        private TopProSystem.Areas.MasterSetting.Models.TopProSystemEntities MasterdataEntities = new Areas.MasterSetting.Models.TopProSystemEntities();

        public ViewResult SaleOrderEntry()
        {
            return View();
        }
        public ViewResult SaleOrderEntry_Add()
        {
            this.CreateSelectListItem();
            return View();
        }

        public ViewResult SaleOrderEntry_Change()
        {
            this.CreateSelectListItem();
            return View();
        }

        public PartialViewResult AddItem()
        {
            CreateSelectListItem();
            return PartialView("_SaleOrderEntry");
        }

        public ViewResult SaleOrderEntry_Delete()
        {
            this.CreateSelectListItem();
            return View();
        }

        public ViewResult SaleOrderStockAllocation()
        {
            return View();
        }
        public ViewResult SaleOrderStockCancellation()
        {
            return View();
        }

        public ViewResult SaleOrderComPletion()
        {
            return View();
        }
        public void CreateSelectListItem()
        {
            ViewBag.UserCode = MasterdataEntities.MA002.Select(x => new { x.MBUSRCD, x.MBUSRNM }).ToList();
            ViewBag.Section= MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("008")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.CommodityCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("006")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.CurrencyCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("012")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.TaxCode = MasterdataEntities.MA010.OrderByDescending(x => x.MKRGSDT).Select(x => new { x.MKTXCD, x.MKTXRT }).ToList();
            ViewBag.ContractType = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("025")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.Delivery= MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("024")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.ExchangeRate = MasterdataEntities.MA009.OrderByDescending(x => x.MJRGSDT).Select(x => new { x.MJEXRT, x.MJTXEXR, x.MJCRRCD }).ToList();
            ViewBag.ExchangeRateTax = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("018")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.Grade = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("015")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.Spec = MasterdataEntities.MA006.OrderByDescending(x => x.MFRGSDT).Select(x => new { x.MFPRDSP }).ToList();
            ViewBag.UnitPrice = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("030")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            ViewBag.Coating = MasterdataEntities.MA005.OrderByDescending(x => x.MERGSDT).Select(x => new { x.MECOAT });



            //ViewBag.Grade = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("015")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.MakerCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("005")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.CurrencyCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("012")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.PriceTerm = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("019")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.TypeOfTerm = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("021")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.CommodityCode = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("006")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.ContractType = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("025")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.SettelementTerm = MasterdataEntities.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals("020")).Select(x => new { x.MNSRCD, x.MNSRNM }).ToList();
            //ViewBag.TaxCode = MasterdataEntities.MA010.OrderByDescending(x => x.MKRGSDT).Select(x => new { x.MKTXCD, x.MKTXRT }).ToList();
            //ViewBag.ExchangeRate = MasterdataEntities.MA009.OrderByDescending(x => x.MJRGSDT).Select(x => new { x.MJEXRT, x.MJTXEXR, x.MJCRRCD }).ToList();
            //ViewBag.Spec = MasterdataEntities.MA006.OrderByDescending(x => x.MFRGSDT).Select(x => new { x.MFPRDSP }).ToList();
            //ViewBag.Coating = MasterdataEntities.MA005.OrderByDescending(x => x.MERGSDT).Select(x => new { x.MECOAT });
            //ViewBag.UserCode = MasterdataEntities.MA002.Select(x => new { x.MBUSRCD, x.MBUSRNM }).ToList();

        }
    }
}