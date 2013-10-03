using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Wedo.Utility.Reflection
{
    /// <summary>
    /// 根据Enum类型生成Dictionary<value,description>的字典表以及根据Enum值返回Description
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TBaseType"></typeparam>
    public static class EnumHelper<TEnum, TBaseType>
        where TEnum : struct
        where TBaseType : struct
    {
        /// <summary>
        /// 返回指定的ENUM的描述内容
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetDescription(TBaseType e)
        {
            try
            {
                TBaseType type = (TBaseType)Convert.ChangeType(e, typeof(TBaseType));
                return GetByDict()[type];

            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 返回指定的枚举的值和描述列表，{1:'添加',2:'更新'}
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Dictionary<TBaseType, string> GetByDict()
        {
            Dictionary<TBaseType, string> dic = new Dictionary<TBaseType, string>();

            var temp = EnumFieldCache.Instance[typeof(TEnum)];
            foreach (var item in temp.Values)
            {
                var val = (TBaseType)Convert.ChangeType(item.GetValue(null), typeof(TBaseType));
                var name = item.Name;
                var desc = item.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault();
                if (desc != null)
                    name = desc.Description;
                dic[val] = name;
            }
            return dic;
        }

        /// <summary>
        /// 返回指定的ENUM的描述内容
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetDescription(Enum e)
        {
            try
            {
                TBaseType type = (TBaseType)Convert.ChangeType(e, typeof(TBaseType));
                return GetByDict()[type];

            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
