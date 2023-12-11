using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace NoteApp
{
    public class Cache
    {
        private readonly Context _dbContext;
        private readonly IMemoryCache _cache;

        public Cache(Context dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public void CacheUserId()
        {
            int userID = Authentication.GetCachedUserId();

            _cache.Set("cached_user_id", userID, TimeSpan.FromHours(10.0));
        }
        public void DeleteCachedUserId()
        {
            _cache.Remove("cached_user_id");
        }

        public List<Category> GetDataFromDatabase()
        {
            using (var context = new Context())
            {
                int cachedUserId = _cache.Get<int>("cached_user_id");
                if (cachedUserId != null)
                {
                    return context.Categories.Where(c => c.userid == cachedUserId)
                        .Select(item => new Category { categoryid = item.categoryid, name = item.name })
                        .ToList();
                }
            }
            return null;
        }

        public int GetCachedUserId()
        {
            return _cache.Get<int>("cached_user_id");
        }
    }
}