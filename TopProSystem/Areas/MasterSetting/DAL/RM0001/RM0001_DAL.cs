using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Areas.MasterSetting.DAL.RM0001
{
    public class RM0001_DAL
    {
        public void UpdateSomthingBeforeInsert()
        {

            using (var dc = new TopProSystemEntities())
            {
                var sqlParam = new SqlParameter[]
                {
                   new SqlParameter("@PERSON_INCHARGE",HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString()),
                };

                dc.Database.ExecuteSqlCommand("EXEC DO_STOCK_ENTRY_RS @PERSON_INCHARGE", sqlParam);
            }

        }

        public enum InventoryAndInspectionExistsRes
        {
            inventory = 1,
            inspection = 2,
            noexists = 3,
            alreadyCompleted = 4
        }
        public InventoryAndInspectionExistsRes CheckInspectionNoOrInventoryExists(string inventory_no, string inspection_no)
        {
            using (var dc = new TopProSystemEntities())
            {
                var user_id = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
                var data = dc.RM0001.Where(x => x.SEURID.Equals(user_id)).ToArray();
                if (Array.Exists(data, x => x.SEINVNO == inventory_no))
                {
                    return InventoryAndInspectionExistsRes.inventory;
                }
                else if (Array.Exists(data, x => x.SEISPNO == inspection_no))
                {
                    return InventoryAndInspectionExistsRes.inspection;
                }
                else
                {
                    var completed = dc.INV001.Any(x => x.CAISPNO.Equals(inspection_no) && x.CAINVST.Equals("1"));
                    if (completed)
                    {
                        return InventoryAndInspectionExistsRes.alreadyCompleted;
                    }
                }
                return InventoryAndInspectionExistsRes.noexists;
            }
        }
        public enum InsertMesage
        {
            Success = 0,
            Error = 1,
            InventoryExists = 2,
            InspectionExists = 3,
            InspectionCompleted = 4
        }
        public InsertMesage Insert(Models.RM0001 model)
        {
            try
            {
                using (var dc = new TopProSystemEntities())
                {
                    var ms = CheckInspectionNoOrInventoryExists(model.SEINVNO, model.SEISPNO);
                    if (ms == InventoryAndInspectionExistsRes.inspection)
                    {
                        return InsertMesage.InspectionExists;
                    }
                    else if (ms == InventoryAndInspectionExistsRes.inventory)
                    {
                        return InsertMesage.InventoryExists;
                    }
                    else if (ms == InventoryAndInspectionExistsRes.alreadyCompleted)
                    {
                        return InsertMesage.InspectionCompleted;
                    }

                    model.SEURID = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
                    dc.RM0001.Add(model);
                    if (dc.SaveChanges() > 0)
                    {
                        return InsertMesage.Success;
                    }
                    return InsertMesage.Error;
                }
            }
            catch (Exception ex)
            {
                WriteLogError_DAL writeLogError_DAL = new WriteLogError_DAL();
                writeLogError_DAL.WriteLogErrorException(ex);
                return InsertMesage.Error;
            }
        }

        public bool Delete(int id)
        {
            using (var dc = new TopProSystemEntities())
            {
                try
                {
                    var obj = dc.RM0001.Find(id);
                    dc.RM0001.Remove(obj);
                    dc.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {

                    WriteLogError_DAL writeLogError_DAL = new WriteLogError_DAL();
                    writeLogError_DAL.WriteLogErrorException(ex);
                    return false;
                }

            }
        }

        public int GetTotalRecord(string searchParam)
        {
            using (var dc = new TopProSystemEntities())
            {
                var userid = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
                if (string.IsNullOrEmpty(searchParam))
                {
                    return dc.RM0001.Where(x => x.SEURID.Trim().Equals(userid)).Count();
                }
                else
                {
                    string[] _array = searchParam.Split('|');
                    string date = _array[0], locationcode = _array[1], inventory = _array[2], inspection = _array[3];
                    var a = dc.RM0001.Where(x => x.SEURID.Trim().Equals(userid)).ToList(); ;

                    if (!string.IsNullOrEmpty(date))
                    {
                        var Date = DateTime.Parse(date);
                        a = a.Where(x => x.SERGSDT.Equals(Date)).ToList();
                    }
                    if (!string.IsNullOrEmpty(locationcode))
                    {
                        a = a.Where(x => x.SELCTCD.Equals(locationcode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(inventory))
                    {
                        a = a.Where(x => x.SEINVNO.Equals(inventory)).ToList();
                    }
                    if (!string.IsNullOrEmpty(inspection))
                    {
                        a = a.Where(x => x.SEISPNO.Equals(inspection)).ToList();
                    }

                    return a.Count;
                }

            }
        }

        public IEnumerable<Models.RM0001> GetTotalDisplayRecord(string searchParam, int skip, int take)
        {
            using (var dc = new TopProSystemEntities())
            {
                var userid = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
                if (string.IsNullOrEmpty(searchParam))
                {
                    return dc.RM0001.Where(x => x.SEURID.Trim().Equals(userid)).OrderByDescending(x => x.SERGSDT).Skip(skip).Take(take).ToList();
                }
                else
                {
                    string[] _array = searchParam.Split('|');
                    string date = _array[0], locationcode = _array[1], inventory = _array[2], inspection = _array[3];
                    var a = dc.RM0001.Where(x => x.SEURID.Trim().Equals(userid)).ToList(); ;

                    if (!string.IsNullOrEmpty(date))
                    {
                        var Date = DateTime.Parse(date);
                        a = a.Where(x => x.SERGSDT.Equals(Date)).ToList();
                    }
                    if (!string.IsNullOrEmpty(locationcode))
                    {
                        a = a.Where(x => x.SELCTCD.Equals(locationcode)).ToList();
                    }
                    if (!string.IsNullOrEmpty(inventory))
                    {
                        a = a.Where(x => x.SEINVNO.Equals(inventory)).ToList();
                    }
                    if (!string.IsNullOrEmpty(inspection))
                    {
                        a = a.Where(x => x.SEISPNO.Equals(inspection)).ToList();
                    }

                    return a.OrderByDescending(x => x.SERGSDT).Skip(skip).Take(take).ToList();
                }
            }
        }

    }
}