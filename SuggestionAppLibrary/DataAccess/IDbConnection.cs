using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess;

public interface IDbConnection
{
   IMongoCollection<Category> Categories { get; }
   string CategoryCollectionName { get; }
   MongoClient Client { get; }
   string DbName { get; }
   string StatusCollectionName { get; }
   IMongoCollection<Status> Statuses { get; }
   string SuggestionCollectionName { get; }
   IMongoCollection<Suggestion> Suggestions { get; }
   string UserCollectionName { get; }
   IMongoCollection<User> Users { get; }
}