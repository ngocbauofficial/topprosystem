using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.MA005
{
    public class MA005_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError = new WriteLogError_DAL();
        public bool Insert(Models.MA005 coating)
        {
            bool check = db.MA005.Count(x => x.MECOAT.Trim() == coating.MECOAT.Trim()) > 0;
            if (check == false)
            {
                try
                {
                    coating.MERGSDT = DateTime.Now;
                    coating.MERGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    db.MA005.Add(coating);
                    if (db.SaveChanges() > 0)
                    {
                        var _model = db.MA005.Where(x => x.MECOAT.Trim().Equals(coating.MECOAT.Trim())).SingleOrDefault();
                        if (_model != null)
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
            else if (check)
            {
                return true;
            }
            return false;

        }
        public bool Update(Models.MA005 model)
        {
            try
            {
                var _model = db.MA005.Where(x => x.MECOAT.Trim().Equals(model.MECOAT.Trim())).SingleOrDefault();
                if (_model != null)
                {
                    _model.MECOATW = model.MECOATW;
                    _model.MEUPDT = DateTime.Now;
                    _model.MEUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    if (db.SaveChanges() > 0)
                    {
                        _model = db.MA005.Where(x => x.MECOAT.Trim().Equals(model.MECOAT.Trim())).SingleOrDefault();
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
        public bool Delete(string coatingCode)
        {
            var _model = db.MA005.Where(x => x.MECOAT.Trim().Equals(coatingCode.Trim())).SingleOrDefault();
            if (_model != null)
            {
                db.MA005.Remove(_model);
                if (db.SaveChanges() > 0)
                {
                    if (db.MA005.Where(x => x.MECOAT.Trim().Equals(coatingCode.Trim())).SingleOrDefault() == null)
                        return true;
                }
            }
            return false;
        }
        public Models.MA005 GetMA005(string meCoat)
        {
            return db.MA005.Where(x => x.MECOAT.Trim().Equals(meCoat.Trim())).Single();
        }
        public bool CheckCoatingExists(string coatingCode)
        {
            var _model = db.MA005.Where(x => x.MECOAT.Trim().Equals(coatingCode.Trim())).SingleOrDefault();
            return _model == null;
        }

        public int GetTotalRecord(string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return db.MA005.Count();

            }
            else
            {
                return db.MA005.Where(x => x.MECOAT.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Count();

            }
        }

        public List<Models.MA005> GetTotalDisplayRecord(int skip, int take, string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam) || String.IsNullOrWhiteSpace(searchParam))
            {
                return db.MA005.OrderByDescending(x => new { x.MERGSDT, x.MERGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return db.MA005.OrderByDescending(x => new { x.MERGSDT, x.MERGSTM }).Where(x => x.MECOAT.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Skip(skip).Take(take).ToList();

            }
        }
    }
}