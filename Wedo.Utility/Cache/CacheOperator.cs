using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Wedo.Utility.Cache
{
    /// <summary>
    /// 缓存操作接口
    /// </summary>
    public interface ICacheOperator
    {
        object GetData(string key);
        void Insert(string key, object value);
    }

    /// <summary>
    /// 基于文件的缓存操作
    /// </summary>
    public class FileDependencyCacheOperator : ICacheOperator
    {
        public string FilePath { set; get; }

        /// <summary>
        /// 从缓存中获取缓存的对象
        /// </summary>
        /// <param name="cacheKey">缓存的KEY</param>
        /// <returns>获取缓存的对象</returns>
        public object GetData(string cacheKey)
        {
            if (HttpRuntime.Cache[cacheKey] != null)
                return HttpRuntime.Cache[cacheKey];
            return null;
        }

        /// <summary>
        /// 添加一个基于文件的缓存
        /// </summary>
        /// <param name="filepath">加载的文件</param>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存的VALUE</param>
        public void Insert(string key, object value)
        {
            CacheDependency dependency = new CacheDependency(FilePath);
            HttpRuntime.Cache.Insert(key, value, dependency);
        }
    }
}
