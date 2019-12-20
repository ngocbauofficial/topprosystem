using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using TopProSystem.Areas.MasterSetting.Models;

namespace TopProSystem.Areas.MasterSetting.DAL.PUR001
{
    public class PUR001_DAL
    {
        private TopProSystemEntities dc = new TopProSystemEntities();
        WriteLogError_DAL WriteLogError_DAL = new WriteLogError_DAL();
        public bool Insert(Models.PUR001 model)
        {
            try
            {
                var modelChecking = dc.PUR001.FirstOrDefault(x => x.AAPURNO.Trim().Equals(model.AAPURNO));
                if (modelChecking != null)
                {
                    if (!modelChecking.AADSTNC.Trim().Equals(model.AADSTNC.Trim())) // double click case
                    {
                        return true;
                    }
                }

                model.PURCODE = CreateGuid();
                dc.PUR001.Add(model);
                if (dc.SaveChanges() > 0)
                {
                    WriteLogError_DAL.WriteLogUserAction("PUR001", "INSERT", model.AAPURNO);
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
        public bool Delete(string key)
        {

            try
            {
                using (var scope = new TransactionScope())
                {
                    var list = dc.PUR001.Where(x => x.AADSTNC.Trim().Equals(key));
                    int count = list.Count();
                    string purno = list.First().AAPURNO;
                    foreach (var model in list)
                    {
                        dc.PUR001.Remove(model);
                    }
                    if (dc.SaveChanges() != count)
                    {
                        scope.Dispose();
                        return false;
                    }
                    else
                    {
                        WriteLogError_DAL.WriteLogUserAction("PUR001", "DELETE", purno);
                        scope.Complete();
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

        public Models.PUR001 GetPUR001s(string key)
        {
            var models = dc.PUR001.First(x => x.AADSTNC.Equals(key));
            return models;
        }
        public IEnumerable<Models.PUR001> GetAll()
        {
            return dc.PUR001.GroupBy(x => x.AADSTNC, (e, g) => g.FirstOrDefault());
        }
        public List<Models.PUR001> GetAllItemOfPurchase(string key, bool reference = false)
        {
            var models = dc.PUR001.OrderByDescending(x => x.ABRGSDT).Where(x => x.AADSTNC.Equals(key)).ToList();
            var rsModels = new List<Models.PUR001>();
            if (reference == false)
            {
                foreach (Models.PUR001 item in models)
                {
                    rsModels.Add(GetReferences(item));

                }
                return rsModels;
            }

            return models;
        }
        public int GetTotalRecord(PurchaseSearchModel searchmodel)
        {
            var model = from a in dc.PUR001 select a;
            if (!String.IsNullOrEmpty(searchmodel.PurchaseNo))
            {
                model = model.Where(x => x.AAPURNO == searchmodel.PurchaseNo);
            }
            if (searchmodel.ShippingMonth != null)
            {
                model = model.Where(x => x.AASHPDT.Equals(searchmodel.ShippingMonth));
            }
            if (!String.IsNullOrEmpty(searchmodel.SupplierContractNo))
            {
                model = model.Where(x => x.AASPPNO == searchmodel.SupplierContractNo);
            }
            if (!String.IsNullOrEmpty(searchmodel.MakerCode))
            {
                model = model.Where(x => x.AAMKCD == searchmodel.MakerCode);
            }
            if (!String.IsNullOrEmpty(searchmodel.SupplierCode))
            {
                model = model.Where(x => x.AASPLCD == searchmodel.SupplierCode);
            }

            if (!String.IsNullOrEmpty(searchmodel.CommondityCode))
            {
                model = model.Where(x => x.AACMDCD == searchmodel.CommondityCode);
            }

            if (!String.IsNullOrEmpty(searchmodel.UserCode))
            {
                model = model.Where(x => x.AAUSRCD == searchmodel.UserCode);
            }

            return model.GroupBy(x => x.AADSTNC, (e, g) => g.FirstOrDefault()).Count();
        }
        public List<Models.PUR001> GetTotalDisplayRecord(jQueryDataTablePurchase param, PurchaseSearchModel searchmodel)
        {
            var model = from a in dc.PUR001 select a;
            if (!String.IsNullOrEmpty(searchmodel.PurchaseNo))
            {
                model = model.Where(x => x.AAPURNO.Trim() == searchmodel.PurchaseNo.Trim());
            }
            if (!String.IsNullOrEmpty(searchmodel.ShippingMonth))
            {
                model = model.Where(x => x.AASHPDT.Equals(searchmodel.ShippingMonth));
            }
            if (!String.IsNullOrEmpty(searchmodel.SupplierContractNo))
            {
                model = model.Where(x => x.AASPPNO.Trim() == searchmodel.SupplierContractNo.Trim());
            }
            if (!String.IsNullOrEmpty(searchmodel.MakerCode))
            {
                model = model.Where(x => x.AAMKCD.Trim() == searchmodel.MakerCode.Trim());
            }
            if (!String.IsNullOrEmpty(searchmodel.SupplierCode))
            {
                model = model.Where(x => x.AASPLCD.Trim() == searchmodel.SupplierCode.Trim());
            }

            if (!String.IsNullOrEmpty(searchmodel.CommondityCode))
            {
                model = model.Where(x => x.AACMDCD.Trim() == searchmodel.CommondityCode.Trim());
            }

            if (!String.IsNullOrEmpty(searchmodel.UserCode))
            {
                model = model.Where(x => x.AAUSRCD.Trim() == searchmodel.UserCode.Trim());
            }
            var list = model.GroupBy(x => x.AADSTNC, (e, g) => g.FirstOrDefault()).OrderByDescending(x => new { x.ABRGSDT }).Skip(param.start).Take(param.length).ToList();

            return list;
        }
        public bool CheckCompletionStatus(string privateKey)
        {
            var allItem = dc.PUR001.AsNoTracking().Where(x => x.AADSTNC.Trim().ToUpper().Equals(privateKey.Trim().ToUpper()));
            return allItem.All(x => x.CompletetionStatus != 1); // false neu co 1 item = 1
        }
        private Guid CreateGuid()
        {
            Guid guid = Guid.NewGuid();
            return guid;
        }
        public bool CheckCodeCreateExists(string code)
        {
            return dc.PUR001.FirstOrDefault(x => x.AADSTNC.Trim().Equals(code)) == null;
        }
        public bool CheckSpNoExists(string a)
        {
            return dc.PUR001.FirstOrDefault(x => x.AAPURNO.Trim().Equals(a.Trim())) == null;
        }
        public string CreatePurchaseContractNo()
        {
            var today = DateTime.Now;
            string rs_code = string.Concat(today.ToString("yy"), today.ToString("MM"));
            string purchr_ctract_no_number, purchr_ctract_no_character;
            var model_nearest = dc.PUR001.OrderByDescending(x => x.AAPURNO).FirstOrDefault();
            if (model_nearest == null)
            {
                rs_code = string.Concat(rs_code, "0001");
            }
            else
            {
                purchr_ctract_no_character = model_nearest.AAPURNO.Substring(0, 4);

                if (purchr_ctract_no_character.Equals(rs_code))  // hop dong trong thang
                {
                    purchr_ctract_no_number = model_nearest.AAPURNO.Substring(4, 4);
                    int next_number = int.Parse(purchr_ctract_no_number) + 1;
                    if (next_number.ToString().Length == 1)
                    {
                        purchr_ctract_no_number = string.Concat("000", next_number);
                    }
                    else if (next_number.ToString().Length == 2)
                    {
                        purchr_ctract_no_number = string.Concat("00", next_number);
                    }
                    else if (next_number.ToString().Length == 3)
                    {
                        purchr_ctract_no_number = string.Concat("0", next_number);
                    }
                    else
                    {
                        purchr_ctract_no_number = string.Concat("", next_number);
                    }

                    rs_code = string.Concat(rs_code, purchr_ctract_no_number);
                }
                else // chua co hop dong trong thang
                {
                    rs_code = string.Concat(rs_code, "0001");
                }

            }

            return rs_code;
        }
        public Models.PUR001 GetReferences(Models.PUR001 _model)
        {
            if (_model == null) _model = new Models.PUR001();


            //_model.Grades = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE015)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.MakerCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE005)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.CurrencyCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE012)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.Priceterms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE019)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();    //pnb edit
            //_model.TypeOfTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE021)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();    //pnb edit
            //_model.CommodityCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE006)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.ContractTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE025)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.SettelementTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE020)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList(); //pnb edit
            //_model.ExchangeRateType = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE018)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //_model.RawMaterialTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE034)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRNM }).ToList();
            //_model.TaxCodes = dc.MA010.OrderByDescending(x => x.MKRGSDT).ToList().ConvertAll(x =>
            //{
            //    return new SelectListItem()
            //    {
            //        Value = x.MKTXCD,
            //        Text = x.MKTXCD.ToString()
            //    };
            //});

            //_model.ExchangeRates = dc.MA009.OrderByDescending(x => x.MJRGSDT).ToList().ConvertAll(x =>
            //{
            //    return new SelectListItem()
            //    {
            //        Value = x.MJCRRCD,
            //        Text = x.MJEXRTT
            //    };
            //});
            //_model.Specs = dc.MA006.OrderByDescending(x => x.MFRGSDT).ToList().ConvertAll(x =>
            //{
            //    return new SelectListItem()
            //    {
            //        Value = x.MFPRDSP,
            //        Text = x.MFPRDSP
            //    };
            //});

            //_model.Coatings = dc.MA005.OrderByDescending(x => x.MERGSDT).Select(x => new SelectListItem { Value = x.MECOAT, Text = x.MECOAT });
            //_model.UserCodes = dc.MA002.Select(x => new SelectListItem { Value = x.MBUSRCD, Text = x.MBUSRCD }).ToList();

            //_model.SteelGrades = dc.SteelGrades.Select(x => new SelectListItem { Value = x.Grade, Text = x.Grade });
            //_model.PersonIncharges = dc.MA003.OrderByDescending(x => x.MCRGSDT).Select(x => new SelectListItem { Value = x.MCIDCD, Text = x.MCIDCD });
            //_model.SupplierCodes = dc.MA001.OrderByDescending(x => x.MARGSDT).Where(x => x.MASPCTG.Trim().Equals("P")).Select(x => new SelectListItem { Value = x.MASPCD, Text = x.MASPCD });
            //_model.DeliveryConditionCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE024)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD }).ToList();
            //return _model;

            /**********bao edit*************/

            // _model.Grades = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE015)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.MakerCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE005)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.CurrencyCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE012)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.Priceterms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE019)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();    //pnb edit
            //_model.TypeOfTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE021)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();    //pnb edit
            // _model.CommodityCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE006)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.ContractTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE025)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            // _model.SettelementTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE020)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList(); //pnb edit
            // _model.RawMaterialTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE034)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();
            //_model.ExchangeRateType = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE018)).Select(x => new SelectListItem { Value = x.MNSRCD, Text = x.MNSRCD + "  -  " + x.MNSRNM + "  " }).ToList();


            //_model.TaxCodes = dc.MA010.OrderByDescending(x => x.MKRGSDT).ToList().ConvertAll(x =>
            //{
            //    return new SelectListItem()
            //    {
            //        Value = x.MKTXCD,
            //        Text = x.MKTXCD.ToString()
            //    };
            //});

            _model.ExchangeRates = dc.MA009.OrderByDescending(x => x.MJRGSDT).ToList().ConvertAll(x =>
            {
                return new SelectListItem()
                {
                    Value = x.MJCRRCD,
                    Text = Areas.MasterSetting.Controllers.MasterController.GetSRNameBySRCode(ClassificationCode.CLASSIFICATTIONCODE018, x.MJEXRTT)
                };
            });
            //  _model.Specs = dc.MA006.OrderByDescending(x => x.MFRGSDT).ToList().ConvertAll(x =>
            //    {
            //      return new SelectListItem()
            //      {
            //          Value = x.MFPRDSP,
            //         Text = x.MFPRDSP
            //     };
            //  });

            //  _model.Coatings = dc.MA005.OrderByDescending(x => x.MERGSDT).Select(x => new SelectListItem { Value = x.MECOAT, Text = x.MECOAT });
            // _model.UserCodes = dc.MA002.Select(x => new SelectListItem { Value = x.MBUSRCD, Text = x.MBUSRCD + "  -  " + x.MBUSRNM }).ToList();

            //  _model.SteelGrades = dc.SteelGrades.Select(x => new SelectListItem { Value = x.Grade, Text = x.Grade });
            // _model.PersonIncharges = dc.MA003.OrderByDescending(x => x.MCRGSDT).Select(x => new SelectListItem { Value = x.MCIDCD, Text = x.MCIDCD + "  -  " + x.MCIDNM });
            //  _model.SupplierCodes = dc.MA001.OrderByDescending(x => x.MARGSDT).Where(x => x.MASPCTG.Trim().Equals("P")).Select(x => new SelectListItem { Value = x.MASPCD, Text = x.MASPCD + "  -  " + x.MASPNM });
            //   _model.DeliveryConditionCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE024)).Select(x => new SelectListItem { Value = x.MNSRCD.Trim(), Text = x.MNSRCD + "  -  " + x.MNSRNM }).ToList();
            /*sugar edit for the fucking order like shit */

            _model.Grades = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE015)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.MakerCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE005)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.CurrencyCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE012)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.Priceterms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE019)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.TypeOfTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE021)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.CommodityCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE006)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.ContractTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE025)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.SettelementTerms = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE020)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.RawMaterialTypes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE034)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());
            _model.ExchangeRateType = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE018)).ToDictionary(x => x.MNSRCD, x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());

            _model.TaxCodes = dc.MA010.OrderByDescending(x => x.MKRGSDT).ToDictionary(x => x.MKTXCD.Trim(), x => x.MKTXCD.Trim() + " - " + x.MKTXDL.Trim());

            _model.Specs = dc.MA006.OrderByDescending(x => x.MFRGSDT).ToDictionary(x => x.MFPRDSP, x => x.MFPRDSP);
            _model.Coatings = dc.MA005.OrderByDescending(x => x.MERGSDT).ToDictionary(x => x.MECOAT, x => x.MECOAT);
            _model.UserCodes = dc.MA002.ToDictionary(x => x.MBUSRCD, x => x.MBUSRCD.Trim() + "-" + x.MBUSRNM);

            _model.SteelGrades = dc.SteelGrades.ToDictionary(x => x.Grade, x => x.Grade); //PNBEDIT
            _model.PersonIncharges = dc.MA003.OrderByDescending(x => x.MCRGSDT).ToDictionary(x => x.MCIDCD, x => x.MCIDCD.Trim() + "  -  " + x.MCIDNM);
            _model.SupplierCodes = dc.MA001.OrderByDescending(x => x.MARGSDT).Where(x => x.MASPCTG.Trim().Equals("P")).ToDictionary(x => x.MASPCD, x => x.MASPCD.Trim() + "-" + x.MASPNM.Trim());
            _model.DeliveryConditionCodes = dc.MA012.OrderByDescending(x => x.MNRGSDT).Where(x => x.MNCLSCD.Equals(ClassificationCode.CLASSIFICATTIONCODE024)).ToDictionary(x => x.MNSRCD.Trim(), x => x.MNSRCD.Trim() + " - " + x.MNSRNM.Trim());

            return _model;
        }
        public int GetTotalRecord(Dictionary<string, string> args, bool completed = false)
        {
            if (args.All(x => x.Value == ""))
            {
                if (completed == false)
                {
                    return dc.PUR001.Count();
                }
                else
                {
                    return dc.PUR001.Where(x => x.CompletetionStatus != 1).Count();
                }
            }

            List<Models.PUR001> a;
            if (completed == false)
            {
                a = dc.PUR001.Select(x => x).ToList();
            }
            else
            {
                a = dc.PUR001.Where(x => x.CompletetionStatus != 1).Select(x => x).ToList();
            }

            if (!string.IsNullOrEmpty(args["AACMPCD"]))
            {
                a = a.Where(x => x.AACMPCD.Trim().Equals(args["AACMPCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AAMKCD"]))
            {
                a = a.Where(x => x.AAMKCD.Trim().Equals(args["AAMKCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AASPPNO"]))
            {
                a = a.Where(x => x.AASPPNO.Trim().Equals(args["AASPPNO"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AASPLCD"]))
            {
                a = a.Where(x => x.AASPLCD.Trim().Equals(args["AASPLCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AAUSRCD"]))
            {
                a = a.Where(x => x.AAUSRCD.Trim().Equals(args["AAUSRCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AAPURNO"]))
            {
                a = a.Where(x => x.AAPURNO.Trim().Equals(args["AAPURNO"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AASHPDT"]))
            {
                a = a.Where(x => x.AASHPDT.Equals(args["AASHPDT"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AACMDCD"]))
            {
                a = a.Where(x => x.AACMDCD.Trim().Equals(args["AACMDCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AACTRTP"]))
            {
                a = a.Where(x => x.AACTRTP.Trim().Equals(args["AACTRTP"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AAIDCD"]))
            {
                a = a.Where(x => x.AAIDCD.Trim().Equals(args["AAIDCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AARMTP"]))
            {
                a = a.Where(x => x.AARMTP.Trim().Equals(args["AARMTP"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["RAPSTLGR"]))
            {
                a = a.Where(x => x.RAPSTLGR.Trim().Equals(args["RAPSTLGR"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABMCSPC"]))
            {
                a = a.Where(x => x.ABMCSPC.Trim().Equals(args["ABMCSPC"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABPRDNM"]))
            {
                a = a.Where(x => x.ABPRDNM != null && x.ABPRDNM.Equals(args["ABPRDNM"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABPRDDIA"]))
            {
                a = a.Where(x => x.ABPRDDIA != null && x.ABPRDDIA.Equals(args["ABPRDDIA"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABBSZT"]))
            {
                a = a.Where(x => x.ABBSZT == double.Parse(args["ABBSZT"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABBSZW"]))
            {
                a = a.Where(x => x.ABBSZW == double.Parse(args["ABBSZW"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABBSZL"]))
            {
                a = a.Where(x => x.ABBSZL == double.Parse(args["ABBSZL"].Trim())).ToList();
            }

            return a.Count;
        }
        public IEnumerable<Models.PUR001> GetTotalDisplayRecord(Dictionary<string, string> args, int iDisplayStart, int iDisplayLength, bool paging = true, bool completed = false)
        {
            IEnumerable<Models.PUR001> ieRs;

            if (args.All(x => x.Value == ""))
            {
                if (completed == false)
                {
                    if (!paging)
                    {
                        ieRs = dc.PUR001.OrderByDescending(x => x.ABRGSDT);
                    }
                    else
                    {
                        ieRs = dc.PUR001.OrderByDescending(x => x.ABRGSDT).Skip(iDisplayStart).Take(iDisplayLength);
                    }
                }
                else
                {
                    if (!paging)
                    {
                        ieRs = dc.PUR001.Where(x => x.CompletetionStatus != 1).OrderByDescending(x => x.ABRGSDT);
                    }
                    else
                    {
                        ieRs = dc.PUR001.Where(x => x.CompletetionStatus != 1).OrderByDescending(x => x.ABRGSDT).Skip(iDisplayStart).Take(iDisplayLength);
                    }
                }

                return ieRs;
            }

            List<Models.PUR001> a;

            if (completed == false)
            {
                a = dc.PUR001.Select(x => x).ToList();
            }
            else
            {
                a = dc.PUR001.Where(x => x.CompletetionStatus != 1).Select(x => x).ToList();
            }

            if (!string.IsNullOrEmpty(args["AACMPCD"]))
            {
                a = a.Where(x => x.AACMPCD.Trim().Equals(args["AACMPCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AAMKCD"]))
            {
                a = a.Where(x => x.AAMKCD.Trim().Equals(args["AAMKCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AASPPNO"]))
            {
                a = a.Where(x => x.AASPPNO.Trim().Equals(args["AASPPNO"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AASPLCD"]))
            {
                a = a.Where(x => x.AASPLCD.Trim().Equals(args["AASPLCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AAUSRCD"]))
            {
                a = a.Where(x => x.AAUSRCD.Trim().Equals(args["AAUSRCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AAPURNO"]))
            {
                a = a.Where(x => x.AAPURNO.Trim().Equals(args["AAPURNO"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AASHPDT"]))
            {
                a = a.Where(x => x.AASHPDT.Equals(args["AASHPDT"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AACMDCD"]))
            {
                a = a.Where(x => x.AACMDCD.Trim().Equals(args["AACMDCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AACTRTP"]))
            {
                a = a.Where(x => x.AACTRTP.Trim().Equals(args["AACTRTP"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AAIDCD"]))
            {
                a = a.Where(x => x.AAIDCD.Trim().Equals(args["AAIDCD"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["AARMTP"]))
            {
                a = a.Where(x => x.AARMTP.Trim().Equals(args["AARMTP"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["RAPSTLGR"]))
            {
                a = a.Where(x => x.RAPSTLGR.Trim().Equals(args["RAPSTLGR"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABMCSPC"]))
            {
                a = a.Where(x => x.ABMCSPC.Trim().Equals(args["ABMCSPC"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABPRDNM"]))
            {
                a = a.Where(x => x.ABPRDNM != null && x.ABPRDNM.Equals(args["ABPRDNM"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABPRDDIA"]))
            {
                a = a.Where(x => x.ABPRDDIA != null && x.ABPRDDIA.Equals(args["ABPRDDIA"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABBSZT"]))
            {
                a = a.Where(x => x.ABBSZT == double.Parse(args["ABBSZT"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABBSZW"]))
            {
                a = a.Where(x => x.ABBSZW == double.Parse(args["ABBSZW"].Trim())).ToList();
            }
            if (!string.IsNullOrEmpty(args["ABBSZL"]))
            {
                a = a.Where(x => x.ABBSZL == double.Parse(args["ABBSZL"].Trim())).ToList();
            }
            if (!paging)
            {
                ieRs = a.OrderByDescending(x => x.ABRGSDT);
            }
            else
            {
                ieRs = a.OrderByDescending(x => x.ABRGSDT).Skip(iDisplayStart).Take(iDisplayLength);
            }

            return ieRs;
        }
        public bool ChangeCompletionStatusOfItem(string itemPrivateKey)
        {
            byte CloseStatus = 1;
            Guid guid = Guid.Parse(itemPrivateKey);
            var item = dc.PUR001.Where(x => x.PURCODE.Value.Equals(guid)).SingleOrDefault();
            if (item != null)
            {
                if (item.CompletetionStatus.Equals(CloseStatus)) return true;

                item.CompletetionStatus = CloseStatus;
                if (dc.SaveChanges() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public List<Models.PUR001> GetPurchaseOrderFromTo(string purchaseFr, string purchaseTo, bool takeFirst = true)
        {
            List<Models.PUR001> rsList = new List<Models.PUR001>();

            int from = int.Parse(purchaseFr), to = int.Parse(purchaseTo);
            for (int i = from; i <= to; i++)
            {
                string key = i.ToString();
                if (takeFirst)
                {
                    var model = dc.PUR001.Where(x => x.AAPURNO.Equals(key)).FirstOrDefault();
                    if (model != null)
                    {
                        rsList.Add(model);
                    }
                }
                else
                {
                    var model = dc.PUR001.Where(x => x.AAPURNO.Equals(key));
                    if (model != null)
                    {
                        rsList.AddRange(model);
                    }
                }
            }
            return rsList;
        }

        public string GetExchangeRatetypeCodeByExchangeRate(double ex) // su dung cho hien thi luc update
        {
            var model = dc.MA009.Where(x => x.MJEXRT.Value - ex == 0).FirstOrDefault();
            if (model != null)
            {
                return model.MJEXRTT;
            }

            return string.Empty;
        }


        #region methodbypnb
        public Models.PUR001 GetPUR001sByPurchaseContractNoAndItemNo(string PurchaseContractNo, int? itemno)
        {
            var models = dc.PUR001.FirstOrDefault(x => x.AAPURNO == PurchaseContractNo && x.ABCTITM == itemno);
            if (models != null)
                return models;
            return null;
        }

        #endregion

    }
}