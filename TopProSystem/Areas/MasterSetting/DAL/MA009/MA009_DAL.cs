using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.MA009
{
    public class MA009_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError = new WriteLogError_DAL();
        public bool Insert(Models.MA009 model)
        {

            bool check = db.MA009.Count(x => x.MJCRRCD.Trim() == model.MJCRRCD.Trim()) > 0;
            if (check == false)
            {
                try
                {
                    model.MJRGSDT = DateTime.Now;
                    model.MJRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);

                    db.MA009.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        var _model = db.MA009.Where(x => x.MJCRRCD == model.MJCRRCD).SingleOrDefault();
                        if (_model != null)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLogError.WriteLogErrorException(ex);
                }
            }
            else if (check)
            {
                return true;
            }
            return false;
        }
        public bool Update(Models.MA009 model)
        {
            try
            {
                var _model = db.MA009.Where(x => x.MJCRRCD == model.MJCRRCD).SingleOrDefault();
                if (_model != null)
                {
                    _model.MJEXRTT = model.MJEXRTT;
                    _model.MJEXRTD = model.MJEXRTD;
                    _model.MJEXRT = model.MJEXRT;
                    _model.MJTXEXR = model.MJTXEXR;
                    _model.MJUPDT = DateTime.Now;
                    _model.MJUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    if (db.SaveChanges() > 0)
                    {
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                WriteLogError.WriteLogErrorException(ex);
            }
            return false;
        }
        public bool Delete(string currencyCode)
        {
            try
            {
                var _model = db.MA009.Where(x => x.MJCRRCD.Trim().Equals(currencyCode.Trim())).SingleOrDefault();
                if (_model != null)
                {
                    db.MA009.Remove(_model);
                    if (db.SaveChanges() > 0)
                    {
                        if (db.MA009.Where(x => x.MJCRRCD.Trim().Equals(currencyCode.Trim())).SingleOrDefault() == null)
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogError.WriteLogErrorException(ex);
            }
            return false;
        }
        public int GetTotalRecord(string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return db.MA009.Count();

            }
            else
            {
                return db.MA009.Where(x => x.MJCRRCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Count();

            }
        }

        public List<Models.MA009> GetTotalDisplayRecord(int skip, int take, string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam) || String.IsNullOrWhiteSpace(searchParam))
            {
                return db.MA009.OrderByDescending(x => new { x.MJRGSDT, x.MJRGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return db.MA009.OrderByDescending(x => new { x.MJRGSDT, x.MJRGSTM }).Where(x => x.MJCRRCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Skip(skip).Take(take).ToList();

            }
        }
        public bool CheckCurrencyCodeInUsed(string currencyCode)
        {
            var _model = db.MA009.Where(x => x.MJCRRCD.Trim().Equals(currencyCode.Trim())).SingleOrDefault();
            return _model == null;
        }
        public Models.MA009 GetMA009(string curCode)
        {
            var _model = db.MA009.Where(x => x.MJCRRCD.Trim().Equals(curCode.Trim())).SingleOrDefault();
            return _model;
        }
        public Models.MA009 GetReference(Models.MA009 mA009)
        {
            //mA009.ExchangerateTypes = db.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE018)).Select(x => new System.Web.Mvc.SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD });
            //mA009.Currencys = db.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE012)).Select(x => new System.Web.Mvc.SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD });

            mA009.ExchangerateTypes = db.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE018)).Select(x => new System.Web.Mvc.SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + " - " + x.MNSRNM });
            mA009.Currencys = db.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE012)).Select(x => new System.Web.Mvc.SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + " - " + x.MNSRNM });
            return mA009;
        }
        public Models.MA009 GetExchangeRate(string curcode, string exchangeratetype,double ddate)
        {
            return db.MA009.Where(x => x.MJCRRCD.Trim().Equals(curcode.Trim()) && x.MJEXRTT.Trim().Equals(exchangeratetype) && x.MJEXRTD == ddate).SingleOrDefault();
        }


    }
}