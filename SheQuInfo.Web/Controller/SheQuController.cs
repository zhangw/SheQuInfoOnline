using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SheQuInfo.Models;
using Wedo.Mvc.Utility;

namespace SheQuInfo.Web.Controller
{
    public class SheQuController : BaseController
    {
        protected IUnitWork mUnitWork = null;

        public SheQuController(IUnitWork unitWork)
        {
            this.mUnitWork = unitWork;
        }
    }
}