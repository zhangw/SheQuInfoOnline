using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Wedo.Utility.Reflection.HistoryLog
{
    /// <summary>
    /// 获取变更数据帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HistorylHelper<T> where T : class
    {
        private string _CreatedBy;
        private string _OwnerToId;
        private int _Version;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="createdBy">创建人</param>
        /// <param name="ownerId">被变更记录的编号</param>
        /// <param name="version">版本号</param>
        public HistorylHelper(string createdBy, string ownerId, int version)
        {
            this._CreatedBy = createdBy;
            this._OwnerToId = ownerId;
            this._Version = version;
        }

        /// <summary>
        /// 创建初始化版本对象
        /// </summary>
        /// <returns></returns>
        private HistoryLogModel CreateModel()
        {
            HistoryLogModel model = new HistoryLogModel()
            {
                Id = Guid.NewGuid(),
                ChangeMan = _CreatedBy,
                Version = _Version,
                CreatedBy = _CreatedBy,
                CreatedTime = DateTime.Now,
                ModelName = typeof(T).Name,
                OwnerId = _OwnerToId,
                ModelChangeLogItems = new List<HistoryLogItem>()
            };
            return model;
        }


        private HistoryLogItem GetChangeItem(PropertyInfo property, T sourceVal, T newVal)
        {
            object sourceValue = property.GetValue(sourceVal, null);
            object newValue = property.GetValue(newVal, null);

            if (sourceValue == newValue)
                return null;

            string name = property.Name;
            string title = property.Name;
            var description = property.GetCustomAttributes(true).OfType<DescriptionAttribute>().FirstOrDefault();
            if (description != null)
            {
                title = description.Description;
            }
            else
            {
                var disAttr = property.GetCustomAttributes(true).OfType<DisplayNameAttribute>().FirstOrDefault();
                if (disAttr != null)
                    title = disAttr.DisplayName;
            }

            HistoryLogItem item = new HistoryLogItem()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Title = title,
                SourceValue = sourceValue.ToString(),
                NewValue = newValue.ToString(),
            };
            return item;
        }


        public HistoryLogModel CreateHistory(T sourceVal, T newVal)
        {
            if (sourceVal == null || newVal == null)
                throw new ArgumentNullException("源对象和新对象都不能为空");
            HistoryLogModel model = CreateModel();
            foreach (var item in PropertyCache.Instance[typeof(T)])
            {
                if (item.Value.PropertyType.IsValueType || item.Value.PropertyType == typeof(string))
                {
                    var logItem = GetChangeItem(item.Value, sourceVal, newVal);
                    logItem.ParentId = model.Id;
                    model.ModelChangeLogItems.Add(logItem);
                }
            }

            if (model.ModelChangeLogItems == null || model.ModelChangeLogItems.Count <= 0)
                return null;

            return model;
        }
    }
}
