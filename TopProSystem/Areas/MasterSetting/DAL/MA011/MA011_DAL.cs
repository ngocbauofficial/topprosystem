using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.DAL.MA011
{
    public class MA011_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError = new WriteLogError_DAL();

        public Models.MA011 GetMa011byMLCSTCD(string MLCSTCD)
        {
            var model = db.MA011.Single(x => x.MLCSTCD.Trim().Equals(MLCSTCD.Trim()));
            return model;
        }

        public bool Update(Models.MA011 model)
        {
            try
            {
                var _model = db.MA011.SingleOrDefault(x => x.MLCSTCD.Trim().Equals(model.MLCSTCD.Trim()));
                if (_model != null)
                {
                    _model.MLCRDRK = model.MLCRDRK;
                    _model.MLVLDTR = model.MLVLDTR;
                    _model.MLCRDLM = model.MLCRDLM;
                    _model.MLUPDT = DateTime.Now;
                    _model.MLUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    if (db.SaveChanges() > 0)
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
        public bool Insert(Models.MA011 model)
        {
            bool check = db.MA011.Count(x => x.MLCSTCD.Trim() == model.MLCSTCD.Trim()) > 0;
            if (check == false)
            {
                try
                {
                    model.MLRGSDT = DateTime.Now;
                    model.MLRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    db.MA011.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        model = db.MA011.SingleOrDefault(x => x.MLCSTCD.Trim().Equals(model.MLCSTCD.Trim()));
                        if (model != null)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLogError.WriteLogErrorException(ex);
                    return false;
                }
            }
            return false;

        }
        public bool Delete(string MLCSTCD)
        {
            try
            {
                var model = db.MA011.Single(x => x.MLCSTCD.Trim().Equals(MLCSTCD.Trim()));
                db.MA011.Remove(model);
                if (db.SaveChanges() > 0)
                {
                    if (db.MA011.SingleOrDefault(x => x.MLCSTCD.Trim().Equals(MLCSTCD.Trim())) == null)
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
        public int GetTotalRecord(string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return db.MA011.Count();
            }
            else
            {
                return db.MA011.Where(x => x.MLCSTCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Count();
            }
        }

        public List<Models.MA011> GetTotalDisplayRecord(int skip, int take, string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam) || String.IsNullOrWhiteSpace(searchParam))
            {
                return db.MA011.OrderByDescending(x => new { x.MLRGSDT, x.MLRGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return db.MA011.OrderByDescending(x => new { x.MLRGSDT, x.MLRGSTM }).Where(x => x.MLCSTCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Skip(skip).Take(take).ToList();

            }
        }
        public bool CheckCustomerCodeExists(string cusCode)
        {
            var model = db.MA011.SingleOrDefault(x => x.MLCSTCD.Trim().Equals(cusCode.Trim()));
            return model == null;
        }

        public Models.MA011 GetListForient(Models.MA011 model)
        {
            model.CustomerCode = db.MA002.Select(x => new SelectListItem { Value = x.MBUSRCD, Text = x.MBUSRCD + " - " + x.MBUSRNM });
            return model;
        }
    }
}