using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess;

public class MongoCategoryData : ICategoryData
{
   private readonly IMongoCollection<Category> _categories;
   private readonly IMemoryCache _cache;
   private const string cacheName = "CategoryData";

   public MongoCategoryData(IDbConnection db, IMemoryCache cache)
   {
      _categories = db.Categories;
      _cache = cache;
   }
   public async Task<List<Category>> GetCategories()
   {
      var result = _cache.Get<List<Category>>(cacheName);
      if (result is null)
      {
         var categories = await _categories.FindAsync(_ => true);
         result = categories.ToList();

         _cache.Set(cacheName, result, TimeSpan.FromDays(1));
      }

      return result;
   }

   public async Task CreateCategory(Category category)
   {
      await _categories.InsertOneAsync(category);
   }
}
