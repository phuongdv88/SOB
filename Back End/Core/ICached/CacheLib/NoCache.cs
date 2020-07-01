using System;
using Mi.BoCached.Common;

namespace Mi.BoCached.CacheLib
{
    public class NoCache : ICached
    {
        public bool Add<T>(string key, T value)
        {
            return true;
        }

        public bool Add<T>(string key, T value, DateTime expiredDate)
        {
            return true;
        }

        public bool Remove(string key)
        {
            return true;
        }

        public bool Exists(string key)
        {
            return false;
        }

        public T Get<T>(string key)
        {
            return default(T);
        }
    }
}
