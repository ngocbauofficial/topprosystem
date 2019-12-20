using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopProSystem.Areas.MasterSetting.ForeignKeyConstraint;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Areas.MasterSetting.DAL.MA012
{
    public class MA012_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError = new WriteLogError_DAL();

        public Models.MA012 GetMa012BySrcode(string srcode, string CLASSIFICATTIONCODE)
        {
            var model = db.MA012.SingleOrDefault(x => x.MNCLSCD.Trim().Equals(CLASSIFICATTIONCODE) && x.MNSRCD.Trim().Equals(srcode.Trim()));
            return model;
        }
        public bool Insert(Models.MA012 model)
        {

            bool check = db.MA012.Count(x => x.MNCLSCD.Trim() == model.MNCLSCD.Trim() && x.MNSRCD.Trim() == model.MNSRCD.Trim()) > 0;
            if (check == false)
            {
                try
                {
                    model.MNRGSDT = DateTime.Now;
                    model.MNRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    db.MA012.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        if (db.MA012.SingleOrDefault(x => x.MNCLSCD.Trim().Equals(model.MNCLSCD.Trim()) && x.MNSRCD.Trim().Equals(model.MNSRCD.Trim())) != null)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    WriteLogError.WriteLogErrorException(ex);
                    return false;
                }
            }
            else if (check)
            {
                return true;
            }
            return false;

        }
        public bool Update(Models.MA012 model)
        {
            try
            {

                var _model = db.MA012.Single(x => x.MNCLSCD.Trim().Equals(model.MNCLSCD.Trim()) && x.MNSRCD.Trim().Equals(model.MNSRCD.Trim()));
                _model.MNSRNM = model.MNSRNM;
                _model.MNSRSNM = model.MNSRSNM;
                _model.MNUPDT = DateTime.Now;
                _model.MNUPTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                if (db.SaveChanges() > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                WriteLogError.WriteLogErrorException(ex);
                return false;
            }
        }
        public bool Delete(string srcode, string classificationCode)
        {
            try
            {
                var model = db.MA012.Single(x => x.MNCLSCD.Trim().Equals(classificationCode.Trim()) && x.MNSRCD.Trim().Equals(srcode.Trim()));
                db.MA012.Remove(model);
                if (db.SaveChanges() > 0)
                {
                    if (db.MA012.SingleOrDefault(x => x.MNCLSCD.Trim().Equals(classificationCode.Trim()) && x.MNSRCD.Trim().Equals(srcode.Trim())) == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                WriteLogError.WriteLogErrorException(ex);
                return false;
            }

        }
        public int GetTotalRecord(string searchParam, string MNCLSCD)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return db.MA012.Count(x => x.MNCLSCD == MNCLSCD);

            }
            else
            {
                return db.MA012.Count(x => x.MNCLSCD == MNCLSCD && x.MNSRCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper()));

            }
        }

        public List<Models.MA012> GetTotalDisplayRecord(int skip, int take, string searchParam, string MNCLSCD)
        {
            if (String.IsNullOrEmpty(searchParam) || String.IsNullOrWhiteSpace(searchParam))
            {
                return db.MA012.OrderByDescending(x => new { x.MNRGSDT, x.MNRGSTM }).Where(x => x.MNCLSCD == MNCLSCD).Skip(skip).Take(take).ToList();
            }
            else
            {
                return db.MA012.OrderByDescending(x => new { x.MNRGSDT, x.MNRGSTM }).Where(x => x.MNCLSCD == MNCLSCD && x.MNSRCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Skip(skip).Take(take).ToList();

            }
        }
        public string GetSRNameBySRCode(string classiCode, string srCode)
        {
            if (String.IsNullOrEmpty(srCode) || String.IsNullOrWhiteSpace(srCode)) return String.Empty;
            var model = db.MA012.SingleOrDefault(x => x.MNCLSCD.Trim().Equals(classiCode.Trim()) && x.MNSRCD.Equals(srCode.Trim()));
            if (model == null) return srCode;
            return model.MNSRNM;
        }
        public bool CheckSRCodeExists(string srcode, string classi)
        {
            var model = db.MA012.SingleOrDefault(x => x.MNCLSCD.Trim().Equals(classi.Trim()) && x.MNSRCD.Trim().Equals(srcode.Trim()));
            return model == null;
        }
        public IEnumerable<Models.MA012> GetAll()
        {
            return db.MA012;
        }
    }
}