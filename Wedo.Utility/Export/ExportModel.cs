using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wedo.Utility.Reflection;
using System.Reflection;
using System.Xml.Linq;
using Wedo.Utility.Cache;
using System.IO;

namespace Wedo.Utility.Export
{
    /// <summary>
    /// 对象配置文件中的单个对象
    /// </summary>
    public class ExportMapping
    {
        /// <summary>
        /// 对象名称，如ThermoFisher.MDM.Domain.Model.KeyAccount
        /// </summary>
        public string ObjectName { set; get; }

        /// <summary>
        /// 编码 utf8
        /// </summary>
        public Encoding Encoding { set; get; }

        /// <summary>
        /// 对象下的所有属性
        /// </summary>
        public List<ExportModel> Properties { set; get; }
    }
    /// <summary>
    /// 导出实体的设置
    /// </summary>
    public class ExportModel
    {
        /// <summary>
        /// 导出列的名称
        /// </summary>
        public string ExportColName { set; get; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { set; get; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int OrderNum { set; get; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { set; get; }

        /// <summary>
        /// 原字段长度
        /// </summary>
        public int SourceLength { set; get; }

        /// <summary>
        /// 生成文件中的长度
        /// </summary>
        public int Length { set; get; }

        /// <summary>
        /// 从前面开始Substring,否则从后面
        /// </summary>
        public bool FrontSub { set; get; }

        /// <summary>
        /// 需要处理，如果为false,则表示在循环处理时可以忽略
        /// </summary>
        public bool NeedProcess { set; get; }
    }

    public class ExportHelper<T> where T : class
    {
        public readonly static string CacheName = "Wedo.Utility.File.ExportHelper" + typeof(T).Name;

        public List<Dictionary<ExportModel, string>> list = null;
        protected List<PropertyInfo> _properties = null;
        protected List<ExportModel> _cols = null;

        public ExportHelper(List<ExportModel> cols)
        {
            _properties = PropertyCache.Instance[typeof(T)].Values.ToList();
            list = new List<Dictionary<ExportModel, string>>();
            _cols = cols;
        }

        public ExportHelper()
        {
            _properties = PropertyCache.Instance[typeof(T)].Values.ToList();
            list = new List<Dictionary<ExportModel, string>>();
            _cols = ObjectMappings.FirstOrDefault(s => s.ObjectName == typeof(T).Name).Properties;
        }

        /// <summary>
        /// 配置项列表
        /// </summary>
        public List<ExportMapping> ObjectMappings
        {
            get
            {
                return GetColumnsMapping(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "File", "ExportMapping.xml")).ToList<ExportMapping>();
            }
        }

        #region cache
        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ExportMapping> GetColumnsMapping(string filepath)
        {
            var columns = GetFromCache();
            if (columns == null)
                columns = LoadObjectMappingFromXml(filepath);
            return columns;
        }

        /// <summary>
        /// 从XML文件中加载配置的列Mapping        
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ExportMapping> LoadObjectMappingFromXml(string mappingPath)
        {
            XDocument doc = XDocument.Load(mappingPath);
            var query = from colItem in doc.Elements("Objects").Elements()
                        select new ExportMapping
                        {
                            ObjectName = colItem.Element("name").Value,
                            Encoding = System.Text.Encoding.GetEncoding(colItem.Element("encoding").Value),
                            Properties = (from pitem in colItem.Element("params").Elements()
                                          select new ExportModel
                                          {
                                              FrontSub = pitem.Attribute("FrontSub") == null ? true : Convert.ToBoolean(pitem.Attribute("FrontSub").Value),
                                              SourceLength = pitem.Attribute("SourceLength") == null ? Convert.ToInt32(pitem.Attribute("Length").Value) : Convert.ToInt32(pitem.Attribute("SourceLength").Value),
                                              PropertyName = pitem.Attribute("PropertyName").Value,
                                              ExportColName = pitem.Attribute("ExportColName").Value,
                                              DefaultValue = pitem.Attribute("DefaultValue").Value,
                                              Length = Convert.ToInt32(pitem.Attribute("Length").Value),
                                              OrderNum = Convert.ToInt32(pitem.Attribute("OrderNum").Value),
                                              NeedProcess = Convert.ToBoolean(pitem.Attribute("NeedProcess").Value),
                                          }).ToList<ExportModel>()
                        };
            var columns = query.ToList();
            //添加缓存操作
            ICacheOperator cacheOp = new FileDependencyCacheOperator() { FilePath = mappingPath };
            cacheOp.Insert(CacheName, columns);
            return columns;
        }

        /// <summary>
        /// 从缓存中获取配置对象
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ExportMapping> GetFromCache()
        {
            Cache.ICacheOperator op = new Cache.FileDependencyCacheOperator();
            return op.GetData(CacheName) as IEnumerable<ExportMapping>;
        }
        #endregion

        /// <summary>
        /// 解析对象的属性值到列表中
        /// </summary>
        /// <param name="obj">要解析的对象</param>
        /// <param name="property">要解析对象的属性</param>
        /// <param name="col">配置项</param>
        /// <param name="values">保存值的字典</param>
        private void AddColValue(T obj, PropertyInfo property, ExportModel col, Dictionary<ExportModel, string> values)
        {
            string val = null;

            //不需要处理的数据直接赋为默认值,否则通过反射取值
            if (!col.NeedProcess)
            {
                val = col.DefaultValue;
            }
            else
            {
                if (property == null)
                {
                    val = col.DefaultValue;
                }
                else
                {
                    val = property.GetValue(obj, null) == null ? string.Empty : property.GetValue(obj, null).ToString();
                }
            }
            val = val.Replace("\t", " ");
            if (val == null)
                val = " ";
            val = GetSubValue(col, val);
            values.Add(col, val);
        }

        /// <summary>
        /// 获取部分字符串,当配置项中没有指定长度的时候，不需要截取字符值
        /// </summary>
        /// <param name="col">配置项</param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetSubValue(ExportModel col, string value)
        {
            if (col.Length == 0 || col.SourceLength == 0 || string.IsNullOrWhiteSpace(value))
                return value;

            if (col.FrontSub)
                return value.Substring(0, value.Length <= col.SourceLength ? value.Length : col.SourceLength);

            return value.Substring(value.Length - col.SourceLength, col.SourceLength);
        }

        /// <summary>
        ///  此方法可供重写,可以将之前添加的值更新或做其他的操作
        /// </summary>
        /// <param name="val">对象对应的属性值</param>
        /// <param name="col">配置项</param>
        /// <param name="values">值字典表</param>
        protected virtual void BeforeToString(List<T> values)
        {
        }

        /// <summary>
        /// 解析单个对象到字典表中
        /// </summary>
        /// <param name="obj">要解析的对象</param>
        private void AddSingleObject(T obj)
        {
            Dictionary<ExportModel, string> values = new Dictionary<ExportModel, string>();
            foreach (var col in _cols)
            {
                var property = _properties.FirstOrDefault(s => s.Name == col.PropertyName);
                AddColValue(obj, property, col, values);
            }
            list.Add(values);
        }

        /// <summary>
        /// 解析一个对象列表
        /// </summary>
        /// <param name="objList">要解析的对象列表</param>
        private void AddList(List<T> objList)
        {
            foreach (var item in objList)
            {
                AddSingleObject(item);
            }
            BeforeToString(objList);
        }

        /// <summary>
        /// 返回对象列表的字符串
        /// </summary>
        /// <param name="values">对象列表</param>
        /// <param name="needHeader">是否需要头部，默认不需要</param>
        /// <returns>返回生成的字符串</returns>
        public string GetString(List<T> values, string seperator = "\t", bool needHeader = false)
        {
            AddList(values);
            StringBuilder buil = new StringBuilder();
            if (needHeader)
            {
                string header = string.Join(seperator, list.FirstOrDefault()
                    .Keys.OrderBy(s => s.OrderNum)
                    .Select(s => s.ExportColName)
                    .ToList());

                buil.AppendLine(header);
            }
            foreach (var item in list)
            {
                buil.AppendLine(string.Join(seperator, item.Select(s => s.Value).ToList()));
            }

            return buil.ToString();
        }

        /// <summary>
        /// 将内容写到文件中s
        /// </summary>
        /// <param name="values"></param>
        /// <param name="filepath"></param>
        /// <param name="seperator"></param>
        /// <param name="needHeader"></param>
        public void WriteFile(List<T> values, string filepath, string seperator = "\t", bool needHeader = false)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException("文件路径不能为空");

            if (System.IO.File.Exists(filepath))
                System.IO.File.Delete(filepath);

            using (StreamWriter writer = new StreamWriter(filepath, false, Encoding.UTF8))
            {
                writer.Write(GetString(values, seperator: seperator, needHeader: needHeader));
            }
        }

    }
}
