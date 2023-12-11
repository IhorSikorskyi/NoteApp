using Microsoft.Extensions.Caching.Memory;

namespace NoteApp;

public class Cache
{
    private readonly Context _dbContext;
    private readonly IMemoryCache _memoryCache;

    public Cache(Context dbContext, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
    }

    public void CacheUserId()
    {
        _memoryCache.Set<int>("cached_user_id", Authentication.GetCachedUserId(), TimeSpan.FromHours(1.0));
    }
    public void DeleteCachedUserId()
    {
        _memoryCache.Remove("cached_user_id");
    }

    public List<Category> GetDataFromDatabase()
    {
        using (var context = new Context())
        {
            int? cachedUserId = _memoryCache.Get<int?>("cached_user_id");
            if (cachedUserId.HasValue)
            {
                return context.Categories.Where(c => c.userid == cachedUserId.Value)
                    .Select(item => new Category { categoryid = item.categoryid, name = item.name })
                    .ToList();
            }
        }
        return null;
    }

    public int GetCachedUserId()
    {
        return _memoryCache.Get<int>("cached_user_id");
    }
}