using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace Wedo.Utility.ExportFile
{
    public interface IExportFile
    {
        FileInfo Export();
    }
}
