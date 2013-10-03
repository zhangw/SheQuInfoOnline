using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Caching;
using System.Web;
using System.IO;

namespace Wedo.Utility
{
    public static class ExportMap
    {
        static void LoadMap()
        {
            Dictionary<string, Dictionary<string, string>> m_ExportMap = new Dictionary<string, Dictionary<string, string>>();

            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "/ExportMap.xml");

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                m_ExportMap[node.Name] = new Dictionary<string, string>();
                foreach (XmlNode field in node.ChildNodes)
                {
                    m_ExportMap[node.Name][field.Attributes["Text"].Value] = field.Attributes["Name"].Value;
                    m_ExportMap[node.Name][field.Attributes["Name"].Value] = field.Attributes["Text"].Value;
                }
            }
            HttpRuntime.Cache.Add("ExportMap", m_ExportMap,
            new CacheDependency(AppDomain.CurrentDomain.BaseDirectory + "/ExportMap.xml"),
             System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0, 0), CacheItemPriority.NotRemovable,
                (s, o, r) =>
                {
                    if (r == CacheItemRemovedReason.DependencyChanged)
                        LoadMap();
                }
            );
        }

        public static Dictionary<string, Dictionary<string, string>> ExportMaps
        {
            get
            {
                if (System.Web.HttpRuntime.Cache["ExportMap"] == null)
                {
                    LoadMap();
                }
                return HttpRuntime.Cache["ExportMap"] as Dictionary<string, Dictionary<string, string>>;
            }
        }

        /// <summary>
        /// 批量加载文件
        /// 最终返回的内容格式如:[ [{"名称":"小弟科技"},{"合同号":"23333"}],... ]
        /// </summary>
        /// <param name="sperator">文件分隔符</param>
        /// <param name="paths">文件路径数组</param>
        /// <returns>返回格式化后的文件内容</returns>
        public static List<Dictionary<string, string>> LoadFile(char sperator, string path)
        {
            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
                return null;

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
             string[] header;
            string[] record;

            using (StreamReader reader = new StreamReader(path, Encoding.GetEncoding("gb2312")))
            {
                header = reader.ReadLine().Split(sperator);
                while (!reader.EndOfStream)
                {
                    record = reader.ReadLine().Split(sperator);
                    if (record.Length == header.Length)
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        for (int i = 0; i < header.Length; i++)
                        {
                            dict[header[i].Trim()] = record[i];
                        }
                        result.Add(dict);
                    }
                }
            }
           
            return result;
        }
    }
}
