using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wedo.Utility.Cache;
using System.Xml.Linq;
using System.Data;
using System.Data.OleDb;

namespace Wedo.Utility.File
{
    /// <summary>
    /// 从文件中生成对象列表
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public interface IObjectFromFile<T>
    {
        /// <summary>
        /// 处理成功
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        List<string> ErrMsgs { get; }

        /// <summary>
        /// 将文件中的记录创建为对象，并返回全部的对象列表
        /// </summary>
        /// <param name="name">对象的类型名称</param>
        /// <param name="filepath">需要处理的文件路径</param>
        /// <returns>从文件中生成的对象列表</returns>
        IEnumerable<T> GenerateObjectsFromFile(string name, string filepath);

        /// <summary>
        /// 返回经过过滤重复数据的对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filepath"></param>
        /// <param name="isDistinct">是否过滤重复数据，true 是，false 否</param>
        /// <returns>经过过滤重复数据的对象</returns>
        IEnumerable<T> GenerateObjectsFromFile(string name, string filepath, bool isDistinct);
    }

    /// <summary>
    /// 基类，实现一些基本方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectFromFile<T> : IObjectFromFile<T>
    {
        public const string CacheName = "Wedo.Utility.File.ObjectMapping";

        private List<string> _ErrMsgs = new List<string>();

        /// <summary>
        /// 操作结果成功
        /// </summary>
        public bool Success
        {
            get
            {
                return _ErrMsgs != null && _ErrMsgs.Count > 0;

            }
        }

        /// <summary>
        /// 错误信息列表
        /// </summary>
        public List<string> ErrMsgs { get { return _ErrMsgs; } }

        /// <summary>
        /// 配置项列表
        /// </summary>
        public List<ObjectMapping> ObjectMappings
        {
            get { return GetColumnsMapping(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "File", "ObjectMapping.xml")).ToList<ObjectMapping>(); }
        }

        /// <summary>
        /// 从XML文件中加载配置的列Mapping        
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ObjectMapping> LoadObjectMappingFromXml(string mappingPath)
        {
            XDocument doc = XDocument.Load(mappingPath);
            var query = from colItem in doc.Elements("Objects").Elements()

                        select new ObjectMapping
                        {
                            ObjectName = colItem.Element("name").Value,
                            Encoding = System.Text.Encoding.GetEncoding(colItem.Element("encoding").Value),
                            Splitter = colItem.Element("splitter").Value,
                            Properties = (from pitem in colItem.Element("params").Elements()
                                          select new ObjectMappingItem
                                          {
                                              FileTitle = pitem.Value,
                                              PropertyName = pitem.Attribute("name").Value,
                                              PropertyType = pitem.Attribute("type").Value
                                          }).ToList<ObjectMappingItem>()
                        };
            var columns = query.ToList();

            //如果使用t作为分隔符,则认为是tab键
            columns.ForEach(s =>
            {
                if (s.Splitter == "t")
                    s.Splitter = "\t";
            });

            //添加缓存操作
            ICacheOperator cacheOp = new FileDependencyCacheOperator() { FilePath = mappingPath };
            cacheOp.Insert(CacheName, columns);
            return columns;
        }

        /// <summary>
        /// 从缓存中获取配置对象
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ObjectMapping> GetFromCache()
        {
            Cache.ICacheOperator op = new Cache.FileDependencyCacheOperator();
            return op.GetData(CacheName) as IEnumerable<ObjectMapping>;
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ObjectMapping> GetColumnsMapping(string filepath)
        {
            var columns = GetFromCache();
            if (columns == null)
                columns = LoadObjectMappingFromXml(filepath);
            return columns;
        }

        /// <summary>
        /// 根据实体的类型返回具体类型的属性值
        /// </summary>
        /// <param name="propertyTypeName">属性类型的名称</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        protected object GetValue(string propertyTypeName, string propertyValue)
        {
            object val = null;
            if (propertyTypeName.ToLower().Trim() != "system.string")
            {
                var propertyType = Type.GetType(propertyTypeName);
                var method = propertyType.GetMethod("Parse", new Type[] { typeof(string) });
                val = method.Invoke(null, new object[] { propertyValue });
            }
            else
            {
                val = propertyValue;
            }
            return val;
        }

        /// <summary>
        /// 根据类名称获取对应的配置对象
        /// </summary>
        /// <param name="name">类型名称，如Model.Account</param>
        /// <returns>映射配置对象</returns>
        protected ObjectMapping GetMappingByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ErrMsgs.Add("对象的名称必须指定");
                return null;
            }

