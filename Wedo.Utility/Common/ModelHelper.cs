using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wedo.Utility
{
    public static class ModelHelper
    {
        /// <summary>
        /// 检查对象是否为空
        /// </summary>
        /// <param name="obj"></param>
        public static void CheckNull(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("对象");
        }

        public static void CheckNull<T>(this T obj,string errorMSG="不能为空") where T : class
        {
            if (obj == null)
                throw new Exception(errorMSG);
        }

        /// <summary>
        /// 根据GUID生成不重复的ID
        /// </summary>
        /// <returns></returns>
        public static string GetNewID()
        {
            return string.Format("{0:x}", GetNewlongID());
        }

        /// <summary>
        /// 根据GUID生成不重复的ID,长整型
        /// </summary>
        /// <returns></returns>
        public static long GetNewlongID()
        {
            long i = 1;
            var ar = Guid.NewGuid().ToByteArray();
            foreach (byte b in ar)
            {
                i *= ((int)b + 1);
            }
            return i;
        }

        /// <summary>
        /// 转换成子类的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ticket"></param>
        /// <param name="targetTicket"></param>
        public static void ToChildModel<T>( T src, T target) where T : class
        {
            Type type = typeof(T);
            var fields = type.GetProperties();

            for (int i = 0; i < fields.Length; i++)
            {
                var item = fields[i];
                var value = item.GetValue(src, null);
                item.SetValue(target, value, null);
            }
        }

        /// <summary>
        /// 将一个的对象属性值赋给另一个制定的对象属性
        /// </summary>
        /// <param name="src">原数据对象</param>
        /// <param name="target">目标数据对象</param>
        /// <param name="changeProperties">属性集，键为原属性，值为目标属性</param>
        /// <param name="unChangeProperties">属性集，目标不修改的属性</param>
        public static void CopyTo(object src, object target, Dictionary<string, string> changeProperties = null, string[] unChangeProperties = null)
        {
            if (src == null || target == null)
                throw new ArgumentException("src == null || target == null ");

            var SourceType = src.GetType();
            var TargetType = target.GetType();

            if (changeProperties == null || changeProperties.Count == 0)
            {
                var fields = TargetType.GetProperties();
                changeProperties = fields.Select(m => m.Name).ToDictionary(m => m);
            }

            if (unChangeProperties == null || unChangeProperties.Length == 0)
            {
                foreach (var item in changeProperties)
                {
                    var srcProperty = SourceType.GetProperty(item.Key);
                    if (srcProperty != null)
                    {
                        var sourceVal = srcProperty
                            .GetValue(src, null);

                        var tarProperty = TargetType.GetProperty(item.Value);
                        if (tarProperty != null)
                        {
                            tarProperty.SetValue(target, sourceVal, null);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in changeProperties)
                {
                    if (!unChangeProperties.Any(m => m == item.Value))
                    {
                        var srcProperty = SourceType.GetProperty(item.Key);
                        if (srcProperty != null)
                        {
                            var sourceVal = srcProperty
                                .GetValue(src, null);

                            var tarProperty = TargetType.GetProperty(item.Value);
                            if (tarProperty != null)
                            {
                                tarProperty.SetValue(target, sourceVal, null);
                            }
                        }
                    }
                }
            }
        } 

    }
}
