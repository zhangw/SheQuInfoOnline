using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wedo.Utility
{
    public  class ConfigData
    {
        public static string GetStrData(string name)
        {
           return System.Configuration.ConfigurationManager.AppSettings[name];
        }

        public static int? GetIntData(string name)
        {
            return GetStrData(name).TryParseTo<int>() ;
        }

        public static decimal? GetDecimalData(string name)
        {
            return GetStrData(name).TryParseTo<decimal>();
        }

        public static Nullable<T> GetTData<T>(string name) where T:struct
        {
            return GetStrData(name).TryParseTo<T>();
        }
    }
}
