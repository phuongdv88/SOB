using System;
using David.Utility.RedisClientHelper;
using Mi.BoCached.Common;


namespace Mi.BoCached.CacheLib
{
    //public class RedisCache : ICached
    //{
    //    private int DbNumber
    //    {
    //        get { return Utility.ConvertToInt(ServiceChannelConfiguration.GetAppSetting(WcfMessageHeader.Current.Namespace, "RedisPublishDb")); }
    //    }

    //    public bool Add<T>(string key, T value)
    //    {
    //        try
    //        {
    //            RedisHelper.Remove(DbNumber, key);
    //            RedisHelper.Add(DbNumber, key, value);
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //        }
    //    }

    //    public bool Add<T>(string key, T value, DateTime expiredDate)
    //    {
    //        try
    //        {
    //            RedisHelper.Remove(DbNumber, key);
    //            RedisHelper.Add(DbNumber, key, value, expiredDate);
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //        }
    //    }

    //    public bool Remove(string key)
    //    {
    //        try
    //        {
    //            RedisHelper.Remove(DbNumber, key);
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //        }
    //    }

    //    public bool Exists(string key)
    //    {
    //        return RedisHelper.Get(DbNumber, key) != null;
    //    }

    //    public T Get<T>(string key)
    //    {
    //        try
    //        {
    //            return RedisHelper.Get<T>(DbNumber, key);
    //        }
    //        catch (Exception ex)
    //        {
    //            return default(T);
    //        }
    //    }
    //}
}
