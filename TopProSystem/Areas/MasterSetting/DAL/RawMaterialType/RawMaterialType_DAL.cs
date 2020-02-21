using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.RawMaterialType
{
    public class RawMaterialType_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError = new WriteLogError_DAL();

        public Models.RawMaterialType GetRawMaterialTypeByCode(string code)
        {
            var model = db.RawMaterialTypes.First(x=>x.RMTCD == code);
            return model;
        }

        public bool Insert(Models.RawMaterialType model)
        {
            bool check = db.RawMaterialTypes.Count(x => x.RMTCD.Trim() == model.RMTCD.Trim()) > 0;
            if (check == false)
            {
                try
                {
                    model.RMTGSDT = DateTime.Now;
                    model.RMTGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    db.RawMaterialTypes.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        if (db.RawMaterialTypes.SingleOrDefault(x => x.RMTCD.Trim().Equals(model.RMTCD.Trim())) != null)
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

        public bool Update(Models.RawMaterialType model)
        {
            try
            {

                var _model = db.RawMaterialTypes.Single(x => x.RMTCD.Trim().Equals(model.RMTCD.Trim()));
                _model.RMTNM = model.RMTNM;
                _model.RMTUPDT = DateTime.Now;
                _model.RMTUPTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
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
        public bool Delete(string code)
        {
            try
            {
                var model = db.RawMaterialTypes.Single(x => x.RMTCD.Trim().Equals(code.Trim()));
                db.RawMaterialTypes.Remove(model);
                if (db.SaveChanges() > 0)
                {
                    if (db.RawMaterialTypes.SingleOrDefault(x => x.RMTCD.Trim().Equals(code.Trim())) == null)
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

        public int GetTotalRecord(string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return db.RawMaterialTypes.Count();
            }
            else
            {
                return db.RawMaterialTypes.Where(x => x.RMTCD.Contains(searchParam)).Count();
            }
        }

        public List<Models.RawMaterialType> GetTotalDisplayRecord(int skip, int take, string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam) || String.IsNullOrWhiteSpace(searchParam))
            {
                return db.RawMaterialTypes.OrderByDescending(x => new { x.RMTGSDT, x.RMTGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return db.RawMaterialTypes.OrderByDescending(x => new { x.RMTGSDT, x.RMTGSTM }).Where(x => x.RMTCD.Trim().Contains(searchParam)).Skip(skip).Take(take).ToList();

            }
        }
        public bool CheckRawMaterialTypeCodeExists(string code)
        {
            var model = db.RawMaterialTypes.SingleOrDefault(x => x.RMTCD.Trim().Equals(code.Trim()));
            return model == null;
        }

    }
}