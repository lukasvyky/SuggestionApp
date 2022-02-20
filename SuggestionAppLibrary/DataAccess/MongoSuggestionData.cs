using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess;

public class MongoSuggestionData : ISuggestionData
{
   private readonly IMongoCollection<Suggestion> _suggestions;
   private readonly IDbConnection _db;
   private readonly IUserData _userData;
   private readonly IMemoryCache _cache;
   private const string CacheName = "SuggestionsData";

   public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
   {
      _db = db;
      _userData = userData;
      _cache = cache;
      _suggestions = db.Suggestions;
   }

   public async Task<List<Suggestion>> GetSuggestions()
   {
      var result = _cache.Get<List<Suggestion>>(CacheName);
      if (result is null)
      {
         var suggestions = await _suggestions.FindAsync(s => !s.Archived);
         result = suggestions.ToList();

         _cache.Set(CacheName, result, TimeSpan.FromMinutes(1));
      }

      return result;
   }

   public async Task<List<Suggestion>> GetApprovedSuggestions()
   {
      var result = await GetSuggestions();
      return result.Where(s => s.ApprovedForRelease).ToList();
   }

   public async Task<Suggestion> GetSuggestion(string id)
   {
      var result = await _suggestions.FindAsync(s => s.Id.Equals(id));
      return result.FirstOrDefault();
   }

   public async Task<List<Suggestion>> GetAllSuggestionsWaitingForApproval()
   {
      var result = await GetSuggestions();
      return result.Where(s => !s.ApprovedForRelease && !s.Rejected).ToList();
   }

   public async Task UpdateSuggestion(Suggestion suggestion)
   {
      await _suggestions.ReplaceOneAsync(s => s.Id.Equals(suggestion.Id), suggestion);
      _cache.Remove(CacheName);
   }

   public async Task UpvoteSuggestion(string suggestionId, string userId)
   {
      var client = _db.Client;

      using var session = await client.StartSessionAsync();

      session.StartTransaction();

      try
      {
         var db = client.GetDatabase(_db.DbName);
         var suggestionsInTransaction = db.GetCollection<Suggestion>(_db.SuggestionCollectionName);
         var suggestion = (await suggestionsInTransaction.FindAsync(s => s.Id.Equals(suggestionId))).First();

         var isUpvote = suggestion.UserVotes.Add(userId);
         if (!isUpvote)
         {
            suggestion.UserVotes.Remove(userId);
         }

         await suggestionsInTransaction.ReplaceOneAsync(s => s.Id.Equals(suggestionId), suggestion);

         var usersInTransaction = db.GetCollection<User>(_db.UserCollectionName);
         var user = await _userData.GetUser(suggestion.Author.Id);

         if (isUpvote)
         {
            user.VotedOnSuggestions.Add(new BasicSuggestion(suggestion));
         }
         else
         {
            var suggestionToRemove = user.VotedOnSuggestions.First(s => s.Id.Equals(suggestionId));
            user.VotedOnSuggestions.Remove(suggestionToRemove);
         }

         await usersInTransaction.ReplaceOneAsync(u => u.Id.Equals(userId), user);
         await session.CommitTransactionAsync();

         _cache.Remove(CacheName);
      }
      catch (Exception ex)
      {
         await session.AbortTransactionAsync();
         throw;
      }
   }

   public async Task CreateSuggestion(Suggestion suggestion)
   {
      var client = _db.Client;
      using var session = await client.StartSessionAsync();

      session.StartTransaction();

      try
      {
         var db = client.GetDatabase(_db.DbName);
         var suggestionsInTransaction = db.GetCollection<Suggestion>(_db.SuggestionCollectionName);
         await suggestionsInTransaction.InsertOneAsync(suggestion);

         var usersInTransaction = db.GetCollection<User>(_db.UserCollectionName);
         var user = await _userData.GetUser(suggestion.Author.Id);
         user.AuthoredSuggestions.Add(new BasicSuggestion(suggestion));
         await usersInTransaction.ReplaceOneAsync(u => u.Id.Equals(user.Id), user);

         await session.CommitTransactionAsync();
      }
      catch (Exception)
      {
         await session.AbortTransactionAsync();
         throw;
      }
   }
}
