using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using System.IO;

namespace Wedo.Utility.Export
{

    /// <summary>
    /// 导出类
    /// </summary>
    public abstract class ExportBase
    {
        /// <summary>
        /// 导出文件到文件中
        /// </summary>
        /// <param name="colNames">要导出的列名称</param>
        /// <param name="colLables">要导出的标签名称，与列名对应</param>
        /// <param name="reader">SQL查询的结果集</param>
        /// <param name="outFilePath">导出文件地址</param>
        public void Export(IList<string> colNames, IList<string> colLables, IDataReader reader, string outFilePath)
        {
            if (string.IsNullOrWhiteSpace(outFilePath))
                throw new FileNotFoundException("文件路径不能为空");

            using (Stream stream = System.IO.File.Open(outFilePath, FileMode.OpenOrCreate))
            {
                WriteToFile(colNames, colLables, reader, stream);
            }
        }

        /// <summary>
        /// 将查询结果写出到文件中，此方法由子类实现具体输出到何种文件中
        /// </summary>
        /// <param name="columns">列列表</param>
        /// <param name="reader">查出的数据源</param>
        /// <param name="outFilePath">要写入的文件路径</param>
        protected abstract void WriteToFile(IList<string> colNames, IList<string> colLables, IDataReader reader, Stream stream);

        /// <summary>
        /// 获取某一行所有值列表
        /// </summary>
        /// <param name="colNames"></param>
        /// <param name="reader"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        protected IList<string> GetValues(IList<string> colNames, IDataReader reader)
        {
            List<string> values = new List<string>();
            foreach (var col in colNames)
            {
                if (reader[col] != DBNull.Value)
                    values.Add(reader[col].ToString());
                else
                    values.Add(string.Empty);
            }
            return values;
        }
    }

    ///// <summary>
    ///// 导出数据到CSV中
    ///// </summary>
    //public class ExportToCsv : ExportBase
    //{
    //    /// <summary>
    //    /// 重写写文件操作
    //    /// </summary>
    //    /// <param name="columns">涉及的列</param>
    //    /// <param name="reader">SQLReader</param>
    //    /// <param name="outFilePath">导出文件地址</param>
    //    protected override void WriteToFile(IList<string> colNames, IList<string> colLables, IDataReader reader, Stream stream)
    //    {
    //        using (Wedo.SimpleCSV.SimpleCSVWriter writer = new Wedo.SimpleCSV.SimpleCSVWriter(stream, Encoding.UTF8))
    //        {
    //            writer.Splitter = ',';
    //            writer.WriteHeader(colLables.ToArray());
    //            while (reader.Read())
    //            {
    //                writer.WriteLine(GetValues(colNames, reader).ToArray<string>());
    //            }
    //        }
    //    }
    //}

    public class ExportToWebExcel : ExportBase
    {
        private string _content;
        public string Content { get { return _content; } }

        protected override void WriteToFile(IList<string> colNames, IList<string> colLables, IDataReader reader, Stream stream)
        {
            string colHeaders = "", ls_item = "";
            colHeaders = string.Join("\t", colLables) + "\n";
            while (reader.Read())
            {
                var array = GetValues(colNames, reader);
                var itemStr = string.Join("\t", array);
                ls_item += itemStr + "\n";
            }
            _content = colHeaders + "" + ls_item;
        }
    }
}
