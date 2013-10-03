using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Wedo.Utility.ExportFile
{
    public class ExportCsv : BaseExportFile, IExportFile
    {
        public ExportCsv(string filePath, string filename, Object datasource)
            : base(filePath, filename, datasource)
        {
            this._filename = filename;
            this._filepath = filePath;
            this._dataSource = datasource;
            this._sperator = "\t";
        }

        public ExportCsv(string filePath, string filename, Object datasource, Encoding encode)
            : base(filePath, filename, datasource, encode)
        {
            this._filename = filename;
            this._filepath = filePath;
            this._dataSource = datasource;
            this._sperator = "\t";
            this.EncodeType = encode;
        }

        public FileInfo Export()
        {
            return base.CreateFile();
        }

        protected override string FormatFileContent(string str, bool thelaststr)
        {
            str = str.Replace("\r\n", "");
            if (str.Contains(_sperator) || str.Contains("\""))
            {
                str = str.Replace("\"", "\"\"");
                str = "\"" + str + "\"";
            }
            if (thelaststr)
            {

                str += "\r\n";
            }
            else
            {
                str += _sperator;
            }
            return str;
            //throw new NotImplementedException();
        }
    }

    public class ExportRecieveVoucherCsv : ExportCsv
    {
        public ExportRecieveVoucherCsv(string filePath, string filename, Object datasource)
            : base(filePath, filename, datasource)
        {
            this._filename = filename;
            this._filepath = filePath;
            this._dataSource = datasource;
            this._sperator = "\t";
        }

        public ExportRecieveVoucherCsv(string filePath, string filename, Object datasource, Encoding encode)
            : base(filePath, filename, datasource, encode)
        {
            this._filename = filename;
            this._filepath = filePath;
            this._dataSource = datasource;
            this._sperator = "\t";
            this.EncodeType = encode;
        }

        protected override FileInfo WriteFileContent(dynamic obj)
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
                                Dictionary<string, string> recieveVoucherMap = ExportMap.ExportMaps["RecieveVoucher"];
                                string c = FormatFileContent(recieveVoucherMap[_property.Name], properties.Last() == _property);
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
    }
}
