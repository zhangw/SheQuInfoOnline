using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wedo.Mvc.Utility
{
    /// <summary>
    /// 搜索对象
    /// </summary>
    public class SearchModel
    {
        public SearchModel()
        {
            Keyword = "";
            Page = 1;
        }
        public string Keyword { get; set; }

        public int Page { get; set; }
    }

}