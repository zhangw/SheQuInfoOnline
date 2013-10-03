using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wedo.Utility
{
     public class DateHelper
    {
        static public double GetDataDiffWithWorkingDay(DateTime dSource, DateTime dMinimizer, bool countWorkingDay)
        {
            if (dSource > dMinimizer)
                return -1;
            if (dSource == dMinimizer)
                return 0;

            TimeSpan span = dMinimizer - dSource;
            if (countWorkingDay)
                return span.TotalHours;  
            else
            {
                int hours = 0;
                DateTime startDate = dSource;
                if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                    hours += 24 - startDate.Hour;
                while (startDate < dMinimizer)
                {
                    startDate = startDate.AddDays(1);
                    if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                        hours += 24;
                    else
                        continue;
                }
                return span.TotalHours - hours;
            }
        }
    }
}
