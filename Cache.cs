using Microsoft.Extensions.Caching.Memory;
using StockSharp.Messages;

namespace NoteApp
{
    public class Cache(IMemoryCache cache)
    {
        public void CacheUserId()
        {
            int userID = Authentication.GetCachedUserId();

            cache.Set("cached_user_id", userID, TimeSpan.FromHours(10.0));
        }
        public void DeleteCachedUserId()
        {
            cache.Remove("cached_user_id");
        }

        public List<Category> GetDataFromDatabase()
        {
            using var context = new Context();

            int cachedId = GetCachedUserId();

            if (cachedId != 0)
            {
                var categories = context.Categories
                    .Where(c => c.userid == cachedId)
                    .Select(item => new Category
                    {
                        categoryid = item.categoryid,
                        name = item.name
                    }).ToList();
                return categories;
            }

            return new List<Category>();
        }

        public int GetCachedUserId()
        {
            return cache.Get<int>("cached_user_id");
        }
    }
}