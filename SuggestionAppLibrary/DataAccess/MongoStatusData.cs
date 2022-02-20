using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess;

public class MongoStatusData : IStatusData
{
   private readonly IMongoCollection<Status> _statuses;
   private readonly IMemoryCache _cache;
   private const string CacheName = "StatusData";
   public MongoStatusData(IDbConnection db, IMemoryCache cache)
   {
      _statuses = db.Statuses;
      _cache = cache;
   }

   public async Task<List<Status>> GetStatuses()
   {
      var result = _cache.Get<List<Status>>(CacheName);
      if (result is null)
      {
         var statuses = await _statuses.FindAsync(_ => true);
         result = statuses.ToList();

         _cache.Set(CacheName, result, TimeSpan.FromDays(1));
      }
      return result;
   }

   public async Task CreateStatus(Status status)
   {
      await _statuses.InsertOneAsync(status);
   }
}