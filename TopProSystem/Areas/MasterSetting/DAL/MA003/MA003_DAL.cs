using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.DAL.MA003
{
    public class MA003_DAL
    {
        private Models.TopProSystemEntities dc = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();

        public bool Insert(Models.MA003 model)
        {
            try
            {
                var _passWord = model.MCPASS;
                var _hashPass = TopProSystem.Models.BcryptHashPass.HashPassword(_passWord);
                model.MCPASS = _hashPass; // Password
                model.MCRGSDT = DateTime.Now; //register date
                model.MCRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8); // register time
                bool check = dc.MA003.Count(x => x.MCIDCD.Trim() == model.MCIDCD.Trim()) > 0;
                if (!check)
                {
                    dc.MA003.Add(model);
                    if (dc.SaveChanges() > 0)
                    {
                        return true;
                    }
                }
                else if (check)
                {
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
        public bool Update(Models.MA003 model)
        {
            try
            {
                var _model = dc.MA003.Where(x => x.MCIDCD.Trim().Equals(model.MCIDCD.Trim())).SingleOrDefault();
                if (_model != null)
                {
                    _model.MCIDNM = model.MCIDNM;
                    _model.MCSCTLV = model.MCSCTLV;
                    _model.MCSCTNC = model.MCSCTNC;
                    _model.MCUPDT = DateTime.Now; //update date
                    _model.MCUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8); // update time

                    if (dc.SaveChanges() > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                WriteLogError_DAL.WriteLogErrorException(ex);
                return false;
            }
        }
        public bool Delete(string userid)
        {
            try
            {
                var model = dc.MA003.SingleOrDefault(x => x.MCIDCD.Equals(userid));
                if (model != null)
                {
                    dc.MA003.Remove(model);
                    if (dc.SaveChanges() > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                WriteLogError_DAL.WriteLogErrorException(ex);
                return false;
            }
        }

        public Models.MA003 GetListForient()
        {
            var model = new Models.MA003();        
            model.SectionCode = dc.MA012.Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE008)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + " - " + x.MNSRNM });
            return model;
        }

        public int GetTotalRecord(string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return dc.MA003.Count();
            }
            else
            {
                return dc.MA003.Where(x => x.MCIDCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Count();
            }
        }

        public List<Models.MA003> GetTotalDisplayRecord(string searchParam, int skip, int take)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return dc.MA003.OrderByDescending(x => new { x.MCRGSDT, x.MCRGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return dc.MA003.Where(x => x.MCIDCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).OrderByDescending(x => new { x.MCRGSDT, x.MCRGSTM }).Skip(skip).Take(take).ToList();
            }
        }
        public bool CheckIdCodeExits(string userid)
        {
            var _model = dc.MA003.Where(x => x.MCIDCD == userid).SingleOrDefault();
            return _model == null;
        }

        public Models.MA003 GetMA003(string userid)
        {
            var model = dc.MA003.SingleOrDefault(x => x.MCIDCD.Trim().Equals(userid.Trim()));
            return model;
        }

        public Models.MA003 CheckUserNameAndPasswordMatch(string uname, string psw)
        {
            var _model = dc.MA003.SingleOrDefault(x => x.MCIDCD.Trim().ToUpper().Equals(uname.Trim().ToUpper()));
            if (_model != null)
            {
                if (TopProSystem.Models.BcryptHashPass.VerifyHashPass(psw,_model.MCPASS))
                {
                    return _model;
                }
            }
            return null;
        }

        public bool ChangePsw(string uname, string psw)
        {
            var _model = dc.MA003.SingleOrDefault(x => x.MCIDCD.Trim().ToUpper().Equals(uname.Trim().ToUpper()));
            if (_model != null)
            {
                _model.MCPASS = TopProSystem.Models.BcryptHashPass.HashPassword(psw);
                if (dc.SaveChanges()>0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}