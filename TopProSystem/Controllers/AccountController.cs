using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TopProSystem.Models;
using TopProSystem.Filters;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using TopProSystem.Areas.MasterSetting.Models;
using TopProSystem.Extension.AccountRole;
using TopProSystem.Areas.MasterSetting.DAL;

namespace TopProSystem.Controllers
{

    public class AccountController : Controller
    {

        private SessionContext context = new SessionContext();
        private void CreateSession(string userid, string name)
        {
            using (var MasterSetingEntities = new TopProSystem.Areas.MasterSetting.Models.TopProSystemEntities())
            {
                Session[ConstantData.SessionUserID] = userid;
                Session[ConstantData.SessionUserName] = name;
                WriteLog(userid, name);
            }
        }

        private void ResetSessionLogin()
        {
            Session.Abandon();
        }

        public ActionResult GetLogin()
        {
            var authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && System.Web.HttpContext.Current.Session[ConstantData.SessionUserID] != null) return RedirectToAction("GetMainMenu", "Home");
            return View();
        }

        [HttpPost]
        public ActionResult GetLogin(LoginModel model, string ReturnUrl)
        {
          
            Areas.MasterSetting.DAL.MA003.MA003_DAL dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();
            try
            {
                if (ModelState.IsValid)
                {
                    var _model = dal.CheckUserNameAndPasswordMatch(model.UserName, model.Password);
                    if (_model != null)
                    {
                        CreateSession(_model.MCIDCD, _model.MCIDNM);
                        context.SetAuthenticationToken(_model.MCIDCD, false, _model);
                        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
                                      && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("GetMainMenu", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username or Password is incorrect");
                        return View();
                    }

                }

                else
                {
                    ModelState.AddModelError("", "Please enter Username and Password");
                    return View();
                }
            }
            catch (Exception ex)
            {
                WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();
                WriteLogError_DAL.WriteLogErrorException(ex);
                ModelState.AddModelError("", ex.Message);
                return View();
            }
           

        }

        [HttpPost]
        public ActionResult SignOut()
        {
            ResetSessionLogin();
            FormsAuthentication.SignOut();
            return RedirectToAction("GetLogin", "Account");
        }

        [CustomAuthorize]
        public ActionResult GetProFile()
        {
            Areas.MasterSetting.DAL.MA003.MA003_DAL dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();
            string userid = Session[ConstantData.SessionUserID].ToString();
            return View(dal.GetMA003(userid));
        }


        [CustomAuthorize]
        public ActionResult CheckPasswordMatchWhenChange(string psw)
        {

            Areas.MasterSetting.DAL.MA003.MA003_DAL dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();
            string userCode = Session[ConstantData.SessionUserID].ToString();
            var _model = dal.CheckUserNameAndPasswordMatch(userCode, psw);
            return Json(_model == null, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult ChangePassword()
        {
            string newpsw = Request["newpsw"];
            string userCode = Session[ConstantData.SessionUserID].ToString();
            Areas.MasterSetting.DAL.MA003.MA003_DAL dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();
            if (dal.ChangePsw(userCode, newpsw))
            {
                return RedirectToAction("SignOut");
            }

            TempData[ConstantData.Notification_key] = ConstantData.FailMessage;
            return RedirectToAction("GetProfile");
        }


        private void WriteLog(string s, string n)
        {
            var session = new
            {
                session_id = s,
                session_name = n.Trim(),
                date = DateTime.Now,
                ip = GetLocalUser_IP()
            };

            string path = Server.MapPath("~/FileCreated/Log/Readme.log.txt");
            using (FileStream stream = new FileStream(path, FileMode.Append))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(session.ToString() + '\n');
                stream.Write(bytes, 0, bytes.Length);
            }

        }

        protected string GetLocalUser_IP()
        {
            try
            {
                string VisitorsIPAddr = string.Empty;
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    VisitorsIPAddr = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else if (System.Web.HttpContext.Current.Request.UserHostAddress.Length != 0)
                {
                    VisitorsIPAddr = System.Web.HttpContext.Current.Request.UserHostAddress;
                }
                return VisitorsIPAddr;
            }
            catch
            {
                return String.Empty;
            }
        }
        public ActionResult Permissions()
        {
            TopProSystemEntities db = new TopProSystemEntities();
            SecurityLevelService _securityLevelService = new SecurityLevelService();
            PermissionService _permissionService = new PermissionService();
            var model = new PermissionMappingModel();
            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var customerRoles = _securityLevelService.AllList();
            var actionRoles = db.PermissionActions.ToList();
            foreach (var pr in permissionRecords)
            {
                model.AvailablePermissions.Add(new PermissionRecord
                {
                    //Name = pr.Name,
                    Name = pr.Name,
                    Id = pr.Id,
                    SystemName = pr.SystemName,
                    Category = pr.Category
                });
            }
            foreach (var cr in customerRoles)
            {
                model.AvailableCustomerRoles.Add(new SecurityLevel
                {
                    Id = cr.Id,
                    Name = cr.Name
                });
            }
            foreach (var ar in actionRoles)
            {
                model.AvailableActionRoles.Add(new PermissionAction
                {
                    Id = ar.Id,
                    Name = ar.Name
                });
            }
            foreach (var pr in permissionRecords)
            {
                foreach (var ar in actionRoles)
                {
                    foreach (var cr in customerRoles)
                    {
                        bool allowed = db.Role_Mapping.Count(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id) > 0;
                        bool allowedAction = db.Role_Mapping_Action.Count(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id && x.Action_Id == ar.Id) > 0;
                        if (!model.Allowed.ContainsKey(pr.SystemName))
                            model.Allowed[pr.SystemName] = new Dictionary<string, bool>();
                        model.Allowed[pr.SystemName][cr.Id] = allowed;
                        if (!model.AllowedAction.ContainsKey(pr.SystemName + ar.Name))
                            model.AllowedAction[pr.SystemName + ar.Name] = new Dictionary<string, bool>();
                        model.AllowedAction[pr.SystemName + ar.Name][cr.Id] = allowedAction;

                    }
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Permissions(FormCollection form)
        {
            TopProSystemEntities db = new TopProSystemEntities();
            SecurityLevelService _securityLevelService = new SecurityLevelService();
            PermissionService _permissionService = new PermissionService();
            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var customerRoles = _securityLevelService.AllList();
            var permissionAction = db.PermissionActions.ToList();
            foreach (var cr in customerRoles)
            {
                string formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = form[formKey] != null ? form[formKey].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                foreach (var pr in permissionRecords)
                {
                    bool allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);
                    if (allow)
                    {
                        var mappingrecord = db.Role_Mapping.FirstOrDefault(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id);
                        if (mappingrecord == null)
                        {
                            var mapping = new Role_Mapping();
                            mapping.CustomerRole_Id = cr.Id;
                            mapping.PermissionRecord_Id = pr.Id;
                            db.Role_Mapping.Add(mapping);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var mappingrecord = db.Role_Mapping.FirstOrDefault(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id);
                        if (mappingrecord != null)
                        {
                            db.Role_Mapping.Remove(mappingrecord);
                            db.SaveChanges();
                        }
                    }
                    string formKeyaction = "allow_" + cr.Id + "_" + pr.Id;
                    var permissionRecordSystemNamesToRestrictAction = form[formKey] != null ? form[formKey].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    foreach (var ar in permissionAction)
                    {
                        bool allowaction = permissionRecordSystemNamesToRestrictAction.Contains(pr.SystemName + "_" + ar.Name);
                        if (allowaction)
                        {
                            var mappingrecord = db.Role_Mapping_Action.FirstOrDefault(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id && x.Action_Id == ar.Id);
                            if (mappingrecord == null)
                            {
                                var mapping = new Role_Mapping_Action();
                                mapping.CustomerRole_Id = cr.Id;
                                mapping.PermissionRecord_Id = pr.Id;
                                mapping.Action_Id = ar.Id;
                                db.Role_Mapping_Action.Add(mapping);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var mappingrecord = db.Role_Mapping_Action.FirstOrDefault(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id && x.Action_Id == ar.Id);
                            if (mappingrecord != null)
                            {
                                db.Role_Mapping_Action.Remove(mappingrecord);
                                db.SaveChanges();
                            }
                        }
                    }

                }
            }
            return RedirectToAction("Permissions");
        }
    }

}


