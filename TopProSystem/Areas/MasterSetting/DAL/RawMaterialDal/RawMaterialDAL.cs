using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopProSystem.Areas.MasterSetting.DAL.PUR001;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Areas.MasterSetting.DAL.RawMaterialDal
{
    public class RawMaterialDAL
    {
        private TopProSystemEntities db = new TopProSystemEntities();
        WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();
        PUR001_DAL PUR001_DALs = new PUR001_DAL();
        public RawMaterial GetNameAjax(RawMaterial model)
        {     DAL.MA001.MA001_DAL mA001_DAL = new DAL.MA001.MA001_DAL();
                   DAL.MA002.MA002_DAL mA002_DAL = new DAL.MA002.MA002_DAL();
        DAL.MA012.MA012_DAL mA012_DAL = new DAL.MA012.MA012_DAL();
        DAL.MA003.MA003_DAL mA003_DAL = new DAL.MA003.MA003_DAL();
        if (!string.IsNullOrEmpty(model.PurchaseContract.AACMDCD))
        {
            model.AACMDCD_dl = mA012_DAL.GetMa012BySrcode(model.PurchaseContract.AACMDCD.Trim(), "006").MNSRNM;
        }
        if (!string.IsNullOrEmpty(model.PurchaseContract.AAMKCD))
        {
            model.AAMKCD_dl = mA012_DAL.GetMa012BySrcode(model.PurchaseContract.AAMKCD.Trim(), "005").MNSRNM;
        }

        if (!string.IsNullOrEmpty(model.PurchaseContract.AAUSRCD ))
        {
            model.AAUSRCD_dl = mA002_DAL.GetUserName(model.PurchaseContract.AAUSRCD.Trim());
        }
        if (model.PurchaseContract.AACMDCD != null)
        {
            model.AASPLCD_dl = mA001_DAL.GetSalePurchase(model.PurchaseContract.AASPLCD.Trim()).MASPNM;
          }
        if (!string.IsNullOrEmpty(model.PurchaseContract.AAIDCD ))
        {
            model.AAIDCD_dl = mA003_DAL.GetMA003(model.PurchaseContract.AAIDCD.Trim()).MCIDNM;
        }
        if (!string.IsNullOrEmpty(model.PurchaseContract.AACTRTP))
        {
            model.AACTRTP_dl = mA012_DAL.GetMa012BySrcode(model.PurchaseContract.AACTRTP.Trim(), "025").MNSRNM;
        }
    
        return model;
        }

    }
}