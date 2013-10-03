using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wedo.Utility.ExportFile
{
    public class ExportFileFailure : ApplicationException
    {
        public ExportFileFailure()
            : base()
        {

        }
        public ExportFileFailure(string message)
            : base(message)
        {

        }
        public ExportFileFailure(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    public class ExportFileExist : ApplicationException
    {
        public ExportFileExist()
            : base()
        {

        }
        public ExportFileExist(string message)
            : base(message)
        {

        }
        public ExportFileExist(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
