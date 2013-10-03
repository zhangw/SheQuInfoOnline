using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SheQuInfo.Models
{
    /// <summary>
    /// 实体已存在异常
    /// </summary>
    public class EntityExistsException : ApplicationException
    {
        public EntityExistsException(string message) : base(message) { }
    }

    /// <summary>
    /// 实体不存在
    /// </summary>
    public class EntityNotExistsException : ApplicationException
    {
        public EntityNotExistsException(string message) : base(message) { }
    }
}