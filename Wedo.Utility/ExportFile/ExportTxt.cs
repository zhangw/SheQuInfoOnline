using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wedo.Utility.ExportFile
{
    public class ExportTxt : BaseExportFile, IExportFile
    {
        public ExportTxt(string filePath, string filename, Object datasource)
            : base(filePath, filename, datasource)
        {
            this._filename = filename;
            this._filepath = filePath;
            this._dataSource = datasource;
            this._sperator = "\t";
        }

        public ExportTxt(string filePath, string filename, Object datasource, Encoding encode)
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
            if (thelaststr)
            {
                str += "\r\n";
            }
            else 
            {
                str += _sperator;
            }
            return str;
        }
    }
}
