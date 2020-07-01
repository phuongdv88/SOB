using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Mi.BoCached.CacheLib;


namespace Mi.BoCached.Common
{
    public class CacheObjectBase
    {
        #region Instance manager

        private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
        private static Dictionary<string, object> _instanceList;
        public static T GetInstance<T>()
        {
            if (null == _instanceList) _instanceList = new Dictionary<string, object>();

            var instanceType = typeof(T);
            var instanceName = instanceType.FullName;
            if (!_instanceList.ContainsKey(instanceName) || _instanceList[instanceName] == null)
            {
                var newInstance = CurrentAssembly.CreateInstance(instanceName, false,
                                                                 BindingFlags.CreateInstance,
                                                                 null,
                                                                 null,
                                                                 System.Globalization.CultureInfo.CurrentCulture,
                                                                 null);

                if (null != newInstance)
                {
                    if (!_instanceList.ContainsKey(instanceName))
                    {
                        _instanceList.Add(instanceName, newInstance);
                    }
                    else
                    {
                        _instanceList[instanceName] = newInstance;
                    }
                    return (T)newInstance;
                }
                return default(T);
            }
            return (T)_instanceList[instanceName];
        }

        #endregion

        #region Private members

        //private static readonly List<string> AllCachedKey = new List<string>();
        private static readonly Dictionary<string, List<string>> AllCachedKeyByGroup = new Dictionary<string, List<string>>();
        private static readonly  List<string> AllCachedKey = new List<string>();
        private static string _cacheKeyPrefix;
        private static long _cacheExpired;
        private static ICached _cached;
        private static ICached Cached
        {
            get
            {
                if (_cached == null)
                {
                    try
                    {
                        var assemblyInString = CmsChannelConfiguration.GetAppSetting("BoCached.CacheType");
                        if (string.IsNullOrEmpty(assemblyInString))
                        {
                            _cached = new NoCache();
                        }
                        else
                        {
                            _cached = (ICached)Activator.CreateInstance(Type.GetType(assemblyInString));
                        }
                    }
                    catch (Exception ex)
                    {
                        _cached = new NoCache();
                    }
                }
                return _cached;
            }
        }
        private static string CacheKeyPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(_cacheKeyPrefix))
                {
                    _cacheKeyPrefix = CmsChannelConfiguration.GetAppSetting("BoCached.CacheKeyPrefix");
                }
                return _cacheKeyPrefix;
            }
        }
        private static long CacheExpired
        {
            get
            {
                if (_cacheExpired <= 0)
                {
                    _cacheExpired =
                        Utility.ConvertToLong(CmsChannelConfiguration.GetAppSetting("BoCached.CacheExpired")) * 1000; // In seconds
                    if (_cacheExpired <= 0) _cacheExpired = 60 * 60 * 1000;
                }
                return _cacheExpired;
            }
        }
        private string GetCacheKey(string key)
        {
            return CacheKeyPrefix + key;
        }
        private string GetCacheKey(string group, string key)
        {
            return CacheKeyPrefix + group + key;
        }

        #endregion

        #region Cached methods

        protected bool Add<T>(string key, T value)
        {
            var cachedKey = GetCacheKey(key);
            if (!AllCachedKey.Contains(cachedKey)) AllCachedKey.Add(cachedKey);
            return Cached.Add(cachedKey, value, DateTime.Now.AddSeconds(CacheExpired));
        }
        protected bool Remove(string key)
        {
            var cachedKey = GetCacheKey(key);
            if (!AllCachedKey.Contains(cachedKey)) AllCachedKey.Remove(cachedKey);
            return Cached.Remove(cachedKey);
        }
        protected bool Exists(string key)
        {
            var cachedKey = GetCacheKey(key);
            return Cached.Exists(cachedKey);
        }
        protected T Get<T>(string key)
        {
            var cachedKey = GetCacheKey(key);
            return Cached.Get<T>(cachedKey);
        }

        #endregion

        #region Cached methods by group

        protected bool Add<T>(string group, string key, T value)
        {
            var cachedKey = GetCacheKey(group, key);
            if (!AllCachedKeyByGroup.ContainsKey(group)) AllCachedKeyByGroup.Add(group, new List<string>());

            var listKeyInGroup = AllCachedKeyByGroup[group];
            if (!listKeyInGroup.Contains(cachedKey)) listKeyInGroup.Add(cachedKey);
            AllCachedKeyByGroup[group] = listKeyInGroup;

            return Cached.Add(cachedKey, value, DateTime.Now.AddSeconds(CacheExpired));
        }
        protected bool Remove(string group, string key)
        {
            var cachedKey = GetCacheKey(group, key);
            if (AllCachedKeyByGroup.ContainsKey(group))
            {
                var listKeyInGroup = AllCachedKeyByGroup[group];
                if (!listKeyInGroup.Contains(cachedKey)) listKeyInGroup.Remove(cachedKey);
                AllCachedKeyByGroup[group] = listKeyInGroup;
            }
            return Cached.Remove(cachedKey);
        }
        protected bool Exists(string group, string key)
        {
            var cachedKey = GetCacheKey(group, key);
            return Cached.Exists(cachedKey);
        }
        protected T Get<T>(string group, string key)
        {
            var cachedKey = GetCacheKey(group, key);
            return Cached.Get<T>(cachedKey);
        }

        #endregion

        public virtual void RemoveAllCached()
        {
            //for (var i = AllCachedKey.Count - 1; i >= 0; i--)
            //{
            //    Cached.Remove(AllCachedKey[i]);
            //    AllCachedKey.RemoveAt(i);
            //}
            foreach (var key in AllCachedKeyByGroup.Keys)
            {
                RemoveAllCachedByGroup(key, false);
            }
            AllCachedKeyByGroup.Clear();
        }

        public virtual void RemoveAllCachedByGroup(string group, bool removeGroup = true)
        {
            if (AllCachedKeyByGroup.ContainsKey(group))
            {
                var listKeyInGroup = AllCachedKeyByGroup[group];
                for (var i = listKeyInGroup.Count - 1; i >= 0; i--)
                {
                    Cached.Remove(listKeyInGroup[i]);
                    listKeyInGroup.RemoveAt(i);
                }
                if (removeGroup)
                {
                    AllCachedKeyByGroup.Remove(group);
                }
                else
                {
                    AllCachedKeyByGroup[group] = listKeyInGroup;
                }
            }
        }
    }
}
