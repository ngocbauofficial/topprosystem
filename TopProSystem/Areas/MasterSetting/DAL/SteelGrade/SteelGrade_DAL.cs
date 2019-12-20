using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Areas.MasterSetting.DAL.SteelGrade
{
    public class SteelGrade_DAL
    {
        private Models.TopProSystemEntities db = new Models.TopProSystemEntities();
        private WriteLogError_DAL WriteLogError = new WriteLogError_DAL();

        public Models.SteelGrade GetSteelGradeByCode(string grade)
        {
            var model = db.SteelGrades.First(x => x.Grade == grade);
            return model;
        }

        public bool Insert(Models.SteelGrade model)
        {
            bool check = db.SteelGrades.Count(x => x.Grade.Trim() == model.Grade.Trim()) > 0;
            if (check == false)
            {
                try
                {
                    model.RGSDT = DateTime.Now;
                    model.RGSTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    db.SteelGrades.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        if (db.SteelGrades.SingleOrDefault(x => x.Grade.Trim().Equals(model.Grade.Trim())) != null)
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

        public bool Update(Models.SteelGrade model)
        {
            try
            {

                var _model = db.SteelGrades.Single(x => x.Grade.Trim().Equals(model.Grade.Trim()));
                _model.SAEsymbol = model.SAEsymbol;

                _model.C = model.C;
                _model.Mn = model.Mn;
                _model.P = model.P;
                _model.S = model.S;
                _model.Si = model.Si;
                _model.Al = model.Al;
                _model.UPDT = DateTime.Now;
                _model.UPTM = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
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
                var model = db.SteelGrades.Single(x => x.Grade.Trim().Equals(code.Trim()));
                db.SteelGrades.Remove(model);
                if (db.SaveChanges() > 0)
                {
                    if (db.SteelGrades.SingleOrDefault(x => x.Grade.Trim().Equals(code.Trim())) == null)
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
                return db.SteelGrades.Count();
            }
            else
            {
                return db.SteelGrades.Where(x => x.Grade.Contains(searchParam)).Count();
            }
        }

        public List<Models.SteelGrade> GetTotalDisplayRecord(int skip, int take, string searchParam)
        {
            if (String.IsNullOrEmpty(searchParam) || String.IsNullOrWhiteSpace(searchParam))
            {
                return db.SteelGrades.OrderByDescending(x => new { x.RGSDT, x.RGSTM }).Skip(skip).Take(take).ToList();
            }
            else
            {
                return db.SteelGrades.OrderByDescending(x => new { x.RGSDT, x.RGSTM }).Where(x => x.Grade.Trim().Contains(searchParam)).Skip(skip).Take(take).ToList();

            }
        }
        public bool CheckSteelGradeCodeExists(string code)
        {
            var model = db.SteelGrades.SingleOrDefault(x => x.Grade.Trim().Equals(code.Trim()));
            return model == null;
        }
    

    }
}