using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wedo.Utility.ExportFile
{
    public interface IExportFileFactory
    {
         IExportFile CreateExportFile(string filePath, string filename, Object dataSource);
         IExportFile CreateExportFile(string filePath, string filename, Object dataSource, Encoding encode);
    }

    public class ExportCsvFactory : IExportFileFactory 
    {
        public IExportFile CreateExportFile(string filePath, string filename, Object dataSource) 
        {
            return new ExportCsv(filePath, filename, dataSource);
        }
        public IExportFile CreateExportFile(string filePath, string filename, Object dataSource, Encoding encode)
        {
            return new ExportCsv(filePath, filename, dataSource, encode);
        }
    }

    public class ExportTxtFactory : IExportFileFactory
    {
        public IExportFile CreateExportFile(string filePath, string filename, Object dataSource)
        {
            return new ExportTxt(filePath, filename, dataSource);
        }
        public IExportFile CreateExportFile(string filePath, string filename, Object dataSource, Encoding encode)
        {
            return new ExportTxt(filePath, filename, dataSource, encode);
        }
    }
}
