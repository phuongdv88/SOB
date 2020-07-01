using Mi.BoCached.Common;
using Mi.Entity.Base.News;
using Mi.MainDal.Databases;

namespace Mi.BoCached.CacheObjects
{
  public  class ConfigCached: CacheObjectBase
    {
        public ConfigEntity GetByConfigName(string configName)
        {
            var cachedKey = string.Format("GetByConfigName[{0}]", configName);
            var data = Get<ConfigEntity>(configName, cachedKey);
            if (data == null)
            {
                using (var db = new CmsMainDb())
                {
                    data = db.ConfigMainDal.GetByConfigName(configName);
                    Add(configName, cachedKey, data);
                }
            }
            return data;
        }
    }
}
