using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Extension.AccountRole
{
    public class SecurityLevelService
    {
        TopProSystemEntities db = new TopProSystemEntities();
        public List<SecurityLevel> AllList()
        {
            var list = db.SecurityLevels;
            return list.ToList();
        }
        public string GetRoler(string MCSCTLV)
        {
            if (MCSCTLV != null)
            {
                var result = db.SecurityLevels.FirstOrDefault(x => x.Id == MCSCTLV).Name;

                return result;
            }

            return null;
        }

    }
}