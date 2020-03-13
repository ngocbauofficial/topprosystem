using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.MA010
{
    public class MA010_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError = new WriteLogError_DAL();
        public Models.MA010 GetMA010ByMKTXCD(string MKTXCD)
        {
            var model = db.MA010.SingleOrDefault(x => x.MKTXCD.Trim().Equals(MKTXCD.Trim()));
            return model;
        }
        public bool Insert(Models.MA010 tax)
        {
            var check = db.MA010.Count(x => x.MKTXCD.Trim() == tax.MKTXCD.Trim()) > 0;
            if (!check)
            {
                try
                {
                    tax.MKRGSDT = DateTime.Now; //register date
                    tax.MKRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8); // register time
                    db.MA010.Add(tax);
                    if (db.SaveChanges() > 0)
                    {
                        var TaxCode = tax.MKTXCD; //tax code
                        var _model = db.MA010.SingleOrDefault(x => x.MKTXCD.Equals(TaxCode));
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
        public bool Update(Models.MA010 tax)
        {
            try
            {
                var _model = db.MA010.Where(x => x.MKTXCD.Trim().Equals(tax.MKTXCD.Trim())).SingleOrDefault();
                if (_model != null)
                {
                    _model.MKTXDL = tax.MKTXDL;
                    _model.MKTXRT = tax.MKTXRT;
                    _model.MKFRDT = tax.MKFRDT;
                    _model.MKUPDT = DateTime.Now;
                    _model.MKUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);

                    if (db.SaveChanges() > 0)
                    {
                        _model = db.MA010.Where(x => x.MKTXCD.Trim().Equals(tax.MKTXCD)).SingleOrDefault();
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
        public bool Delete(string taxCode)
        {
            if (taxCode == null)
                return false;
            try
            {

                var _model = db.MA010.Where(x => x.MKTXCD.Trim().Equals(taxCode.Trim())).SingleOrDefault();
                if (_model != null)
                {
                    db.MA010.Remove(_model);
                    if (db.SaveChanges() > 0)
                    {
                        if (db.MA010.Where(x => x.MKTXCD.Trim().Equals(taxCode.Trim())).SingleOrDefault() == null)
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
                return db.MA010.Count();

            }
            else
            {
                return db.MA010.Where(x => x.MKTXCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Count();

            }
        }

        public List<Models.MA010> GetTotalDisplayRecord(int skip, int take, string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam) || String.IsNullOrWhiteSpace(searchParam))
            {
                return db.MA010.OrderByDescending(x => new { x.MKRGSDT, x.MKRGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return db.MA010.OrderByDescending(x => new { x.MKRGSDT, x.MKRGSTM }).Where(x => x.MKTXCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Skip(skip).Take(take).ToList();

            }
        }
        public bool CheckTaxCodeExists(string taxCode)
        {
            var _model = db.MA010.Where(x => x.MKTXCD.Trim().Equals(taxCode)).FirstOrDefault();
            return _model == null;
        }
    }
}