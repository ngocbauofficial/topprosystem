using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TopProSystem.Areas.MasterSetting.Models;
using TopProSystem.Filters;
using StaticResources.Controller;
using TopProSystem.Areas.MasterSetting.ForeignKeyConstraint;
using TopProSystem.Extension.AccountRole;
using System.Resources;
using System.Configuration;
using System.Data.SqlClient;

namespace TopProSystem.Areas.MasterSetting.Controllers
{

    public class MasterController : BaseMasterController
    {
        //private static TopProSystemEntities MasterDataContext = new TopProSystemEntities();
        PermissionService _permissionService = new PermissionService();
        #region Sale Purchase
        public ActionResult SalePurchaseMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SalePurchaseMaster))
                return AccessDeniedView();


            return View();
        }



        public ActionResult AjaxhanlderSalePurchase(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SalePurchaseMaster))
                return AccessDeniedView();
            DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();
            int number = 0;
            var displayItem = new List<MA001>();
            if (!String.IsNullOrWhiteSpace(param.sSearch))
            {
                number = mA001_DAL.GetTotalRecord(param.sSearch);
                displayItem = mA001_DAL.GetTotalDisplayRecord(param.sSearch, param.iDisplayStart, param.iDisplayLength);
            }
            else
            {
                number = mA001_DAL.GetTotalRecord(String.Empty);
                displayItem = mA001_DAL.GetTotalDisplayRecord(String.Empty, param.iDisplayStart, param.iDisplayLength);
            }
            var result = displayItem.Select(c => new
            {
                MASPCD = c.MASPCD,
                MASPNM = c.MASPNM,
                MASPSNM = c.MASPSNM,
                MASPAD1 = c.MASPAD1,
                MASPAD2 = c.MASPAD2,
                MASPAD3 = c.MASPAD3,
                MACNTRC = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE010, c.MACNTRC),
                MASPTEL = c.MASPTEL,
                MASPFAX = c.MASPFAX,
                MABUZCD = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE011, c.MABUZCD),
                MAIDCD = c.MAIDCD,
                MASPCTG = c.MASPCTG,
                MARGSDT = String.Format("{0:dd/MM/yyyy}", c.MARGSDT),
                MARGSTM = c.MARGSTM,
                MAUPDT = String.Format("{0:dd/MM/yyyy}", c.MAUPDT),
                MAUPDTM = c.MAUPDTM,
                MASTDUE = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE001, c.MASTDUE),
                MASCLSD = c.MASCLSD,
                MASDFER = c.MASDFER,
                MASSETL = c.MASSETL,
                MACLTRM = c.MACLTRM,
                MASDAYS = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE002, c.MASDAYS),
                MASCRCD = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE012, c.MASCRCD),
                MASTXCD = c.MASTXCD,
                MASCALC = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE003, c.MASCALC),
                MAPTDUE = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE001, c.MAPTDUE),
                MAPCLSD = c.MAPCLSD,
                MAPDFER = c.MAPDFER,
                MAPSETEL = c.MAPSETEL,
                MAPYTRM = c.MAPYTRM,
                MAPDAYS = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE002, c.MAPDAYS),
                MAPCRCD = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE012, c.MAPCRCD),
                MAPTXCD = c.MAPTXCD,
                MAPCALC = MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE003, c.MAPCALC)

            }).ToList();
            return Json(new
            {
                iTotalRecords = number,
                iTotalDisplayRecords = number,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckSPCodeExists(string spcode)
        {
            DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();
            return Json(mA001_DAL.CheckSpCodeExists(spcode), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSalePurchase(string arrayspcode)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.SalePurchaseMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();

            DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();
            string[] array = null;

            if (!String.IsNullOrEmpty(arrayspcode))
            {
                array = arrayspcode.Split('|');
            }

            if (array != null)
            {
                foreach (var spCode in array)
                {
                    if (!mA001_DAL.Delete(spCode))
                    {
                        return Json("/MasterSetting/ErrorMessage/Notification?actionName=deletesalepurchase", JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateSalePurchase(string spcode)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.SalePurchaseMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();
            return View(mA001_DAL.GetListForientkey(spcode));
        }

        [HttpPost]
        public ActionResult UpdateSalePurchase(MA001 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SalePurchaseMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();
            if (mA001_DAL.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("SalePurchaseMaster");
            }
            else
            {
                return ReturnErrorMessage("Update sale purchase");
            }
        }

        public ActionResult GetInsertSalePurchase()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SalePurchaseMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();
            return View(mA001_DAL.GetListForientkey(null));
        }

        [HttpPost]
        public ActionResult InsertSalePurchase(MA001 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SalePurchaseMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();
            DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();

            if (mA001_DAL.Insert(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("SalePurchaseMaster");
            }
            else
            {
                return ReturnErrorMessage("Insert sale purchase");
            }
        }

        public JsonResult GetJsonSalePurchase(string spcode)
        {
            DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();
            return Json(mA001_DAL.GetSalePurchase(spcode), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckSalePurchaseMasterInUsedReference(string code)
        {
            var checkused = Constraint.CheckConstraintMA001(code);
            if (checkused != null)
                return Json(new { message = checkused }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region User Master
        public ActionResult UserMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserMaster))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult AjaxhanlderUser(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserMaster))
                return AccessDeniedView();

            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();

            int number = 0;
            var displayItem = new List<MA002>();
            if (!String.IsNullOrWhiteSpace(param.sSearch))
            {
                number = mA002_DAL.GetTotalRecord(param.sSearch);
                displayItem = mA002_DAL.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, param.sSearch);
            }
            else
            {
                number = mA002_DAL.GetTotalRecord(String.Empty);
                displayItem = mA002_DAL.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, String.Empty);
            }
            var result = displayItem.Select(c => new
            {
                MBUSRCD = c.MBUSRCD,
                MBUSRNM = c.MBUSRNM,
                MBUSSNM = c.MBUSSNM,
                MBUSAD1 = c.MBUSAD1,
                MBUSAD2 = c.MBUSAD2,
                MBUSAD3 = c.MBUSAD3,
                MBUSTEL = c.MBUSTEL,
                MBWTCAL = c.MBWTCAL,
                MBCOMCD = c.MBCOMCD,
                MBUNICD = c.MBUNICD,
                MBRGSDT = String.Format("{0:dd/MM/yyyy}", c.MBRGSDT),
                MBRGSTM = c.MBRGSTM,
                MBUPDT = String.Format("{0:dd/MM/yyyy}", c.MBUPDT),
                MBUPDTM = c.MBUPDTM,
            }).ToList();
            return Json(new
            {
                iTotalRecords = number,
                iTotalDisplayRecords = number,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckUserNameExists(string username)
        {
            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
            return Json(mA002_DAL.CheckUserNameExists(username), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckUserIDExists(string userid)
        {
            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
            return Json(mA002_DAL.CheckUserIDExists(userid), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteUserMaster(string arrayuserid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();

            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
            string[] array = null;

            if (!String.IsNullOrEmpty(arrayuserid))
            {
                array = arrayuserid.Split('|');
            }

            if (array != null)
            {
                foreach (var userid in array)
                {
                    if (!mA002_DAL.Delete(userid))
                    {
                        return Json("/MasterSetting/ErrorMessage/Notification?actionName=deleteusermaster", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("/MasterSetting/ErrorMessage/Notification?actionName=deleteusermaster", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateUserMaster(string userid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
            var model = mA002_DAL.GetMA002(userid);
            model = mA002_DAL.GetReference(model);
            return View(model);
        }

        public ActionResult UpdateUserMaster(MA002 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
            if (mA002_DAL.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("UserMaster");
            }
            else
            {
                return ReturnErrorMessage("Update usermaster");
            }
        }

        public ActionResult GetInsertUserMaster()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.UserMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();
            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
            var model = new MA002();
            model = mA002_DAL.GetReference(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult InsertUserMaster(MA002 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
            if (mA002_DAL.Insert(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("UserMaster");
            }
            else
            {
                return ReturnErrorMessage("insert usermaster");
            }
        }

        public ActionResult GetUserName(string userid)
        {
            DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
            return Json(new { username = mA002_DAL.GetUserName(userid) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckUserCodeInUsedReference(string userid)
        {
            var checkused = Constraint.CheckConstraintMA002(userid);
            if (checkused != null)
                return Json(new { message = checkused }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region UserID Master
        public ActionResult UserIDMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserIdMaster))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult AjaxhanlderUserIDMaster(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserIdMaster))
                return AccessDeniedView();

            DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();
            int number = 0;
            var displayItem = new List<MA003>();
            if (!String.IsNullOrWhiteSpace(param.sSearch))
            {
                number = mA003_DAL.GetTotalRecord(param.sSearch);
                displayItem = mA003_DAL.GetTotalDisplayRecord(param.sSearch, param.iDisplayStart, param.iDisplayLength);
            }
            else
            {
                number = mA003_DAL.GetTotalRecord(String.Empty);
                displayItem = mA003_DAL.GetTotalDisplayRecord(String.Empty, param.iDisplayStart, param.iDisplayLength);
            }
            var result = displayItem.Select(c => new
            {
                MCIDCD = c.MCIDCD,
                MCIDNM = c.MCIDNM,
                MCSCTLV = c.MCSCTLV,
                MCSCTNC = GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE008, c.MCSCTNC),
                MCRGSDT = String.Format("{0:dd/MM/yyyy}", c.MCRGSDT),
                MCRGSTM = c.MCRGSTM,
                MCUPDT = String.Format("{0:dd/MM/yyyy}", c.MCUPDT),
                MCUPDTM = c.MCUPDTM,
            }).ToList();
            return Json(new
            {
                iTotalRecords = number,
                iTotalDisplayRecords = number,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckIdCodeExits(string idCode)
        {
            DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();
            return Json(mA003_DAL.CheckIdCodeExits(idCode), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteUserID(string arrayuserid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserIdMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();
            DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();

            string[] array = null;

            if (!String.IsNullOrEmpty(arrayuserid))
            {
                array = arrayuserid.Split('|');
            }

            if (array != null)
            {
                foreach (var userid in array)
                {

                    if (!mA003_DAL.Delete(userid))
                    {
                        return Json("/MasterSetting/ErrorMessage/Notification?actionName=deleteuseridmaster", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("/MasterSetting/ErrorMessage/Notification?actionName=deleteuseridmaster", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateUserIDMaster(string userid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserIdMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();
            var model = mA003_DAL.GetMA003(userid);
            var _model = mA003_DAL.GetListForient();
            model.SectionCode = _model.SectionCode;
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateUserID(MA003 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserIdMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();
            if (mA003_DAL.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("UserIDMaster");
            }
            else
            {
                return ReturnErrorMessage("update userid");
            }
        }

        public ActionResult GetInsertUserID()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserIdMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();
            DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();
            return View(mA003_DAL.GetListForient());
        }

        [HttpPost]
        public ActionResult InsertUserIDMaster(MA003 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UserIdMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();
            if (mA003_DAL.Insert(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("UserIDMaster");
            }
            else
            {
                return ReturnErrorMessage("Insert userid master");
            }

        }

        public JsonResult GetJsonMa003(string userid)
        {
            DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();
            return Json(mA003_DAL.GetMA003(userid), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckUserIDCodeInUsedReference(string userid)
        {
            var checkused = Constraint.CheckConstraintMA003(userid);
            if (checkused != null)
                return Json(new { message = checkused }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Location Master
        public ActionResult LocationMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LocationMaster))
                return AccessDeniedView();
            return View();
        }

        public ActionResult AjaxHandlerLocationMaster(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LocationMaster))
                return AccessDeniedView();
            DAL.MA004.MA004_DAL mA004_DAL = new DAL.MA004.MA004_DAL();
            int model = 0;
            var displayItem = new List<MA004>();
            if (!String.IsNullOrWhiteSpace(param.sSearch))
            {
                model = mA004_DAL.GetTotalRecord(param.sSearch);
                displayItem = mA004_DAL.GetTotalDisplayRecord(param.sSearch, param.iDisplayStart, param.iDisplayLength);
            }
            else
            {
                model = mA004_DAL.GetTotalRecord(String.Empty);
                displayItem = mA004_DAL.GetTotalDisplayRecord(String.Empty, param.iDisplayStart, param.iDisplayLength);
            }

            var result = displayItem.Select(c => new
            {
                MDLCTCD = c.MDLCTCD,
                MDWRCTG = GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE035, c.MDWRCTG),
                MDLCTNM = c.MDLCTNM,
                MDRGSDT = String.Format("{0:dd/MM/yyyy}", c.MDRGSDT),
                MDRGSTM = c.MDRGSTM,
                MDUPDT = String.Format("{0:dd/MM/yyyy}", c.MDUPDT),
                MDUPDTM = c.MDUPDTM

            }).ToList();
            return Json(new
            {
                iTotalRecords = model,
                iTotalDisplayRecords = model,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CheckLocationCodeExists(string locationCode)
        {
            DAL.MA004.MA004_DAL mA004_DAL = new DAL.MA004.MA004_DAL();
            return Json(mA004_DAL.CheckLocationCodeExists(locationCode), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteLocationMaster(string arraylocationCode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LocationMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();


            DAL.MA004.MA004_DAL mA004_DAL = new DAL.MA004.MA004_DAL();
            string[] array = null;
            if (!String.IsNullOrEmpty(arraylocationCode))
            {
                array = arraylocationCode.Split('|');
            }

            if (array != null)
            {
                foreach (var locationCode in array)
                {
                    if (!mA004_DAL.Delete(locationCode))
                    {
                        return Json("/MasterSetting/ErrorMessage/Notification?actionName=deletelocationmaster", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("/MasterSetting/ErrorMessage/Notification?actionName=deletelocationmaster", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateLocationMaster(string locationcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LocationMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA004.MA004_DAL mA004_DAL = new DAL.MA004.MA004_DAL();
            var model = mA004_DAL.GetMA004(locationcode);


            model = mA004_DAL.GetReference(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateLocationMaster(MA004 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LocationMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA004.MA004_DAL mA004_DAL = new DAL.MA004.MA004_DAL();
            if (mA004_DAL.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("LocationMaster");
            }
            else
            {
                return ReturnErrorMessage("Update location master");
            }
        }

        public ActionResult GetInsertLocationMaster() //004
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LocationMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA004.MA004_DAL mA004_DAL = new DAL.MA004.MA004_DAL();
            var model = new MA004();
            model = mA004_DAL.GetReference(model);
            return View(model);

        }
        [HttpPost]
        public ActionResult InsertLocationMaster(MA004 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LocationMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA004.MA004_DAL mA004_DAL = new DAL.MA004.MA004_DAL();
            if (mA004_DAL.Insert(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("LocationMaster");
            }
            else
            {
                return ReturnErrorMessage("insert location");
            }
        }

        #endregion

        #region Coating Master
        public ActionResult CoatingMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CoatingMaster))
                return AccessDeniedView();
            return View();
        }

        public ActionResult AjaxHandlerCoatingMaster(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CoatingMaster))
                return AccessDeniedView();

            DAL.MA005.MA005_DAL _ma005dal = new DAL.MA005.MA005_DAL();
            var countRecord = _ma005dal.GetTotalRecord(param.sSearch);
            var displayItem = _ma005dal.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, param.sSearch);

            var result = displayItem.Select(c => new
            {
                MECOAT = c.MECOAT,
                MECOATW = c.MECOATW,
                MERGSDT = String.Format("{0:dd/MM/yyyy}", c.MERGSDT),
                MERGSTM = c.MERGSTM,
                MEUPDT = String.Format("{0:dd/MM/yyyy}", c.MEUPDT),
                MEUPDTM = c.MEUPDTM

            }).ToList();
            return Json(new
            {
                iTotalRecords = countRecord,
                iTotalDisplayRecords = countRecord,
                aaData = result
            }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult CheckCoatingExists(string coatingCode)
        {
            DAL.MA005.MA005_DAL _ma005dal = new DAL.MA005.MA005_DAL();
            return Json(_ma005dal.CheckCoatingExists(coatingCode), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCoatingMaster(string arraycoatingCode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CoatingMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();

            DAL.MA005.MA005_DAL _ma005dal = new DAL.MA005.MA005_DAL();
            string[] array = null;
            if (!String.IsNullOrEmpty(arraycoatingCode))
            {
                array = arraycoatingCode.Split('|');
            }
            if (array != null)
            {
                foreach (var coatingCode in array)
                {
                    if (!_ma005dal.Delete(coatingCode))
                        return Json("/MasterSetting/ErrorMessage/Notification?actionName=deletecoating", JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json("/MasterSetting/ErrorMessage/Notification?actionName=deletecoating", JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetUpdateCoatingMaster(string meCoat)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CoatingMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA005.MA005_DAL _ma005dal = new DAL.MA005.MA005_DAL();
            return View(_ma005dal.GetMA005(meCoat));
        }

        [HttpPost]
        public ActionResult UpdateCoatingMaster(MA005 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CoatingMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA005.MA005_DAL _ma005dal = new DAL.MA005.MA005_DAL();
            if (_ma005dal.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("CoatingMaster");
            }
            return ReturnErrorMessage("update coating master");
        }

        public ActionResult GetInsertCoating()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CoatingMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult InsertCoatingMaster(MA005 coating)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CoatingMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA005.MA005_DAL _ma005dal = new DAL.MA005.MA005_DAL();
            if (_ma005dal.Insert(coating))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("CoatingMaster");
            }
            return RedirectToAction("CoatingMaster");

        }
        public JsonResult CheckCoatingInUsedReference(string coating)
        {
            var checkused = Constraint.CheckConstraintMA005(coating);
            if (checkused != null)
                return Json(new { message = checkused }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Spec Master
        public ActionResult SpecMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SpecMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult AjaxHandlerSpecMaster(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SpecMaster))
                return AccessDeniedView();

            DAL.MA006.MA006_DAL _ma006dal = new DAL.MA006.MA006_DAL();

            var countRecord = _ma006dal.GetTotalRecord(param.sSearch);
            var displayItem = _ma006dal.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, param.sSearch);

            var result = displayItem.Select(c => new
            {
                MFPRDSP = c.MFPRDSP,
                MFPNSTY = String.Format("{0:0.000000}", c.MFPNSTY),
                MFRMK10 = c.MFRMK10,
                MFRGSDT = String.Format("{0:dd/MM/yyyy}", c.MFRGSDT),
                MFRGSTM = c.MFRGSTM,
                MFUPDT = String.Format("{0:dd/MM/yyyy}", c.MFUPDT),
                MFUPDTM = c.MFUPDTM,
                CMOCD = c.CMOCD

            }).ToList();
            return Json(new
            {
                iTotalRecords = countRecord,
                iTotalDisplayRecords = countRecord,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CheckProductSpecInUse(string proSpec)
        {
            DAL.MA006.MA006_DAL _ma006dal = new DAL.MA006.MA006_DAL();

            return Json(_ma006dal.CheckProductSpecInUse(proSpec), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSpecMaster(string _array)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SpecMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();
            DAL.MA006.MA006_DAL _ma006dal = new DAL.MA006.MA006_DAL();
            string[] array = null;
            if (!String.IsNullOrEmpty(_array))
            {
                array = _array.Split('|');
            }
            if (array != null)
            {
                foreach (var proSpec in array)
                {

                    if (!_ma006dal.Delete(proSpec))
                        return Json("/MasterSetting/ErrorMessage/Notification?actionName=delete spec master", JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json("/MasterSetting/ErrorMessage/Notification?actionName=delete spec master", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateSpecMaster(string proSpec)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SpecMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA006.MA006_DAL _ma006dal = new DAL.MA006.MA006_DAL();
            var model = _ma006dal.GetMA006(proSpec);
            model = _ma006dal.GetListForient(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateSpecMaster(MA006 spec)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SpecMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA006.MA006_DAL _ma006dal = new DAL.MA006.MA006_DAL();
            if (_ma006dal.Update(spec))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("SpecMaster");
            }
            return ReturnErrorMessage("update spec master");
        }

        public ActionResult GetInsertSpec()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SpecMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();
            DAL.MA006.MA006_DAL _ma006dal = new DAL.MA006.MA006_DAL();
            var model = new MA006();
            model = _ma006dal.GetListForient(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult InsertSpecMaster(MA006 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SpecMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA006.MA006_DAL _ma006dal = new DAL.MA006.MA006_DAL();
            if (_ma006dal.Insert(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("SpecMaster");
            }
            return ReturnErrorMessage("Insert Spec");

        }

        public JsonResult CheckSpecInUsedReference(string spec)
        {
            var checkused = Constraint.CheckConstraintMA006(spec);
            if (checkused != null)
                return Json(new { message = checkused }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Closing Date
        public ActionResult ClosingDateMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CoatingMaster))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public JsonResult AjaxHandlerMA007(jQueryDataTableParamModel param)
        {
            var dal = new DAL.MA007.MA007_DAL();
            var totalRecord = dal.GetTotalRecord(param.sSearch);
            var totalDisplay = dal.GetTotalDisplayRecord(param.sSearch, param.iDisplayLength, param.iDisplayStart);
            var result = totalDisplay.Select(x => new
            {
                id = x.ID,
                MHBKMNT = x.MHBKMNT,
                MHCLSDT = string.Format("{0:dd/MM/yyyy}", x.MHCLSDT),
                MHRGSDT = string.Format("{0:dd/MM/yyyy}", x.MHRGSDT),
                MHRGSTM = x.MHRGSTM,
                MHUPDT = string.Format("{0:dd/MM/yyyy}", x.MHUPDT),
                MHUPDTM = x.MHUPDTM
            });
            return Json(new
            {
                iTotalRecords = totalRecord,
                iTotalDisplayRecords = totalRecord,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public ViewResult GetInsertClosingDate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InsertClosingDate(MA007 model)
        {
            if (model != null)
            {
                var dal = new DAL.MA007.MA007_DAL();
                if (dal.Insert(model))
                {
                    TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                    return RedirectToAction("ClosingDateMaster");
                }
                return ReturnErrorMessage("insert");
            }
            else
            {
                return ReturnErrorMessage("insert");
            }

        }

        public ViewResult GetUpdateClosingDate(string id)
        {
            var dal = new DAL.MA007.MA007_DAL();
            var model = dal.GetClosingDate(int.Parse(id));
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateClosingDate(Models.MA007 model)
        {
            var dal = new DAL.MA007.MA007_DAL();
            if (dal.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageSendView.MessageNotifi("update");
                return RedirectToAction("ClosingDateMaster");
            }
            else
            {
                return ReturnErrorMessage("update closingdate");
            }
        }
        public JsonResult DeleteClosingDate(string array)
        {
            string[] _array = null;
            if (!string.IsNullOrEmpty(array))
            {
                _array = array.Split('|');
            }

            if (_array != null)
            {
                var dal = new DAL.MA007.MA007_DAL();
                if (dal.Delete(_array))
                {
                    return Json(true,JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckBookingMonthExists(string bookingMonth)
        {
            var dal = new DAL.MA007.MA007_DAL();
            return Json(dal.CheckBookingMonthExists(bookingMonth), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Exchange Rate
        public ActionResult ExchangeRateMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster))
                return AccessDeniedView();
            return View();
        }

        public ActionResult AjaxHandlerExchangeRateMaster(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster))
                return AccessDeniedView();

            DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();
            var countRecord = _ma009dal.GetTotalRecord(param.sSearch);
            var displayItem = _ma009dal.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, param.sSearch);
            var result = displayItem.Select(c => new
            {
                ID = c.ID,
                MJCRRCD = c.MJCRRCD,
                MJEXRTT = GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE018, c.MJEXRTT),
                MJEXRTD = c.MJEXRTD,
                MJEXRT = c.MJEXRT,
                MJTXEXR = c.MJTXEXR,
                MJUPDT = String.Format("{0:dd/MM/yyyy}", c.MJUPDT),
                MJUPDTM = c.MJUPDTM,
                MJRGSDT = String.Format("{0:dd/MM/yyyy}", c.MJRGSDT),
                MJRGSTM = c.MJRGSTM

            }).ToList();
            return Json(new
            {
                iTotalRecords = countRecord,
                iTotalDisplayRecords = countRecord,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CheckCurrencyCodeInUsed(string currencyCode, double? date)
        {
            if (!string.IsNullOrEmpty(currencyCode) && date!=0)
            {
                DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();
                return Json(_ma009dal.CheckCurrencyCodeInUsed(currencyCode, date), JsonRequestBehavior.AllowGet);
            }
          
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckCurrencyCodeInUsedEdit(string currencyCode, double? date, int Id)
        {
            DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();
            return Json(_ma009dal.CheckCurrencyCodeInUsed(currencyCode, date, Id), JsonRequestBehavior.AllowGet);
        }



        public ActionResult DeleteExchangeRate(string _array)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();


            DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();

            string[] array = null;

            if (!String.IsNullOrEmpty(_array))
            {
                array = _array.Split('|');
            }

            if (array != null)
            {
                foreach (var id in array)
                {
                    if (!_ma009dal.Delete(int.Parse(id)))
                        return Json("/MasterSetting/ErrorMessage/Notification?actionName=deleteexchangerate", JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json("/MasterSetting/ErrorMessage/Notification?actionName=deleteexchangerate", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateExchangeRate(int curCode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();
            var _model = _ma009dal.GetMA009(curCode);
            _model.ExchangerateTypes = _ma009dal.GetReference(_model).ExchangerateTypes;
            return View(_model);
        }

        [HttpPost]
        public ActionResult UpdateExchangeRate(MA009 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();
            if (_ma009dal.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("ExchangeRateMaster");
            }
            return ReturnErrorMessage("update exchange rate");
        }

        public ActionResult GetInsertExchangeRate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            var model = new Models.MA009();
            DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();
            return View(_ma009dal.GetReference(model));
        }

        [HttpPost]
        public ActionResult InsertExchangeRate(MA009 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();
            if (_ma009dal.Insert(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("ExchangeRateMaster");
            }
            return ReturnErrorMessage("Insert exchange rate");
        }

        public JsonResult GetExchangeRate(string curcode, string exchangerateCode, string date)
        {
            var _day = DateTime.Parse(date);
            double _ddate = double.Parse(string.Concat(_day.Month, _day.Year));
            DAL.MA009.MA009_DAL _ma009dal = new DAL.MA009.MA009_DAL();
            return Json(new { data = _ma009dal.GetExchangeRate(curcode, exchangerateCode, _ddate) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckExchangeRateInUsedReference(int code)
        {
            var checkused = Constraint.CheckConstraintMA009(code);
            if (checkused != null)
                return Json(new { message = checkused }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Tax Master
        public ActionResult TaxMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TaxMaster))
                return AccessDeniedView();
            return View();
        }

        public ActionResult AjaxHandlerTaxMaster(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TaxMaster))
                return AccessDeniedView();

            DAL.MA010.MA010_DAL _ma010dal = new DAL.MA010.MA010_DAL();
            int countRecord = _ma010dal.GetTotalRecord(param.sSearch);
            var displayItem = _ma010dal.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, param.sSearch);
            var result = displayItem.Select(c => new
            {
                MKTXCD = c.MKTXCD,
                MKTXDL = c.MKTXDL,
                MKTXRT = c.MKTXRT,
                MKFRDT = String.Format("{0:dd/MM/yyyy}", c.MKFRDT),
                MKRGSDT = String.Format("{0:dd/MM/yyyy}", c.MKRGSDT),
                MKRGSTM = c.MKRGSTM,
                MKUPDT = String.Format("{0:dd/MM/yyyy}", c.MKUPDT),
                MKUPDTM = c.MKUPDTM,

            }).ToList();
            return Json(new
            {
                iTotalRecords = countRecord,
                iTotalDisplayRecords = countRecord,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckTaxCodeExists(string taxCode)
        {
            DAL.MA010.MA010_DAL _ma010dal = new DAL.MA010.MA010_DAL();
            return Json(_ma010dal.CheckTaxCodeExists(taxCode), JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetInsertTax()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TaxMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult InsertTaxMaster(MA010 tax)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TaxMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA010.MA010_DAL _ma010dal = new DAL.MA010.MA010_DAL();
            if (_ma010dal.Insert(tax))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("TaxMaster");
            }
            return ReturnErrorMessage("insert tax master");
        }


        [HttpPost]
        public ActionResult DeleteTax(string arraytaxCode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TaxMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();

            DAL.MA010.MA010_DAL _ma010dal = new DAL.MA010.MA010_DAL();
            var array = arraytaxCode.Split('|');

            foreach (var taxCode in array)
            {
                if (!_ma010dal.Delete(taxCode))
                {
                    return Json("/MasterSetting/Errormessage/Notification?actionName=Delete", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateTax(string taxCode)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.TaxMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA010.MA010_DAL _ma010dal = new DAL.MA010.MA010_DAL();
            return View(_ma010dal.GetMA010ByMKTXCD(taxCode));
        }

        [HttpPost]
        public ActionResult UpdateTax(MA010 tax)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TaxMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA010.MA010_DAL _ma010dal = new DAL.MA010.MA010_DAL();
            if (_ma010dal.Update(tax))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("TaxMaster");
            }

            return ReturnErrorMessage("update tax master");

        }

        public JsonResult GetTax(string taxcode)
        {
            DAL.MA010.MA010_DAL _ma010dal = new DAL.MA010.MA010_DAL();
            return Json(_ma010dal.GetMA010ByMKTXCD(taxcode), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckTaxCodeInused(string taxCode)
        {
            var checkused = Constraint.CheckConstraintMA010(taxCode);
            if (checkused != null)
                return Json(new { message = checkused }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetJsonTaxCode(string codes)
        {
            DAL.MA010.MA010_DAL mA010_DAL = new DAL.MA010.MA010_DAL();
            var model = mA010_DAL.GetMA010ByMKTXCD(codes);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Credit master

        public ActionResult CreditMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CreditMaster))
                return AccessDeniedView();
            return View();
        }
        public ActionResult AjaxHandlerCreditMaster(jQueryDataTableParamModel param)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CreditMaster))
                return AccessDeniedView();
            DAL.MA011.MA011_DAL _ma011dal = new DAL.MA011.MA011_DAL();
            var countitem = _ma011dal.GetTotalRecord(param.sSearch);
            var displayItem = _ma011dal.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, param.sSearch);

            var result = displayItem.Select(c => new
            {
                MLCSTCD = c.MLCSTCD,
                MLCRDRK = c.MLCRDRK,
                MLVLDTR = c.MLVLDTR,
                MLCRDLM = c.MLCRDLM,
                MLRGSDT = String.Format("{0:dd/MM/yyyy}", c.MLRGSDT),
                MLRGSTM = c.MLRGSTM,
                MLUPDT = String.Format("{0:dd/MM/yyyy}", c.MLUPDT),
                MLUPDTM = c.MLUPDTM

            }).ToList();
            return Json(new
            {
                iTotalRecords = countitem,
                iTotalDisplayRecords = countitem,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CheckCustomerCodeExists(string cusCode)
        {
            DAL.MA011.MA011_DAL _ma011dal = new DAL.MA011.MA011_DAL();
            return Json(_ma011dal.CheckCustomerCodeExists(cusCode), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateCredit(string cusCode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CreditMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA011.MA011_DAL _ma011dal = new DAL.MA011.MA011_DAL();
            var model = _ma011dal.GetMa011byMLCSTCD(cusCode);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateCredit(MA011 model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CreditMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA011.MA011_DAL _ma011dal = new DAL.MA011.MA011_DAL();
            if (_ma011dal.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("CreditMaster");
            }
            return ReturnErrorMessage("Updatecredit(writelog)");
        }

        public ActionResult DeleteCredit(string arraycusCode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CreditMaster, StandardPermissionProvider.Delete))
                return AccessDeniedView();

            DAL.MA011.MA011_DAL _ma011dal = new DAL.MA011.MA011_DAL();
            var array = arraycusCode.Split('|');
            foreach (var cusCode in array)
            {
                bool delete = _ma011dal.Delete(cusCode);
                if (delete == false)
                {
                    return Json("/MasterSetting/Errormessage/Notification?actionName=deletecredit", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInsertCreditMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CreditMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA011.MA011_DAL _ma011dal = new DAL.MA011.MA011_DAL();
            var model = new MA011();
            model = _ma011dal.GetListForient(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult InsertCredit(MA011 model)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.CreditMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();
            DAL.MA011.MA011_DAL _ma011dal = new DAL.MA011.MA011_DAL();
            if (_ma011dal.Insert(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("CreditMaster");
            }
            return ReturnErrorMessage("Insertcredit(writelog)");
        }

        #endregion

        #region Due date type, Days in Month, Calculation Type, Weigth Display Code, Maker Code

        #region view index
        public ActionResult DueDateTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DueDateTypeMaster))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult AjaxhanlderMA012(jQueryDataTableParamModel param, string MNCLSD)
        {
            if (!_permissionService.AuthorizeMA012(MNCLSD))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var countmodel = _ma012dal.GetTotalRecord(param.sSearch, MNCLSD);
            var displayItem = _ma012dal.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, param.sSearch, MNCLSD);
            var result = displayItem.Select(c => new
            {
                MNCLSD = c.MNCLSCD,
                MNSRCD = c.MNSRCD,
                MNSRNM = c.MNSRNM,
                MNSRSNM = c.MNSRSNM,
                MNRGSDT = String.Format("{0:dd/MM/yyyy}", c.MNRGSDT),
                MNRGSTM = c.MNRGSTM,
                MNUPDT = String.Format("{0:dd/MM/yyyy}", c.MNUPDT),
                MNUPTM = c.MNUPTM,
                disabled = Constraint.CheckMA012Constraint(c.MNCLSCD, c.MNSRCD)
            }).ToList();
            return Json(new
            {
                iTotalRecords = countmodel,
                iTotalDisplayRecords = countmodel,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DaysInMonthMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DaysInMonthMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult CalculationTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CalculationTypeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult WeightDisplayCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WeightDisplayCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult MakerCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MakerCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult CommodityCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CommodityCodeMaster))
                return AccessDeniedView();


            return View();
        }
        public ActionResult StatusCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.StatusCodeMaster))
                return AccessDeniedView();

            return View();
        }
        public ActionResult SectionCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SectionCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult DamageCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DamageCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult CountryCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CountryCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult BusinessTypeCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BusinessTypeCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult CurrencyCodeMaster()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.CurrencyCodeMaster))
                return AccessDeniedView();
            return View();
        }

        public ActionResult ShiftCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ShiftCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult MachineNoMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MachineNoMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GradeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.GradeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult ReasonForChangingMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ReasonForChangingMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult BankCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BankCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult ExchangeRateTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateTypeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult PriceTermMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.PriceTermMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult SettlementTermMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SettlementTermMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult TypeofTermsMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TypeofTermsMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult PackingTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.PackingTypeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult InterruptedReasonCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.InterruptedReasonCodeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult DeliveryConditionMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DeliveryConditionMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult ContractTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ContractTypeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult TradeCategoryMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TradeCategoryMaster))
                return AccessDeniedView();

            return View();
        }
        public ActionResult InventoryStatusMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.InventoryStatusMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult InventoryTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.InventoryTypeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult MoneyTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MoneyTypeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult UnitPriceUnitMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UnitPriceUnitMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult LogTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LogTypeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult DataTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DataTypeMaster))
                return AccessDeniedView();

            return View();
        }

        public ActionResult WeightCalculationCodeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WeightCalculationCodeMaster))
                return AccessDeniedView();
            return View();
        }
        public ActionResult RawMaterialTypeMaster()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.RawMaterialTypeMaster))
                return AccessDeniedView();
            return View();

        }
        #endregion

        public ActionResult CheckSRCodeExists(string arraycode)
        {
            DAL.MA012.MA012_DAL mA012_DAL = new DAL.MA012.MA012_DAL();
            var array = arraycode.Split('|');
            string classificode = array[0], srcode = array[1];

            return Json(mA012_DAL.CheckSRCodeExists(srcode, classificode), JsonRequestBehavior.AllowGet);
        }


        // Lay srname show tren view salepurchase va nhung cho khac
        public static string GetSRNameBySRCode(string classiCode, string srCode)
        {
            DAL.MA012.MA012_DAL mA012_DAL = new DAL.MA012.MA012_DAL();
            return mA012_DAL.GetSRNameBySRCode(classiCode, srCode);
        }

        public ActionResult GetJsonBySRCode(string codes)
        {
            var array = codes.Split('|');
            string classiCode = array[0], srCode = array[1];
            DAL.MA012.MA012_DAL mA012_DAL = new DAL.MA012.MA012_DAL();

            var model = mA012_DAL.GetMa012BySrcode(srCode, classiCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WarehouseCategoryCodeMaster() //035master
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WarehouseCategoryCodeMaster))
                return AccessDeniedView();
            return View();

        }
        public ActionResult RawMaterialLabelTypeMaster() //036master
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.RawMaterialLabelTypeMaster))
                return AccessDeniedView();
            return View();
        }

        #region insert
        public ActionResult GetInsertDuedateType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DueDateTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertDaysInMonth()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DaysInMonthMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertCalculationType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CalculationTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertWeightDisplayCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WeightDisplayCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertMakerCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MakerCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertCommodityCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CommodityCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertStatusCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.StatusCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertSectionCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SectionCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertDamageCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DamageCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertCountryCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CountryCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInserDamageCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DamageCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertBusinessTypeCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BusinessTypeCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertCurrencyCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CurrencyCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertShiftCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ShiftCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertMachineNo()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MachineNoMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertGrade()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.GradeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertReasonForChangingCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ReasonForChangingMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertBankCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BankCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertExchangeRateType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertPriceTerm()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.PriceTermMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();


            return View();
        }

        public ActionResult GetInsertSettlementTerm()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SettlementTermMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertTypeofTerms()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TypeofTermsMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertPackingType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.PackingTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertInterruptedReasonCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.InterruptedReasonCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertDeliveryCondition()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DeliveryConditionMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertContractType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ContractTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertTradeCategory()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TradeCategoryMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }

        public ActionResult GetInsertInventoryStatus()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.InventoryStatusMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertInventoryType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.InventoryTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertMoneyType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MoneyTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertUnitPriceUnit()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UnitPriceUnitMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertLogType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LogTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertDataType()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DataTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();

            return View();
        }
        public ActionResult GetInsertWeightCalculationCode()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WeightCalculationCodeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();
            return View();
        }

        public ActionResult GetInsertRawMaterialType()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.RawMaterialTypeMaster, StandardPermissionProvider.Add))
                return AccessDeniedView();
            return View();
        }
        public ActionResult GetInsertWarehouseCategoryCode() //035insert
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WarehouseCategoryCodeMaster))
                return AccessDeniedView();
            return View();
        }
        public ActionResult GetInsertRawMaterialLabelType() //036insert
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.RawMaterialLabelTypeMaster))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult Insert_Due_Maker_Commodity_Days_Cal_Weigth(MA012 model)
        {

            if (!_permissionService.AuthorizeMA012(model.MNCLSCD.Trim(), StandardPermissionProvider.Add))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            bool insert = _ma012dal.Insert(model);
            if (insert == true)
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction(GetRedirectAction(model.MNCLSCD.Trim()));
            }
            return ReturnErrorMessage(GetRedirectAction(model.MNCLSCD.Trim()));
        }

        #endregion

        #region update
        public ActionResult GetUpdateDueDateType(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DueDateTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE001);
            return View(model);
        }

        public ActionResult GetUpdateDaysInMonth(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DaysInMonthMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE002);
            return View(model);
        }

        public ActionResult GetUpdateCalculationTypeMaster(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CalculationTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE003);
            return View(model);
        }

        public ActionResult GetUpdateWeightDisplayCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WeightDisplayCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE004);
            return View(model);
        }

        public ActionResult GetUpdateMakerCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MakerCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE005);
            return View(model);
        }

        public ActionResult GetUpdateCommodityCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CommodityCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE006);
            return View(model);
        }
        public ActionResult GetUpdateStatusCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.StatusCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE007);
            return View(model);
        }
        public ActionResult GetUpdateSectionCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SectionCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE008);
            return View(model);
        }
        public ActionResult GetUpdateDamageCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DamageCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE009);
            return View(model);
        }
        public ActionResult GetUpdateCountryCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CountryCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE010);
            return View(model);
        }
        public ActionResult GetUpdateBusinessTypeCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BusinessTypeCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE011);
            return View(model);
        }
        public ActionResult GetUpdateCurrencyCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.CurrencyCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE012);
            return View(model);
        }
        public ActionResult GetUpdateShiftCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ShiftCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE013);
            return View(model);
        }
        public ActionResult GetUpdateMachineNo(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MachineNoMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();


            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE014);
            return View(model);
        }
        public ActionResult GetUpdateGrade(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.GradeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE015);
            return View(model);
        }
        public ActionResult GetUpdateReasonForChangingCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ReasonForChangingMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE016);
            return View(model);
        }

        public ActionResult GetUpdateBankCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BankCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE017);
            return View(model);
        }

        public ActionResult GetUpdateExchangeRateType(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ExchangeRateMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE018);
            return View(model);
        }

        public ActionResult GetUpdatePriceTerm(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.PriceTermMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE019);
            return View(model);
        }

        public ActionResult GetUpdateSettlementTerm(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SettlementTermMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE020);
            return View(model);
        }

        public ActionResult GetUpdateTypeofTerms(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TypeofTermsMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE021);
            return View(model);
        }
        public ActionResult GetUpdatePackingType(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.PackingTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE022);
            return View(model);
        }

        public ActionResult GetUpdateInterruptedReasonCode(string srcode)
        {


            if (!_permissionService.Authorize(StandardPermissionProvider.InterruptedReasonCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE023);
            return View(model);
        }

        public ActionResult GetUpdateDeliveryCondition(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DeliveryConditionMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE024);
            return View(model);
        }

        public ActionResult GetUpdateContractType(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ContractTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE025);
            return View(model);
        }

        public ActionResult GetUpdateTradeCategory(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TradeCategoryMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE026);
            return View(model);
        }

        public ActionResult GetUpdateInventoryStatus(string srcode)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.InventoryStatusMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE027);

            return View(model);
        }

        public ActionResult GetUpdateInventoryType(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.InventoryTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE028);
            return View(model);
        }

        public ActionResult GetUpdateMoneyType(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MoneyTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE029);
            return View(model);
        }

        public ActionResult GetUpdateUnitPriceUnit(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UnitPriceUnitMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE030);
            return View(model);
        }

        public ActionResult GetUpdateLogType(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.LogTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE031);
            return View(model);
        }

        public ActionResult GetUpdateDataType(string srcode)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.DataTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();


            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE032);
            return View(model);
        }
        public ActionResult GetUpdateWeightCalculationCode(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WeightCalculationCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();
            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE033);
            return View(model);
        }
        public ActionResult GetUpdateRawMaterialType(string srcode)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.RawMaterialTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();
            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE034);
            return View(model);
        }
        public ActionResult GetUpdateWarehouseCategoryCode(string srcode) //035
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WarehouseCategoryCodeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();
            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE035);
            return View(model);
        }

        public ActionResult GetUpdateRawMaterialLabelType(string srcode) //036
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.RawMaterialLabelTypeMaster, StandardPermissionProvider.Edit))
                return AccessDeniedView();
            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            var model = _ma012dal.GetMa012BySrcode(srcode, ClassificationCode.CLASSIFICATTIONCODE036);
            return View(model);
        }
        [HttpPost]
        public ActionResult Update_Due_Maker_Commodity_Days_Cal_Weigth(MA012 model)
        {
            if (!_permissionService.AuthorizeMA012(model.MNCLSCD, StandardPermissionProvider.Edit))
                return AccessDeniedView();

            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            bool Update = _ma012dal.Update(model);
            if (Update == true)
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction(GetRedirectAction(model.MNCLSCD.Trim()));
            }
            return ReturnErrorMessage("Update_Due_Maker_Commodity_Days_Cal_Weigth(writelog)");
        }
        #endregion
        [ActionName("CheckSRCodeInUsedOnOrtherData")] // khong duoc xoa
        public ActionResult CheckSRCodeInUsedOnSalePurchase(string srCode, string classiCode)
        {
            var model = Constraint.CheckMA012Constraint(classiCode, srCode);
            if (model != null)
                return Json(new { message = model }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete_Due_Maker_Commodity_Days_Cal_Weigth(string _array)
        {
            DAL.MA012.MA012_DAL _ma012dal = new DAL.MA012.MA012_DAL();
            string[] array = null;
            if (!String.IsNullOrEmpty(_array))
            {
                array = _array.Split('|');
            }
            if (array != null)
            {
                string classificationCode = array[0];
                if (!_permissionService.AuthorizeMA012(classificationCode, StandardPermissionProvider.Delete))
                    return AccessDeniedView();

                foreach (var valueIndex in array.Where(x => x != classificationCode))
                {
                    if (!_ma012dal.Delete(valueIndex, classificationCode))
                    {
                        return Json("/MasterSetting/Errormessage/Notification?actionName=Delete_Due_Maker_Commodity_Days_Cal_Weigth", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json("/MasterSetting/Errormessage/Notification?actionName=Delete_Due_Maker_Commodity_Days_Cal_Weigth", JsonRequestBehavior.AllowGet);
        }

        public string GetRedirectAction(string classifiCationCode)
        {
            string redirect = String.Empty;
            switch (classifiCationCode)
            {
                case ClassificationCode.CLASSIFICATTIONCODE001:
                    redirect = "DueDateTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE002:
                    redirect = "DaysInMonthMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE003:
                    redirect = "CalculationTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE004:
                    redirect = "WeightDisplayCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE005:
                    redirect = "MakerCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE006:
                    redirect = "CommodityCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE007:
                    redirect = "StatusCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE008:
                    redirect = "SectionCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE009:
                    redirect = "DamageCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE010:
                    redirect = "CountryCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE011:
                    redirect = "BusinessTypeCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE012:
                    redirect = "CurrencyCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE013:
                    redirect = "ShiftCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE014:
                    redirect = "MachineNoMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE015:
                    redirect = "GradeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE016:
                    redirect = "ReasonForChangingMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE017:
                    redirect = "BankCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE018:
                    redirect = "ExchangeRateTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE019:
                    redirect = "PriceTermMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE020:
                    redirect = "SettlementTermMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE021:
                    redirect = "TypeofTermsMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE022:
                    redirect = "PackingTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE023:
                    redirect = "InterruptedReasonCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE024:
                    redirect = "DeliveryConditionMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE025:
                    redirect = "ContractTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE026:
                    redirect = "TradeCategoryMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE027:
                    redirect = "InventoryStatusMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE028:
                    redirect = "InventoryTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE029:
                    redirect = "MoneyTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE030:
                    redirect = "UnitPriceUnitMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE031:
                    redirect = "LogTypeMaster";
                    break;

                case ClassificationCode.CLASSIFICATTIONCODE032:
                    redirect = "DataTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE033:
                    redirect = "WeightCalculationCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE034:
                    redirect = "RawMaterialTypeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE035:
                    redirect = "WarehouseCategoryCodeMaster";
                    break;
                case ClassificationCode.CLASSIFICATTIONCODE036: //036
                    redirect = "RawMaterialLabelTypeMaster";
                    break;
            }

            return redirect;
        }
        #endregion

   

        #region SteelGrade
        public ActionResult SteelGradeMaster()
        {

            return View();
        }
        public ActionResult AjaxHandlerSteelGradeMaster(jQueryDataTableParamModel param)
        {

            DAL.SteelGrade.SteelGrade_DAL _SteelGradeDal = new DAL.SteelGrade.SteelGrade_DAL();
            var countitem = _SteelGradeDal.GetTotalRecord(param.sSearch);
            var displayItem = _SteelGradeDal.GetTotalDisplayRecord(param.iDisplayStart, param.iDisplayLength, param.sSearch);

            var result = displayItem.Select(c => new
            {
                C = c.C,
                Al = c.Al,
                Grade = c.Grade,
                Id = c.Id,
                Mn = c.Mn,
                P = c.P,
                S = c.S,
                Si = c.Si,
                SAEsymbol = c.SAEsymbol,
                RGSDT = String.Format("{0:dd/MM/yyyy}", c.RGSDT),
                RGSTM = c.RGSTM,
                UPDT = String.Format("{0:dd/MM/yyyy}", c.UPDT),
                UPTM = c.UPTM

            }).ToList();
            return Json(new
            {
                iTotalRecords = countitem,
                iTotalDisplayRecords = countitem,
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CheckSteelGradeCodeExists(string Code)
        {
            DAL.SteelGrade.SteelGrade_DAL _SteelGradeDal = new DAL.SteelGrade.SteelGrade_DAL();
            return Json(_SteelGradeDal.CheckSteelGradeCodeExists(Code), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUpdateSteelGrade(string Code)
        {


            DAL.SteelGrade.SteelGrade_DAL _SteelGradeDal = new DAL.SteelGrade.SteelGrade_DAL();
            var model = _SteelGradeDal.GetSteelGradeByCode(Code);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateSteelGrade(SteelGrade model)
        {

            DAL.SteelGrade.SteelGrade_DAL _SteelGradeDal = new DAL.SteelGrade.SteelGrade_DAL();
            if (_SteelGradeDal.Update(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Update");
                return RedirectToAction("SteelGradeMaster");
            }
            return ReturnErrorMessage("UpdateSteelGrade(writelog)");
        }

        public ActionResult DeleteSteelGrade(string arrayCode)
        {


            DAL.SteelGrade.SteelGrade_DAL _SteelGradeDal = new DAL.SteelGrade.SteelGrade_DAL();
            var array = arrayCode.Split('|');
            foreach (var cusCode in array)
            {
                bool delete = _SteelGradeDal.Delete(cusCode);
                if (delete == false)
                {
                    return Json("/MasterSetting/Errormessage/Notification?actionName=DeleteSteelGrade", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInsertSteelGrade()
        {
            return View();
        }


        [HttpPost]
        public ActionResult InsertSteelGrade(SteelGrade model)
        {

            DAL.SteelGrade.SteelGrade_DAL _SteelGradeDal = new DAL.SteelGrade.SteelGrade_DAL();
            if (_SteelGradeDal.Insert(model))
            {
                TempData[TopProSystem.Models.ConstantData.Notification_key] = MessageNotifi("Insert");
                return RedirectToAction("SteelGradeMaster");
            }
            return ReturnErrorMessage("InsertSteelGrade(writelog)");
        }


        public JsonResult CheckSteelGradeInUsedReference(string grade)
        {
            var checkused = Constraint.CheckConstraintSteelGrade(grade);
            if (checkused != null)
                return Json(new { message = checkused }, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        #endregion 

        #region public method

        public string MessageNotifi(string actionName)
        {
            var action = actionName.ToLower().Trim();
            string message = String.Empty;
            switch (action)
            {
                case "insert":
                    message = ErrorResource.Insert;
                    break;
                case "update":
                    message = ErrorResource.Change;
                    break;
                case "delete":
                    message = ErrorResource.Delete;
                    break;
                default:
                    message = "Unnown action";
                    break;
            }
            return "SuccessAlert('" + message + "')";
        }

        public ActionResult ReturnErrorMessage(string _actionName)
        {
            return RedirectToAction("Notification", "ErrorMessage", new { actionName = _actionName });
        }

        #endregion

        #region printer machine

        public ViewResult GetPrinter()
        {
            return View();
        }
        public JsonResult GetAllPrinterAjaxHandler(jQueryDataTableParamModel param)
        {
            var dal = new DAL.Printer.PrinterMachine();
            int TotalRecord = dal.GetTotalRecord();
            var DisplayRecord = dal.GetTotalDisplayRecord();
            var rs = DisplayRecord.Select(x => new
            {
                id = x.ID,
                printerName = x.PrinterName,
                paperName = x.PaperName,
                isHorizontal = x.isHorizontal,
                copy = x.Copies,
                active = x.Active
            });

            return Json(new
            {
                iTotalRecords = TotalRecord,
                iTotalDisplayRecords = DisplayRecord,
                aaData = rs
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllPaperNameFromSystem()
        {
            var List = Extension.Printer.PrinterMachine.GetAllPaperName();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllPrinterNameFromSystem()
        {
            var list = Extension.Printer.PrinterMachine.GetAllPrinterName();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InsertPrinter(Models.PrinterSetting printer)
        {
            if (printer != null)
            {
                var _printer = new PrinterSetting();
                _printer.PrinterName = printer.PrinterName;
                _printer.PaperName = printer.PaperName;
                _printer.Copies = printer.Copies;
                _printer.isHorizontal = (byte)printer.isHorizontal;
                _printer.Active = Convert.ToByte(printer.Active);

                var dal = new DAL.Printer.PrinterMachine();
                if (dal.Insert(_printer))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePrinter(string array)
        {
            string[] _array = null;
            if (!string.IsNullOrEmpty(array))
            {
                _array = array.Split('|');
            }
            if (_array != null)
            {
                var dal = new DAL.Printer.PrinterMachine();
                if (dal.Delete(_array))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetprinterByid(int id)
        {
            var dal = new DAL.Printer.PrinterMachine();
            return Json(dal.GetPrinterById(id),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdatePrinter(PrinterSetting printerSetting)
        {
            var dal = new DAL.Printer.PrinterMachine();
            return Json(dal.Update(printerSetting), JsonRequestBehavior.DenyGet);
        }

        #endregion


        #region backupdatabase
        public void backUpDatabase()
        {

            var connectionString = ConfigurationManager.ConnectionStrings["TopProSystemEntities"].ConnectionString;
            System.Data.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.EntityClient.EntityConnectionStringBuilder(connectionString);
            connectionString = efBuilder.ProviderConnectionString;
            // read backup folder from config file ("C:/temp/")
            var backupFolder = "D:/";

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
         
            // set backupfilename (you will get something like: "C:/temp/MyDatabase-2013-12-07.bak")
            var backupFileName = String.Format("{0}{1}-{2}.bak",
                backupFolder, sqlConStrBuilder.InitialCatalog,
                DateTime.Now.ToString("yyyy-MM-dd"));

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("BACKUP DATABASE {0} TO DISK='{1}'",
                    sqlConStrBuilder.InitialCatalog, backupFileName);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename="+ "backupTopProSystem.bak");
            Response.TransmitFile(backupFileName); //backup must be located in folder in your application folder, that folder named *backups*
            Response.End();
            System.IO.File.Delete(backupFileName);
        }
        #endregion
    }
}