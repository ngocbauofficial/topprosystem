using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopProSystem.Areas.MasterSetting.Models
{
    public partial class MA001
    {
        [NotMapped]
        public IEnumerable<SelectListItem> DueDate { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> CalculationType { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> DayinMonth { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> BusinessTypeCode { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> CountryCode { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> taxCode { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> currencyCode { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> Personincharge { get; set; }

    }
}