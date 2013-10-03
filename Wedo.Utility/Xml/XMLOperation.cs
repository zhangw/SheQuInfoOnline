using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Wedo.Utility.Xml
{
    public class XMLOperation
    {
        /// <summary>
        /// 从文件中加载XML文档
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <returns></returns>
        public static XDocument LoadFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException("文件路径不能为空");
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException(string.Format("文件{0}不存在", filePath));
            XDocument doc = XDocument.Load(filePath);
            return doc;
        }
    }
}
