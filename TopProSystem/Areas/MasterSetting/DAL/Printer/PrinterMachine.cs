using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.Printer
{
    public class PrinterMachine
    {
        private Models.TopProSystemEntities dc = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();

        public int GetTotalRecord()
        {
            return dc.PrinterSettings.Count();
        }
        public IEnumerable<Models.PrinterSetting> GetTotalDisplayRecord()
        {
            return dc.PrinterSettings;
        }
        public bool Insert(Models.PrinterSetting printerSetting)
        {
            try
            {
                dc.PrinterSettings.Add(printerSetting);
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
        public bool Delete(string[] array)
        {
            if (array != null)
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var id in array)
                    {
                        var printer = dc.PrinterSettings.Find(int.Parse(id));
                        dc.PrinterSettings.Remove(printer);
                    }
                    if (dc.SaveChanges() < array.Length)
                    {
                        scope.Dispose();
                        return false;
                    }

                    scope.Complete();
                    return true;
                }

            }
            return false;
        }
        public bool Update(Models.PrinterSetting _model)
        {
            try
            {
                var model = dc.PrinterSettings.Find(_model.ID);
                model.isHorizontal = (byte)_model.isHorizontal;
                model.Copies = _model.Copies;
                model.PaperName = _model.PaperName;
                model.PrinterName = _model.PrinterName;

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
        public Models.PrinterSetting GetPrinterById(int id)
        {
            return dc.PrinterSettings.Find(id);
        }
    }
}