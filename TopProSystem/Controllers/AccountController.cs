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

        [AllowAnonymous]
        public ActionResult GetUserLoggedIn()
        {
            Extension.LoginControll.LoginControll loginControll = new Extension.LoginControll.LoginControll();
            var ma003_dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();
            List<MA003> mA003s = new List<MA003>();
            foreach (var id in loginControll._GetUserLoggedIn())
            {
                mA003s.Add(ma003_dal.GetMA003(id));
            }
            return View(mA003s);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RemoveUserSession(string userid)
        {
            Extension.LoginControll.LoginControll loginControll = new Extension.LoginControll.LoginControll();
            loginControll._DistroyAll();
            ResetSessionLogin();
            return RedirectToAction("GetUserLoggedIn");
        }

        public ActionResult GetLogin()
        {
            var authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session[ConstantData.SessionUserID])))
            {
                var user = context.GetUserData();
                CreateSession(user.MCIDCD, user.MCIDNM);
                return RedirectToAction("GetMainMenu", "Home");
            }
            else if (authCookie != null && !string.IsNullOrEmpty(Convert.ToString(System.Web.HttpContext.Current.Session[ConstantData.SessionUserID])))
            {
                return RedirectToAction("GetMainMenu", "Home");
            }
            return View();
        }

        public bool DetectMobile()
        {
            string u = Request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public ActionResult GetLogin(LoginModel model, string ReturnUrl)
        {
            if (DetectMobile())
            {
                ReturnUrl = Url.Action("GetRawMaterialWarehousingResult", "RawMaterial", new { area = "" });
            }

            Areas.MasterSetting.DAL.MA003.MA003_DAL dal = new Areas.MasterSetting.DAL.MA003.MA003_DAL();
            try
            {
                if (ModelState.IsValid)
                {
                    var _model = dal.CheckUserNameAndPasswordMatch(model.UserName, model.Password);
                    if (_model != null)
                    {
                        Extension.LoginControll.LoginControll loginControll = new Extension.LoginControll.LoginControll();
                        if (!loginControll._loginControll(_model.MCIDCD))
                        {
                            ModelState.AddModelError("", "User is already logged in !");
                            return View();
                        }

                        CreateSession(_model.MCIDCD, _model.MCIDNM);
                        context.SetAuthenticationToken(_model.MCIDNM, false, _model);
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
                //WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();
                //WriteLogError_DAL.WriteLogErrorException(ex);
                //ModelState.AddModelError("", ex.Message);
                // return View();
                return Content(ex.Message);
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
            //foreach (var ar in actionRoles)
            //{
            //    model.AvailableActionRoles.Add(new PermissionAction
            //    {
            //        Id = ar.Id,
            //        Name = ar.Name
            //    });
            //}
            foreach (var pr in permissionRecords)
            {
                //foreach (var ar in actionRoles)
                //{
                foreach (var cr in customerRoles)
                {
                    bool allowed = db.Role_Mapping.Count(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id) > 0;
                    bool allowedAction = db.Role_Mapping_Action.Count(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id /*&& x.Action_Id == ar.Id*/) > 0;
                    if (!model.Allowed.ContainsKey(pr.SystemName))
                        model.Allowed[pr.SystemName] = new Dictionary<string, bool>();
                    model.Allowed[pr.SystemName][cr.Id] = allowed;
                    //if (!model.AllowedAction.ContainsKey(pr.SystemName + ar.Name))
                    //    model.AllowedAction[pr.SystemName + ar.Name] = new Dictionary<string, bool>();
                    //model.AllowedAction[pr.SystemName + ar.Name][cr.Id] = allowedAction;

                }
                //}
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
                    //string formKeyaction = "allow_" + cr.Id + "_" + pr.Id;
                    //var permissionRecordSystemNamesToRestrictAction = form[formKey] != null ? form[formKey].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    //foreach (var ar in permissionAction)
                    //{
                    //    bool allowaction = permissionRecordSystemNamesToRestrictAction.Contains(pr.SystemName + "_" + ar.Name);
                    //    if (allowaction)
                    //    {
                    //        var mappingrecord = db.Role_Mapping_Action.FirstOrDefault(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id && x.Action_Id == ar.Id);
                    //        if (mappingrecord == null)
                    //        {
                    //            var mapping = new Role_Mapping_Action();
                    //            mapping.CustomerRole_Id = cr.Id;
                    //            mapping.PermissionRecord_Id = pr.Id;
                    //            mapping.Action_Id = ar.Id;
                    //            db.Role_Mapping_Action.Add(mapping);
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var mappingrecord = db.Role_Mapping_Action.FirstOrDefault(x => x.PermissionRecord_Id == pr.Id && x.CustomerRole_Id == cr.Id && x.Action_Id == ar.Id);
                    //        if (mappingrecord != null)
                    //        {
                    //            db.Role_Mapping_Action.Remove(mappingrecord);
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //}

                }
            }
            return RedirectToAction("Permissions");
        }

        [AllowAnonymous]
        public ActionResult Log(string access)
        {
            if (string.IsNullOrEmpty(access))
            {
                return View("NotFound404Page");
            }
            string path = Server.MapPath("~/FileCreated/Log/Readme.log.txt");
            if (System.IO.File.Exists(path))
            {
                var contents = System.IO.File.ReadLines(path);
                StringBuilder rs = new StringBuilder();

                foreach (var obj in contents)
                {
                    rs.Append(obj);
                }
                return Content(rs.ToString());
            }
            return Content("File is not exists !");
        }
    }

}