            ObjectMapping mapping = ObjectMappings.FirstOrDefault(s => s.ObjectName == name);
            if (mapping == null)
            {
                ErrMsgs.Add(string.Format("指定的类名称{0}不存在,请检查名称或者是否引用了对应的DLL", name));
                return null;
            }

            return mapping;
        }

        /// <summary>
        /// 返回经过过滤重复数据的对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filepath"></param>
        /// <param name="isDistinct">是否过滤重复数据，true 是，false 否</param>
        /// <returns>经过过滤重复数据的对象</returns>
        public IEnumerable<T> GenerateObjectsFromFile(string name, string filepath, bool isDistinct)
        {
            IEnumerable<T> list = GenerateObjectsFromFile(name, filepath);
            if (isDistinct && list != null)
                list = list.Distinct();
            return list;
        }

        public abstract IEnumerable<T> GenerateObjectsFromFile(string name, string filepath);
    }


    ///// <summary>
    ///// 从CSV导入
    ///// </summary>
    ///// <typeparam name="T">实体类型</typeparam>
    //public class ObjectFromCSVFile<T> : ObjectFromFile<T> where T : class,new()
    //{
    //    public override IEnumerable<T> GenerateObjectsFromFile(string name, string filepath)
    //    {
    //        ObjectMapping mapping = GetMappingByName(name);
    //        if (mapping == null)
    //            return null;

    //        List<T> list = new List<T>();
    //        using (Wedo.SimpleCSV.SimpleCSVReader reader = new SimpleCSV.SimpleCSVReader(filepath, System.Text.Encoding.UTF8))
    //        {
    //            Type tType = typeof(T);
    //            reader.Splitter = mapping.Splitter.FirstOrDefault();

    //            reader.HasHeader = true;
    //            while (reader.ReadLine())
    //            {
    //                T inst = new T();
    //                foreach (var item in mapping.Properties)
    //                {
    //                    object val = GetValue(item.PropertyType, reader[item.FileTitle]);
    //                    tType.GetProperty(item.PropertyName).SetValue(inst, val, null);
    //                }
    //                list.Add(inst);
    //            }
    //        }
    //        return list;
    //    }
    //}

    /// <summary>
    /// 从Excel文件生成对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectFromExcelFile<T> : ObjectFromFile<T> where T : class,new()
    {
        /// <summary>
        /// 将EXCEL中的SHEET生成为DataSet中的表，并返回DataSet        
        /// </summary>
        /// <param name="aboslutePath">Excel文件的地址</param>
        /// <param name="sheetnames">SHEET的名称列表</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataTableFromExcel(string aboslutePath, IList<string> sheetnames)
        {
            DataSet ds = new DataSet();
            string strConn = null;
            string ext = System.IO.Path.GetExtension(aboslutePath).ToLower();
            if (ext == ".xlsx")
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + aboslutePath + "';Extended Properties='Excel 12.0;HDR=YES'";
            else if (ext == ".xls")
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + aboslutePath + "';Extended Properties='Excel 8.0;HDR=YES;'";
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                OleDbDataAdapter adapter = null;
                foreach (var shname in sheetnames)
                {
                    adapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}$]", shname), conn);
                    adapter.Fill(ds, shname);
                }
            }
            return ds;
        }

        public override IEnumerable<T> GenerateObjectsFromFile(string name, string filepath)
        {
            if (!System.IO.File.Exists(filepath))
            {
                ErrMsgs.Add(string.Format("指定的文件{0}不存在", filepath));
                return null;
            }
            ObjectMapping mapping = GetMappingByName(name);
            if (mapping == null)
                return null;
            if (string.IsNullOrWhiteSpace(mapping.Splitter))
            {
                ErrMsgs.Add("请指定需要处理的Sheet的名称列表");
                return null;
            }
            var sheetNames = mapping.Splitter.Split(',').ToList();
            DataSet ds = GetDataTableFromExcel(filepath, sheetNames);
            if (ds.Tables == null || ds.Tables.Count <= 0)
            {
                ErrMsgs.Add(string.Format("找不到指定的{0}sheets", mapping.Splitter));
                return null;
            }

            List<T> list = new List<T>();

            foreach (var shName in sheetNames)
            {
                Type tType = typeof(T);
                if (!ds.Tables.Contains(shName))
                    break;
                foreach (DataRow row in ds.Tables[shName].Rows)
                {
                    T inst = new T();
                    foreach (var item in mapping.Properties)
                    {
                        try
                        {
                            object val = GetValue(item.PropertyType, row[item.FileTitle].ToString());
                            tType.GetProperty(item.PropertyName).SetValue(inst, val, null);
                        }
                        catch (Exception ex)
                        {
                            ErrMsgs.Add(ex.Message);
                        }
                    }
                    list.Add(inst);
                }
            }
            return list;
        }
    }
}
