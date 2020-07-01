using System;

namespace Mi.BoCached.Common
{
    public interface ICached
    {
        bool Add<T>(string key, T value);
        bool Add<T>(string key, T value, DateTime expiredDate);
        bool Remove(string key);
        bool Exists(string key);
        T Get<T>(string key);
    }
}
