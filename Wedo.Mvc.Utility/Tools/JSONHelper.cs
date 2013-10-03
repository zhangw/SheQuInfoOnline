using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Wedo.Mvc.Utility
{
    public static class JSONHelper
    {
        /// <summary>
        /// 扩展Object方法，使其具有格式化为JSON字符的能力
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(this object obj)
        {
            if (obj == null)
                return null;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
    }
}
