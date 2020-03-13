using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.DAL.MA001
{
    public class MA001_DAL
    {
        private Models.TopProSystemEntities dc = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();

        public bool Insert(Models.MA001 model)
        {
            model.MARGSDT = DateTime.Now;
            model.MARGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
            bool check = dc.MA001.Count(x => x.MASPCD.Trim() == model.MASPCD.Trim()) > 0;
            if (check == false)
            {
                try
                {
                    dc.MA001.Add(model);

                    if (dc.SaveChanges() > 0)
                    {
                        var _model = dc.MA001.Where(x => x.MASPCD.Trim().Equals(model.MASPCD.Trim())).SingleOrDefault();
                        if (_model != null)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLogError_DAL.WriteLogErrorException(ex);
                    return false;
                }

            }
            else if (check == true)
            {
                return true;
            }

            return false;
        }
        public bool Update(Models.MA001 model)
        {
            try
            {
                var _model = dc.MA001.Where(x => x.MASPCD.Trim().Equals(model.MASPCD.Trim())).SingleOrDefault();
                if (_model != null)
                {
                    _model.MABUZCD = model.MABUZCD;
                    _model.MACLTRM = model.MACLTRM;
                    _model.MACNTRC = model.MACNTRC;
                    _model.MAIDCD = model.MAIDCD;
                    _model.MAPCALC = model.MAPCALC;
                    _model.MAPCLSD = model.MAPCLSD;
                    _model.MAPCRCD = model.MAPCRCD;
                    _model.MAPDAYS = model.MAPDAYS;
                    _model.MAPDFER = model.MAPDFER;
                    _model.MAPSETEL = model.MAPSETEL;
                    _model.MAPTDUE = model.MAPTDUE;
                    _model.MAPTXCD = model.MAPTXCD;
                    _model.MAPYTRM = model.MAPYTRM;
                    _model.MASCALC = model.MASCALC;
                    _model.MASCLSD = model.MASCLSD;
                    _model.MASCRCD = model.MASCRCD;
                    _model.MASDAYS = model.MASDAYS;
                    _model.MASDFER = model.MASDFER;
                    _model.MASPAD1 = model.MASPAD1;
                    _model.MASPAD2 = model.MASPAD2;
                    _model.MASPAD3 = model.MASPAD3;
                    _model.MASPCTG = model.MASPCTG;
                    _model.MASPFAX = model.MASPFAX;
                    _model.MASPNM = model.MASPNM;
                    _model.MASPSNM = model.MASPSNM;
                    _model.MASPTEL = model.MASPTEL;
                    _model.MASSETL = model.MASSETL;
                    _model.MASTDUE = model.MASTDUE;
                    _model.MASTXCD = model.MASTXCD;
                    _model.MAUPDT = DateTime.Now;
                    _model.MAUPDTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);

                    if (dc.SaveChanges() > 0)
                    {
                        _model = dc.MA001.Where(x => x.MASPCD.Trim().Equals(model.MASPCD.Trim())).SingleOrDefault();
                        if (_model != null)
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
        public bool Delete(string spCode)
        {
            try
            {
                var model = dc.MA001.SingleOrDefault(x => x.MASPCD.Trim().Equals(spCode));
                if (model != null)
                {
                    dc.MA001.Remove(model);
                    if (dc.SaveChanges() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogError_DAL.WriteLogErrorException(ex);
                return false;
            }

            return false;
        }
        public Models.MA001 GetListForientkey(string spcode)
        {
            Models.MA001 model = null;
            if(String.IsNullOrEmpty(spcode)) model = new Models.MA001();
            else model = dc.MA001.Single(x => x.MASPCD.Trim().Equals(spcode.Trim()));

            //model.DueDate = dc.MA012.Where(x => x.MNCLSCD.Trim().Equals(Models.ClassificationCode.CLASSIFICATTIONCODE001)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD });
            //model.currencyCode = dc.MA012.Where(x => x.MNCLSCD.Trim().Equals(Models.ClassificationCode.CLASSIFICATTIONCODE012)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD });
            //model.taxCode = dc.MA010.ToList().Select(a => new SelectListItem { Value = a.MKTXCD, Text = a.MKTXCD });
            //model.CalculationType = dc.MA012.Where(x => x.MNCLSCD.Trim().Equals(Models.ClassificationCode.CLASSIFICATTIONCODE003)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD });
            //model.DayinMonth = dc.MA012.Where(x => x.MNCLSCD.Trim().Equals(Models.ClassificationCode.CLASSIFICATTIONCODE002)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD });
            //model.CountryCode = dc.MA012.Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE010)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD });
            //model.BusinessTypeCode = dc.MA012.Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE011)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD });
            //model.Personincharge = dc.MA003.Select(x => new SelectListItem { Value = x.MCIDCD, Text = x.MCIDCD });

            model.DueDate = dc.MA012.Where(x => x.MNCLSCD.Trim().Equals(Models.ClassificationCode.CLASSIFICATTIONCODE001)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD + " - " + a.MNSRNM });
            model.currencyCode = dc.MA012.Where(x => x.MNCLSCD.Trim().Equals(Models.ClassificationCode.CLASSIFICATTIONCODE012)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD + " - " + a.MNSRNM });
            model.taxCode = dc.MA010.ToList().Select(a => new SelectListItem { Value = a.MKTXCD, Text = a.MKTXCD + " - " + a.MKTXDL });
            model.CalculationType = dc.MA012.Where(x => x.MNCLSCD.Trim().Equals(Models.ClassificationCode.CLASSIFICATTIONCODE003)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD + " - " + a.MNSRNM });
            model.DayinMonth = dc.MA012.Where(x => x.MNCLSCD.Trim().Equals(Models.ClassificationCode.CLASSIFICATTIONCODE002)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD + " - " + a.MNSRNM });
            model.CountryCode = dc.MA012.Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE010)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD + " - " + a.MNSRNM });
            model.BusinessTypeCode = dc.MA012.Where(x => x.MNCLSCD.Equals(Models.ClassificationCode.CLASSIFICATTIONCODE011)).ToList().Select(a => new SelectListItem { Value = a.MNSRCD, Text = a.MNSRCD + " - " + a.MNSRNM });
            model.Personincharge = dc.MA003.Select(x => new SelectListItem { Value = x.MCIDCD, Text = x.MCIDCD + " - " + x.MCIDNM });

            return model;
        }
        public bool CheckSpCodeExists(string spcode)
        {
            var model = dc.MA001.SingleOrDefault(x => x.MASPCD.Trim().Equals(spcode.Trim()));
            return model == null;
        }
        public int GetTotalRecord(string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return dc.MA001.Count();
            }
            else
            {
                return dc.MA001.Count(x => x.MASPCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper()));
            }
        }
        public List<Models.MA001> GetTotalDisplayRecord(string searchParam, int skip, int take)
        {
            if (String.IsNullOrEmpty(searchParam))
            {
                return dc.MA001.OrderByDescending(x => new { x.MARGSDT, x.MARGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return dc.MA001.Where(x => x.MASPCD.Trim().ToUpper().Contains(searchParam.Trim().ToUpper())).OrderByDescending(x => new { x.MARGSDT, x.MARGSTM }).Skip(skip).Take(take).ToList();
            }
        }

        public Models.MA001 GetSalePurchase(string spcode)
        {
            return dc.MA001.SingleOrDefault(x => x.MASPCD.Trim().ToUpper().Equals(spcode.Trim().ToUpper()));
        }
    }
}