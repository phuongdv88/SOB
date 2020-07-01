using System;
using System.Collections.Generic;
using Mi.BoCached.Common;

namespace Mi.BoCached.CacheLib
{
    public class InProcessMemoryCache : ICached
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public bool Add<T>(string key, T value)
        {
            try
            {
                if (!_cache.ContainsKey(key))
                {
                    _cache.Add(key, value);
                }
                else
                {
                    _cache[key] = value;
                }
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
                if (!_cache.ContainsKey(key))
                {
                    _cache.Add(key, value);
                }
                else
                {
                    _cache[key] = value;
                }
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
                if (_cache.ContainsKey(key)) _cache.Remove(key);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Exists(string key)
        {
            return _cache.ContainsKey(key);
        }

        public T Get<T>(string key)
        {
            try
            {
                return _cache.ContainsKey(key) ? (T) _cache[key] : default(T);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
