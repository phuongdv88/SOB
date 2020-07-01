using Mi.BoCached.Common;
using Mi.Common;
using Mi.Entity.Base.Security;
using Mi.MainDal.Databases;

namespace Mi.BoCached.CacheObjects
{
    public class AccountCached : CacheObjectBase
    {
        public UserEntity GetUserByUsername(string username)
        {
            var cachedKey = string.Format("AccountCached.GetUserByUsername[{0}]", username);
            var data = Get<UserEntity>(username, cachedKey);
            if (data == null || data.Id <= 0)
            {
                using (var db = new CmsMainDb())
                {
                    data = db.UserMainDal.GetUserByUsername(username);
                    if (null != data)
                    {
                        data.EncryptId = CryptonForId.EncryptId(data.Id);
                        data.Id = 0;
                    }
                    Add(username, cachedKey, data);
                }
                
            }
            return data;
        }
    }
}
