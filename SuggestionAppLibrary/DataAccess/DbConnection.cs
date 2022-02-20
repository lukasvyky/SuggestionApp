using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess;

public class DbConnection : IDbConnection
{
   private readonly IConfiguration _config;
   private readonly IMongoDatabase _db;
   private string _connectionId = "MongoDB";
   public string DbName { get; private set; }

   public MongoClient Client { get; private set; }

   public IMongoCollection<Category> Categories { get; private set; }
   public IMongoCollection<Status> Statuses { get; private set; }
   public IMongoCollection<User> Users { get; private set; }
   public IMongoCollection<Suggestion> Suggestions { get; private set; }


   public string CategoryCollectionName { get; private set; } = "categories";
   public string StatusCollectionName { get; private set; } = "statuses";
   public string UserCollectionName { get; private set; } = "users";
   public string SuggestionCollectionName { get; private set; } = "suggestions";


   public DbConnection(IConfiguration config)
   {
      _config = config;
      Client = new MongoClient(_config.GetConnectionString(_connectionId));
      DbName = _config["DatabaseName"];
      _db = Client.GetDatabase(DbName);

      Categories = _db.GetCollection<Category>(CategoryCollectionName);
      Statuses = _db.GetCollection<Status>(StatusCollectionName);
      Users = _db.GetCollection<User>(UserCollectionName);
      Suggestions = _db.GetCollection<Suggestion>(SuggestionCollectionName);
   }
}