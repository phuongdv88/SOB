using System;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using Mi.BoCached.Common;

namespace Mi.BoCached.CacheLib
{
    public class HttpWebCache : ICached
    {
        public bool Add<T>(string key, T value)
        {
            try
            {
                HttpContent.Current.Cache.Remove(key);
                HttpContext.Current.Cache.Add(key, value, null, DateTime.Now.AddDays(1), TimeSpan.Zero,
                                              CacheItemPriority.Default, null);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Add<T>(string key, T value, DateTime expiredDate)
        {
            try
            {
                HttpContext.Current.Cache.Remove(key);
                HttpContext.Current.Cache.Add(key, value, null, expiredDate, TimeSpan.Zero,
                                              CacheItemPriority.Default, null);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Remove(string key)
        {
            try
            {
                HttpContext.Current.Cache.Remove(key);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Exists(string key)
        {
            return HttpContext.Current.Cache[key] != null;
        }

        public T Get<T>(string key)
        {
            try
            {
                return (T)HttpContext.Current.Cache[key];
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
