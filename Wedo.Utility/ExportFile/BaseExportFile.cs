using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;

namespace Wedo.Utility.ExportFile
{

    public abstract class BaseExportFile
    {
        protected Object _dataSource;
        protected string _filepath;
        protected string _filename;
        protected FileInfo _fileinfo;
        protected string _sperator;
        protected Encoding EncodeType { get; set; }
        public BaseExportFile(string filePath,string filename,dynamic dataSource) 
        {
            Construct(filePath, filename, dataSource);
            this.EncodeType = Encoding.Unicode;
        }

        public BaseExportFile(string filePath, string filename, dynamic dataSource, Encoding encode)
        {
            Construct(filePath, filename, dataSource);
            this.EncodeType = encode;
        }

        private void Construct(string filePath, string filename, dynamic dataSource) 
        {
            this._filepath = filePath;
            this._filename = filename;
            this._dataSource = dataSource;
        }

        private void LoadDataSource(dynamic dataSource) 
        {
            if (this._dataSource != null)
            {
                if (this._dataSource is IDataReader) 
                {
                    var dr = this._dataSource as IDataReader;
                    WriteFileContent(dr);
                }
                else if (this._dataSource is DataTable) 
                {
                    var dt = this._dataSource as DataTable;
                    WriteFileContent(dt);
                }
                else
                {
                    WriteFileContent(this._dataSource);
                }
            }
            else 
            {
                throw new ExportFileFailure("The DataSource of ExportFile is null.");
            }
        }

        protected FileInfo CreateFile() 
        {
            if (_filepath == null || _filepath == string.Empty)
            {
                throw new ExportFileFailure("The Directory of ExportFile is Not Found.");
            }
            if (string.IsNullOrEmpty(_filename))
            {
                throw new ExportFileFailure("The Name of ExportFile is null or empty.");
            }
            try
            {
                if (!Directory.Exists(_filepath))
                {
                    Directory.CreateDirectory(_filepath);
                }
            }
            catch (IOException ioException)
            {
                throw new ExportFileFailure("CreateDirectory Exception", ioException);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                throw new ExportFileFailure("CreateDirectory Exception", unauthorizedAccessException);
            }
            catch (Exception)
            {
                throw;
            }

            string filename = _filepath + _filename;
            _fileinfo = new FileInfo(filename);
            if (_fileinfo.Exists)
            {
                //文件已存在
                throw new ExportFileExist();
            }
            else
            {
                try
                {
                    this.LoadDataSource(this._dataSource);
                }
                catch (IOException ioException)
                {
                    throw new ExportFileFailure("CreateFile Exception", ioException);
                }
                catch (UnauthorizedAccessException unauthorizedAccessException)
                {
                    throw new ExportFileFailure("CreateFile Exception", unauthorizedAccessException);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return _fileinfo;
        }

        protected virtual FileInfo WriteFileContent(DataTable dataTable)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn column in dataTable.Columns)
            {
                string c = FormatFileContent(column.ColumnName, dataTable.Columns.IndexOf(column) == dataTable.Columns.Count - 1);
                sb.Append(c);
            }
            foreach (DataRow dr in dataTable.Rows)
            {
                for (int i = 0; i <= dataTable.Columns.Count - 1; i++)
                {
                    string r = FormatFileContent(dr[i].ToString(), i == dataTable.Columns.Count - 1);
                    sb.Append(r);
                }
            }
            //var utf8BOM = UnicodeEncoding.UTF8; // 指定encoding实例encoderShouldEmitUTF8Identifier
            //string s = sb.ToString();
            //int byteCount = utf8BOM.GetByteCount(s);

            //byte[] prefix = utf8BOM.GetPreamble(); // 取出UTF-8的BOM字节
            //byte[] output = new byte[prefix.Length + byteCount]; // 声明输出的字节数组，注意长度

            //System.Buffer.BlockCopy(prefix, 0, output, 0, prefix.Length); // 复制BOM
            //utf8BOM.GetBytes(s.ToCharArray(), 0, s.Length, output, prefix.Length); // 写入正文
            System.IO.File.WriteAllText(_fileinfo.FullName, sb.ToString(), EncodeType);
            return new FileInfo(_fileinfo.Name);
        }

        protected virtual FileInfo WriteFileContent(IDataReader dataReader)
        {
            using (StreamWriter sw = new StreamWriter(_fileinfo.FullName, false, EncodeType))
            {
                int count = 0;
                while (dataReader.Read()) 
                {
                    if (count == 0) 
                    {
                        for (int i = 0; i <= dataReader.FieldCount - 1; i++) 
                        {
                            string c = FormatFileContent(dataReader.GetName(i), i == dataReader.FieldCount - 1);
                            sw.Write(c);
                        }
                    }
                    for (int i = 0; i <= dataReader.FieldCount - 1; i++) 
                    {
                        string r = FormatFileContent(dataReader[i].ToString(), i == dataReader.FieldCount - 1);
                        sw.Write(r);
                    }
                    count++;
                }
            }
            return new FileInfo(_fileinfo.Name);
        }

        protected virtual FileInfo WriteFileContent(dynamic obj)
        {
            using (StreamWriter sw = new StreamWriter(_fileinfo.FullName, false, EncodeType))
            {
                if (obj.GetType().ToString() == @"System.Collections.Generic.List`1[System.Object]")
                {
                    int count = 0;
                    foreach (var _obj in obj)
                    {
                        Type typeRecieveVoucher = _obj.GetType();
                        var properties = typeRecieveVoucher.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        if (count == 0) 
                        {
                            foreach (var _property in properties)
                            {
                                string c = FormatFileContent(_property.Name, properties.Last() == _property);
                                sw.Write(c);
                            }
                        }
                        foreach (var _property in properties)
                        {
                            string c = FormatFileContent(_property.GetValue(_obj, _property.GetIndexParameters()).ToString(), properties.Last() == _property);
                            sw.Write(c);
                        }
                        count++;
                    }
                }
                else 
                {
                    throw new ExportFileFailure("The Type Of Datasouce Is Not Valid.");
                }
            }
            return new FileInfo(_fileinfo.Name);
        }

        protected abstract string FormatFileContent(string str, bool thelaststr);
    }
}
