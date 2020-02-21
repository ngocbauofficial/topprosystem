using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.DAL.MA006
{
    public class MA006_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError = new WriteLogError_DAL();

        public bool Delete(string proSpec)
        {
            try
            {
                var _model = db.MA006.Where(x => x.MFPRDSP.Trim().Equals(proSpec.Trim())).SingleOrDefault();
                if (_model != null)
                {
                    db.MA006.Remove(_model);
                    if (db.SaveChanges() > 0)
                    {
                        if (db.MA006.Where(x => x.MFPRDSP == proSpec).SingleOrDefault() == null)
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
        public bool Insert(Models.MA006 model)
        {

            bool check = db.MA006.Count(x => x.MFPRDSP.Trim() == model.MFPRDSP.Trim()) > 0;
            if (check == false)
            {
                try
                {
                    model.MFRGSDT = DateTime.Now;
                    model.MFRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    db.MA006.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        var _model = db.MA006.Where(x => x.MFPRDSP.Trim().Equals(model.MFPRDSP.Trim())).SingleOrDefault();
                        return true;
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
        public bool Update(Models.MA006 spec)
        {
            try
            {
                var _model = db.MA006.Where(x => x.MFPRDSP == spec.MFPRDSP).SingleOrDefault();
                if (_model != null)
                {
                    _model.MFPNSTY = spec.MFPNSTY;
                    _model.MFRMK10 = spec.MFRMK10;
                    _model.CMOCD = spec.CMOCD;
                    _model.MFUPDT = DateTime.Now;
                    _model.MFUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    if (db.SaveChanges() > 0)
                    {
                        _model = db.MA006.Where(x => x.MFPRDSP.Trim().Equals(spec.MFPRDSP.Trim())).SingleOrDefault();
                        if (_model != null)
                        {
                            return true;
                        }

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
                return db.MA006.Count();

            }
            else
            {
                return db.MA006.Where(x => x.MFPRDSP.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Count();

            }
        }

        public List<Models.MA006> GetTotalDisplayRecord(int skip, int take, string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam) || String.IsNullOrWhiteSpace(searchParam))
            {
                return db.MA006.OrderByDescending(x => new { x.MFRGSDT, x.MFRGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return db.MA006.OrderByDescending(x => new { x.MFRGSDT, x.MFRGSTM }).Where(x => x.MFPRDSP.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Skip(skip).Take(take).ToList();

            }
        }
        public bool CheckProductSpecInUse(string proSpec)
        {
            var _model = db.MA006.Where(x => x.MFPRDSP.Trim().Equals(proSpec.Trim())).SingleOrDefault();
            return _model == null;
        }
        public Models.MA006 GetMA006(string proSpec)
        {
            var _model = db.MA006.Where(x => x.MFPRDSP.Trim().Equals(proSpec.Trim())).SingleOrDefault();
            return _model;
        }
        public Models.MA006 GetListForient(Models.MA006 model)
        {
            model.CommodityCode = db.MA012.Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE006)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + " - " + x.MNSRNM });
            return model;
        }
    }
}