using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wedo.Utility.Xml;
using System.Xml.Linq;
using Wedo.Utility.Cache;
using System.IO;

namespace Wedo.Utility.File
{
    /// <summary>
    /// 针对文件的一些扩展操作
    /// </summary>
    public class FileExtend
    {
        public string FilePath { set; get; }

        public string FileName { set; get; }

        public string FileExtension { set; get; }

        public string ContentType { set; get; }

        public static readonly Dictionary<string, string> ContentTypes = GetContentTypes();

        public static Dictionary<string, string> GetContentTypes()
        {
            var item = LoadFromCache();
            if (item == null)
                item = LoadContentTypesFromXML();
            return item;
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <returns></returns>
        public string GetFileName()
        {
            if (!string.IsNullOrEmpty(FilePath) && System.IO.File.Exists(FilePath))
                return new FileInfo(FilePath).Name;
            if (!string.IsNullOrEmpty(FileName))
                return FileName;
            return string.Empty;
        }

        /// <summary>
        /// 获取ContentType
        /// </summary>
        /// <returns></returns>
        public string GetContentType()
        {
            string extension = GetFileExtension().ToUpper();
            if (!string.IsNullOrEmpty(extension) && ContentTypes.ContainsKey(extension))
                return ContentTypes.FirstOrDefault(s => s.Key == extension).Value;
            return ContentTypes["OTHER"];
        }

        /// <summary>
        ///  获取文件的扩展名
        /// </summary>
        /// <returns>如xml,txt,jpeg</returns>
        public string GetFileExtension()
        {
            if (string.IsNullOrEmpty(FilePath) && string.IsNullOrEmpty(FileName))
                throw new ArgumentException("请至少指定FileName 和 FilePath两个参数中一个值");

            if (!string.IsNullOrEmpty(FilePath) && System.IO.File.Exists(FilePath))
            {
                var fItem = new System.IO.FileInfo(FilePath);
                return fItem.Extension.Substring(1);
            }
            else if (!string.IsNullOrEmpty(FileName))
            {
                string ext = null;
                if (FileName.LastIndexOf('.') > 0)
                {
                    string[] fs = FileName.Split('.');
                    ext = fs[fs.Length - 1];
                }
                return ext;
            }
            return string.Empty;
        }

        #region 从缓存或文件中加载配置项
        private static Dictionary<string, string> LoadFromCache()
        {
            ICacheOperator cacheOp = new FileDependencyCacheOperator();
            return cacheOp.GetData("WEDO_UITNITY_CONTENTTYPE") as Dictionary<string, string>;
        }

        private static Dictionary<string, string> LoadContentTypesFromXML()
        {
            string fileDcc = AppDomain.CurrentDomain.RelativeSearchPath;
            fileDcc = string.IsNullOrEmpty(fileDcc) ? AppDomain.CurrentDomain.BaseDirectory : fileDcc;
            string filePath = System.IO.Path.Combine(fileDcc,"File", "ContentType.xml");

            filePath = new System.IO.FileInfo(filePath).FullName;

            XDocument doc = XMLOperation.LoadFromFile(filePath);
            var contentItems = from item in doc.Elements("ContentTypes").Elements()
                               select new
                               {
                                   Extension = item.Value,
                                   Content = item.Attribute("value").Value
                               };
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var item in contentItems)
            {
                dic[item.Extension] = item.Content;
            }

            //添加缓存操作
            ICacheOperator cacheOp = new FileDependencyCacheOperator() { FilePath = filePath };
            cacheOp.Insert("WEDO_UITNITY_CONTENTTYPE", dic);
            return dic;
        }
        #endregion
    }
}
