using System;
using System.Web.Caching;

namespace Framework.Caching
{
    public class WebCache
    {
        static Cache _cache = new Cache();

        public static void Set(string key, object val)
        {
            _cache.Insert(key, val);
        }

        public static void Set(string key, object val, CacheDependency dependency)
        {
            _cache.Insert(key, val, dependency);
        }

        public static void Set(string key, object val, CacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            _cache.Insert(key, val, dependency, absoluteExpiration, slidingExpiration);
        }

        public static object Get(string key)
        {
            return _cache.Get(key);
        }

        public static T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        public static void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
