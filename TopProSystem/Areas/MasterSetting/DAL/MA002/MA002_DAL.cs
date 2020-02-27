using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.MA002
{
    public class MA002_DAL
    {
        private Models.TopProSystemEntities dc = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();

        public bool Insert(Models.MA002 model)
        {
            try
            {
                bool check = dc.MA002.Count(x => x.MBUSRCD.Trim() == model.MBUSRCD.Trim()) > 0;
                if (check == false)
                {
                    model.MBRGSDT = DateTime.Now; //register date
                    model.MBRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8); // register date
                    dc.MA002.Add(model);
                    if (dc.SaveChanges() > 0)
                    {
                        var _model = dc.MA002.Where(x => x.MBUSRCD.Trim().Equals(model.MBUSRCD.Trim())).SingleOrDefault();
                        if (_model != null)
                        {
                            return true;
                        }
                    }
                }
                else if (check == true)
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
        public bool Update(Models.MA002 model)
        {
            try
            {
                var _model = dc.MA002.Where(x => x.MBUSRCD.Trim().Equals(model.MBUSRCD.Trim())).SingleOrDefault();
                if (_model != null)
                {
                    _model.MBUSAD1 = model.MBUSAD1;
                    _model.MBUSAD2 = model.MBUSAD2;
                    _model.MBUSAD3 = model.MBUSAD3;
                    _model.MBUSRNM = model.MBUSRNM;
                    _model.MBUSSNM = model.MBUSSNM;
                    _model.MBUSTEL = model.MBUSTEL;
                    _model.MBWTCAL = model.MBWTCAL;
                    _model.MBCOMCD = model.MBCOMCD;
                    _model.MBUNICD = model.MBUNICD;
                    _model.MBUPDT = DateTime.Now;
                    _model.MBUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    if (dc.SaveChanges() > 0)
                    {
                        _model = dc.MA002.Where(x => x.MBUSRCD.Trim().Equals(model.MBUSRCD.Trim())).SingleOrDefault();
                        if (_model != null)
                        {
                            return true;
                        }

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
                var model = dc.MA002.SingleOrDefault(x => x.MBUSRCD.Equals(userid));
                if (model != null)
                {
                    dc.MA002.Remove(model);
                    if (dc.SaveChanges() > 0)
                    {
                        model = dc.MA002.SingleOrDefault(x => x.MBUSRCD.Equals(userid));
                        if (model == null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
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
        public Models.MA002 GetMA002(string userid)
        {
            return dc.MA002.Single(x => x.MBUSRCD.Trim().Equals(userid.Trim()));
        }

        public int GetTotalRecord(string searchParam)
        {
            if (!String.IsNullOrEmpty(searchParam))
            {
                return dc.MA002.Where(x => x.MBUSRCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Count();
            }
            else
            {
                return dc.MA002.Count();
            }
        }

        public List<Models.MA002> GetTotalDisplayRecord(int skip, int take, string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return dc.MA002.OrderByDescending(x => new { x.MBRGSDT, x.MBRGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return dc.MA002.OrderByDescending(x => new { x.MBRGSDT, x.MBRGSTM }).Where(x => x.MBUSRCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Skip(skip).Take(take).ToList();
            }
        }

        public bool CheckUserNameExists(string username)
        {
            var _model = dc.MA002.Where(x => x.MBUSRNM.Trim().Equals(username.Trim())).SingleOrDefault();
            return _model == null;
        }

        public bool CheckUserIDExists(string userid)
        {
            var _model = dc.MA002.Where(x => x.MBUSRCD.Trim().Equals(userid.Trim())).SingleOrDefault();
            return _model == null;
        }
        public string GetUserName(string userid)
        {
            return dc.MA002.SingleOrDefault(x => x.MBUSRCD.Trim().Equals(userid)) != null ? dc.MA002.SingleOrDefault(x => x.MBUSRCD.Trim().Equals(userid)).MBUSRNM : string.Empty;
        }
        public IEnumerable<Models.MA002> GetAll()
        {
            return dc.MA002;
        }
        public Models.MA002 GetReference(Models.MA002 mA002)
        {
          
            mA002.WeightCalculationCode = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE033)).Select(x => new System.Web.Mvc.SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + " - " + x.MNSRNM });
            return mA002;
        }
    }
}