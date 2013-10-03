using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Wedo.Utility.Common
{
    public class BoolSelectList : List<SelectListItem> 
    {
        public BoolSelectList(){
            List<SelectListItem> list = new List<SelectListItem> { 
                new SelectListItem { Text = "", Value = "" }, 
                new SelectListItem { Text = "是", Value = "True" },
                new SelectListItem { Text = "否", Value = "False" }
            };
            this.AddRange(list);
        }
    }
}
