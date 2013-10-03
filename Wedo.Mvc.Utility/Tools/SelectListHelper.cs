using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Wedo.Mvc.Utility
{
    public static class SelectListHelper
    {
        /// <summary>
        /// 生成选择列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="list">对象列表</param>
        /// <param name="selectedList">已经选中的列表</param>
        /// <param name="textName">文本的对象属性名称</param>
        /// <param name="valueName">值的对象属性和名称</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GenerateFromList<T>(List<T> list, string textName, string valueName, bool needDefault = false)
        {
            return GenerateFromList<T>(list, null, textName, valueName, needDefault);
        }

        /// <summary>
        /// 生成选择列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="list">对象列表</param>
        /// <param name="selectedList">已经选中的列表</param>
        /// <param name="textName">文本的对象属性名称</param>
        /// <param name="valueName">值的对象属性和名称</param>
        /// <param name="needDefault">是否需要默认项，如"请选择"</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GenerateFromList<T>(List<T> list, List<T> selectedList, string textName, string valueName, bool needDefault = false)
        {
            if (list == null)
                throw new ArgumentNullException("对象列表不能为null");

            if (string.IsNullOrWhiteSpace(textName) || string.IsNullOrWhiteSpace(valueName))
                throw new ArgumentNullException("textName和valueName都不能为空");

            Type type = typeof(T);

            if (type.GetProperty(textName) == null || type.GetProperty(valueName) == null)
                throw new ArgumentException(string.Format("指定的属性名称{0}或{1}不存在", textName, valueName));

            List<SelectListItem> selectList = new List<SelectListItem>();

            if (needDefault)
            {
                selectList.Add(new SelectListItem()
                {
                    Text = "请选择",
                    Value = "",
                    Selected = (selectedList == null || selectedList.Count <= 0) ? true : false
                });
            }

            if (list.Count == 0)
                return selectList;

            var textProperty = type.GetProperty(textName);
            var valueProperty = type.GetProperty(valueName);

            foreach (var item in list)
            {
                var sItem = new SelectListItem()
                {
                    Text = textProperty.GetValue(item, null).ToString(),
                    Value = valueProperty.GetValue(item, null).ToString()
                };
                if (selectedList != null && selectedList.Contains(item))
                    sItem.Selected = true;
                selectList.Add(sItem);
            }

            return selectList;
        }


        /// <summary>
        /// 根据Enum生成选择列表，其中Text为Enum字段的Description,Value值为Enum的基本值
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <typeparam name="TBaseType">枚举的基本类型</typeparam>
        /// <param name="val">应该选中的值</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GenerateListFromEnum<TEnum, TBaseType>(TEnum val)
            where TEnum : struct
            where TBaseType : struct
        {
            var value = (TBaseType)Convert.ChangeType(val, typeof(TBaseType));
            var list = GenerateListFromEnum<TEnum, TBaseType>().ToList();
            list.ForEach(s =>
            {
                if (s.Value == value.ToString())
                    s.Selected = true;
            });
            return list;
        }

        /// <summary>
        /// 根据Enum生成选择列表，其中Text为Enum字段的Description,Value值为Enum的基本值
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <typeparam name="TBaseType">枚举的基本类型</typeparam>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GenerateListFromEnum<TEnum, TBaseType>()
            where TEnum : struct
            where TBaseType : struct
        {
            var items = Wedo.Utility.Reflection.EnumHelper<TEnum, TBaseType>.GetByDict();
            var list = new List<SelectListItem>();
            foreach (var item in items)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            return list;
        }
    }
}
