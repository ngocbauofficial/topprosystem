using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Areas.MasterSetting.DAL
{
    public class WriteLogError_DAL
    {
        public void WriteLogErrorException(Exception ex)
        {
            try
            {
                using (Models.TopProSystemEntities dc = new Models.TopProSystemEntities())
                {
                    var userID = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString();
                    LogError log = new LogError()
                    {
                        CreateDate = DateTime.Now,
                        ExceptionSource = ex.Source,
                        ExceptionMsg = ex.Message,
                        ExceptionType = ex.GetType().ToString(),
                        UserCreate = userID,
                        ExceptionURL = System.Web.HttpContext.Current.Request.Url.PathAndQuery,
                    };
                    dc.LogErrors.Add(log);
                    dc.SaveChanges();

                }

            }
            catch
            {

                throw;
            }

        }

        public void WriteLogUserAction(string tableName, string actionName, string recordPrivateKey)
        {
            try
            {
                using (Models.TopProSystemEntities dc = new Models.TopProSystemEntities())
                {
                    LogUserAction log = new LogUserAction()
                    {
                        UserCode = HttpContext.Current.Session[TopProSystem.Models.ConstantData.SessionUserID].ToString(),
                        TableName = tableName,
                        Date = DateTime.Now,
                        Remark = "Success",
                        ActionType = actionName,
                        RecordPrivateKey = recordPrivateKey
                    };
                    dc.LogUserActions.Add(log);
                    dc.SaveChanges();
                }
            }
            catch
            {
                throw;
            }


        }
    }
}