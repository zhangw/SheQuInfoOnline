using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Wedo.Utility.Cache;

namespace Wedo.Utility.Mapping
{
    /// <summary>
    /// 列定义
    /// </summary>
    class Column
    {
        /// <summary>
        /// 源对象列名称
        /// </summary>
        public string SourceColName { set; get; }

        /// <summary>
        /// 目标对象列名称
        /// </summary>
        public string TargetColName { set; get; }
    }

    /// <summary>
    /// 根据XML文件提供的列对应关系，将对象1的属性赋值给对象2
    /// </summary>
    public class ObjectMapping
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public object Target { set; get; }

        /// <summary>
        /// 目标对象类型
        /// </summary>
        public Type TargetType { get { return Target.GetType(); } }

        /// <summary>
        /// 源目标对象
        /// </summary>
        public object Source { set; get; }

        /// <summary>
        /// 源目标类型
        /// </summary>
        public Type SourceType { get { return Source.GetType(); } }

        public string FilePath { get; set; }

        private string _XmlCacheName;
        /// <summary>
        /// 缓存的名称
        /// </summary>
        public string XmlCacheName
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("缓存名称不能为空");
                }
                _XmlCacheName = value;
            }
            get
            {
                return _XmlCacheName;
            }
        }

        public ObjectMapping() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="source">源对象</param>
        /// <param name="xmlCacheName">缓存的名称</param>
        public ObjectMapping(object target, object source, string xmlCacheName, string filePath)
        {
            this.Target = target;
            this.Source = source;
            this.XmlCacheName = xmlCacheName;
            this.FilePath = filePath;
        }

        /// <summary>
        /// 通过反射为将源目标的属性值赋值给目标对象的属性
        /// </summary>
        /// <param name="targetPropertyName">目标对象属性名称</param>
        /// <param name="sourceProperyName">源对象属性名称</param>
        private void SetSourceValueToTargetProperty(string targetPropertyName, string sourceProperyName)
        {
            var sourceVal = SourceType.GetProperty(sourceProperyName).GetValue(Source, null);
            TargetType.GetProperty(targetPropertyName).SetValue(Target, sourceVal, null);
        }

        /// <summary>
        /// 从XML文件中加载配置的列Mapping        
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Column> LoadColumnsMappingFromXml()
        {
            XDocument doc = XDocument.Load(FilePath);
            var query = from colItem in doc.Elements("columns").Elements()
                        select new Column
                        {
                            SourceColName = colItem.Attribute("sourcecolname").Value,
                            TargetColName = colItem.Attribute("targetcolname").Value
                        };
            var columns = query.ToList();
            //添加缓存操作
            ICacheOperator cacheOp = new FileDependencyCacheOperator() { FilePath = FilePath };
            cacheOp.Insert(XmlCacheName, columns);
            return columns;
        }

        /// <summary>
        /// 从缓存中获取配置对象
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Column> GetFromCache()
        {
            Cache.ICacheOperator op = new Cache.FileDependencyCacheOperator();
            return op.GetData(XmlCacheName) as IEnumerable<Column>;
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Column> GetColumnsMapping()
        {
            var columns = GetFromCache();
            if (columns == null)
                columns = LoadColumnsMappingFromXml();
            return columns;
        }

        /// <summary>
        /// 转换后返回目标对象
        /// </summary>
        /// <returns></returns>
        public object GetTargetObject()
        {
            foreach (var col in GetColumnsMapping())
            {
                SetSourceValueToTargetProperty(col.TargetColName, col.SourceColName);
            }
            return Target;
        }
    }
}
