using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Wedo.Utility.Reflection
{
    /// <summary>
    /// 属性缓存类
    /// </summary>
    public class EnumFieldCache
    {
        public static readonly EnumFieldCache Instance = new EnumFieldCache();

        SyncDictionary<string, SyncDictionary<string, FieldInfo>> types = new SyncDictionary<string, SyncDictionary<string, FieldInfo>>();

        void ResoleType(Type type)
        {
            if (type == null || types.ContainsKey(type.FullName))
                return;
            types[type.FullName] = new SyncDictionary<string, FieldInfo>();
            foreach (var property in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                types[type.FullName][property.Name] = property;
            }
        }

        public SyncDictionary<string, FieldInfo> this[Type type]
        {
            get
            {
                if (!types.ContainsKey(type.FullName))
                    ResoleType(type);
                return types[type.FullName];
            }
        }
    }
}
