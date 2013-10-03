using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wedo.Mvc.Utility
{
    public class ResultMSG
    {
        public ResultMSG( )
        {
            
        }
        public ResultMSG(string msg)
        {
            MSG = msg;
        }

        public ResultMSG(string msg,bool result)
        {
            MSG = msg;
            Result = result;
        }

        public virtual string ToJson()
        {
            return "{Result:\'" + Result + "\',MSG:\'" + MSG + "\'}";
        }

        /// <summary>
        /// 结果，是否成功
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        public string MSG { get; set; }
    }
}
