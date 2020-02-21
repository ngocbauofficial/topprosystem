using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.MA007
{
    public class MA007_DAL
    {
        private Models.TopProSystemEntities dc = new Models.TopProSystemEntities();
        private WriteLogError_DAL logError_DAL = new WriteLogError_DAL();
        public int GetTotalRecord(string searchParam)
        {
            if (string.IsNullOrEmpty(searchParam) || string.IsNullOrWhiteSpace(searchParam))
            {
                return dc.MA007.Count();
            }
            else
            {
                var _searchParam = decimal.Parse(searchParam);
                return dc.MA007.Count(x => x.MHBKMNT.Trim().Contains(searchParam.Trim()));
            }
        }

        public IEnumerable<Models.MA007> GetTotalDisplayRecord(string searchParam, int take, int skip)
        {
            if (string.IsNullOrEmpty(searchParam) || string.IsNullOrWhiteSpace(searchParam))
            {
                return dc.MA007.OrderByDescending(x => x.MHRGSDT).Skip(skip).Take(take);
            }
            else
            {
                var _searchParam = decimal.Parse(searchParam);
                return dc.MA007.Where(x => x.MHBKMNT.Trim().Contains(searchParam.Trim())).OrderByDescending(x => x.MHRGSDT).Skip(skip).Take(take);
            }
        }

        public bool Insert(Models.MA007 model)
        {
            try
            {
                if (model != null)
                {
                    bool check = dc.MA007.Where(x => x.MHBKMNT.Trim().Equals(model.MHBKMNT)).SingleOrDefault() != null;
                    if (check)
                    {
                        return true;
                    }

                    model.MHRGSDT = DateTime.Now;
                    model.MHRGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);

                    dc.MA007.Add(model);
                    if (dc.SaveChanges() > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logError_DAL.WriteLogErrorException(ex);
                return false;
            }
        }

        public bool Update(Models.MA007 model)
        {
            try
            {
                if (model != null)
                {
                    var _model = dc.MA007.Find(model.ID);
                    _model.MHUPDT = DateTime.Now;
                    _model.MHUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    _model.MHCLSDT = model.MHCLSDT;
                    if (dc.SaveChanges()>0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logError_DAL.WriteLogErrorException(ex);
                return false;
            }
        }

        public bool Delete(string[] array)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach(var id in array)
                    {
                        var _id = int.Parse(id);
                        var model = dc.MA007.Find(_id);
                        if (model != null)
                        {
                            dc.MA007.Remove(model);
                            if (dc.SaveChanges() <= 0)
                            {
                                scope.Dispose();
                                return false;
                            }
                        }
                        
                    }
                    scope.Complete();

                    return true;
                }
            }
            catch (Exception ex)
            {
                logError_DAL.WriteLogErrorException(ex);
                return false;
            }
        }
        public Models.MA007 GetClosingDate(int _id)
        {
            return dc.MA007.Find(_id);
        }
        public bool CheckBookingMonthExists(string bookingMonth)
        {
            var model = dc.MA007.Where(x => x.MHBKMNT.Trim().Equals(bookingMonth)).SingleOrDefault();
            return model == null;
        }
    }
}