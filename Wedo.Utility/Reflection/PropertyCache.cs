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
    public class PropertyCache
    {
        public static readonly PropertyCache Instance = new PropertyCache();

        SyncDictionary<string, SyncDictionary<string, PropertyInfo>> types = new SyncDictionary<string, SyncDictionary<string, PropertyInfo>>();

        void ResoleType(Type type)
        {
            if (type == null || types.ContainsKey(type.FullName))
                return;

            types[type.FullName] = new SyncDictionary<string, PropertyInfo>();

            foreach (var property in type.GetProperties())
            {
                types[type.FullName][property.Name] = property;
            }
        }

        void ResoleInterface(Type type)
        {
            ResoleType(type);
            foreach (var itype in type.GetInterfaces())
            {
                ResoleInterface(itype);
                foreach (var properties in types[itype.FullName])
                    types[type.FullName][properties.Key] = properties.Value;
            }
        }

        void ResoleClass(Type type)
        {
            ResoleType(type);
        }

        void Resole(Type type)
        {
            if (!types.ContainsKey(type.FullName))
            {
                if (type.IsInterface)
                    ResoleInterface(type);
                else
                    ResoleClass(type);
            }
        }

        public SyncDictionary<string, PropertyInfo> this[Type type]
        {
            get
            {
                if (!types.ContainsKey(type.FullName))
                    Resole(type);
                return types[type.FullName];
            }
        }
    }
}
