using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.MA004
{
    public class MA004_DAL
    {
        private Models.TopProSystemEntities dc = new Models.TopProSystemEntities();
        WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();

        public bool Insert(Models.MA004 model)
        {
            try
            {
                model.MDRGSDT = DateTime.Now; // register date
                model.MDRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8); //register time
                bool check = dc.MA004.Count(x => x.MDLCTCD.Trim() == model.MDLCTCD.Trim()) > 0;
                if (check == false)
                {
                    dc.MA004.Add(model);
                    if (dc.SaveChanges() > 0)
                    {
                        var _model = dc.MA004.Where(x => x.MDLCTCD.Trim().Equals(model.MDLCTCD.Trim())).SingleOrDefault();
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
        public bool Update(Models.MA004 model)
        {
            try
            {
                var _model = dc.MA004.Where(x => x.MDLCTCD.Trim().Equals(model.MDLCTCD.Trim())).SingleOrDefault();
                _model.MDLCTNM = model.MDLCTNM;
                _model.MDWRCTG = model.MDWRCTG;
                _model.MDUPDT = DateTime.Now;
                _model.MDUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                if (dc.SaveChanges() > 0)
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
        public bool Delete(string locationcode)
        {
            try
            {
                var model = dc.MA004.SingleOrDefault(x => x.MDLCTCD.Trim().Equals(locationcode.Trim()));
                if(model != null)
                {
                    dc.MA004.Remove(model);
                    if (dc.SaveChanges() > 0)
                    {
                        if (dc.MA004.SingleOrDefault(x => x.MDLCTCD.Trim().Equals(locationcode.Trim())) == null)
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
        public Models.MA004 GetMA004(string locationcode)
        {
            var model = dc.MA004.SingleOrDefault(x => x.MDLCTCD.Trim().Equals(locationcode.Trim()));
            return model;
        }
        public int GetTotalRecord(string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return dc.MA004.Count();
            }
            else
            {
                return dc.MA004.Where(x => x.MDLCTCD.Equals(searchParam)).Count();
            }
        }
        public List<Models.MA004> GetTotalDisplayRecord(string searchParam, int skip, int take)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return dc.MA004.OrderByDescending(x => new { x.MDRGSDT, x.MDRGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return dc.MA004.OrderByDescending(x => new { x.MDRGSDT, x.MDRGSTM }).Where(x => x.MDLCTCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).Skip(skip).Take(take).ToList();
            }
        }
        public bool CheckLocationCodeExists(string locationcode)
        {
            var _model = dc.MA004.Where(x => x.MDLCTCD.Trim().Equals(locationcode.Trim())).SingleOrDefault();
            return _model == null;
        }
    }
}