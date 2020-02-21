using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopProSystem.Extension.ModelShowAction
{
    public class BaseActionModel
    {
        public bool ShowAdd { get; set; }
        public bool ShowDelete { get; set; }
        public bool ShowEdit { get; set; }
        public BaseActionModel()
        {
            ShowAdd = false;
            ShowDelete = false;
            ShowEdit = false;
        }
    }
}