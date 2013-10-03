using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Wedo.Mvc.Utility
{
    public class BaseTextDropdownList
    {
        protected List<SelectListItem> DataList = null;

        /// <summary>
        /// 相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected virtual List<SelectListItem> GetData(string path)
        {
            if (DataList == null)
            {
                DataList = new List<SelectListItem>();

                string context = System.IO.File.ReadAllText(path).Replace("\r\n", "");

                DataList = Parse(context);
            }
            return DataList;
        }

        /// <summary>
        /// 将文本转为数据列表
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected virtual List<SelectListItem> Parse(string text)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            string[] types = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in types)
            {
                var vals = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (vals != null && vals.Length > 0 && vals.Length < 4)
                {
                    if (vals.Length == 1)
                    {
                        string value = vals[0];
                        list.Add(new SelectListItem { Text = value, Value = value });
                    }
                    else if (vals.Length == 2)
                    {
                        string value = vals[0];
                        string name = vals[1];
                        list.Add(new SelectListItem { Text = name, Value = value });
                    }
                    else if (vals.Length == 3)
                    {
                        string value = vals[0];
                        string name = vals[1];
                        bool selected = vals[3] == "true";
                        list.Add(new SelectListItem { Text = name, Value = value, Selected = selected });
                    }
                }
                else
                {
                    throw new Exception("格式错误！");
                }
            }
            return list;
        }

        public virtual string GetText(string value)
        {
            if (DataList != null)
            {
                var data = DataList.FirstOrDefault(m => m.Value == value);
                if (data != null)
                    return data.Text;
            }
            return null;
        }

        public virtual List<SelectListItem> GetData()
        {
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "App_Data", GetType().Name + ".txt");

            return GetData(filePath);
        }

    }

    public abstract class BaseTextDictionary<TKey,TValue> 
    {
        protected Dictionary<TKey, TValue> DataList = null;

        /// <summary>
        /// 相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected virtual Dictionary<TKey, TValue> GetData(string path)
        {
            if (DataList == null)
            {
                DataList = new Dictionary<TKey, TValue>();

                string context = System.IO.File.ReadAllText(path).Replace("\r\n", "");

                DataList = Parse(context);
            }
            return DataList;
        }

        /// <summary>
        /// 将文本转为数据列表
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected virtual Dictionary<TKey, TValue> Parse(string text)
        {
            Dictionary<TKey, TValue> list = new Dictionary<TKey, TValue>();
            string[] types = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in types)
            {
                var vals = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (vals != null && vals.Length > 0 && vals.Length < 3)
                {
                    if (vals.Length == 1)
                    {
                        string value = vals[0];
                        list.Add( ParseToKey(value), ParseToValue(value ));
                    }
                    else if (vals.Length == 2)
                    {
                        string value = vals[0];
                        string name = vals[1];
                        list.Add(ParseToKey(value), ParseToValue(name));
                    }
                }
                else
                {
                    throw new Exception("格式错误！");
                }
            }
            return list;
        }

        protected abstract TKey ParseToKey(string value);

        protected abstract TValue ParseToValue(string value);


        public virtual TValue GetTValue(TKey value)
        {
            return DataList[value];
        }

        public virtual Dictionary<TKey, TValue> GetData()
        {
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", GetType().Name + ".txt");

            return GetData(filePath);
        }

    }


    public class IntStrTextDict : BaseTextDictionary<int, string>
    {

        protected override int ParseToKey(string value)
        {
            return int.Parse(value);
        }

        protected override string ParseToValue(string value)
        {
            return value;
        }
    }

    public class  StrTextDict : BaseTextDictionary<string, string>
    {
        protected override string ParseToKey(string value)
        {
            return value;
        }

        protected override string ParseToValue(string value)
        {
            return value;
        }
    }
 
}
