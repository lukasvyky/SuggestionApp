namespace SuggestionAppLibrary.DataAccess;

public class MongoUserData : IUserData
{
   private readonly IMongoCollection<User> _users;
   public MongoUserData(IDbConnection db)
   {
      _users = db.Users;
   }

   public async Task<List<User>> GetUsersAsync()
   {
      var result = await _users.FindAsync(_ => true);
      return result.ToList();
   }

   public async Task<User> GetUserAsync(string id)
   {
      var result = await _users.FindAsync(id);
      return result.FirstOrDefault();
   }

   public async Task<User> GetUserFromAuthenticationAsync(string objectId)
   {
      var result = await _users.FindAsync(u => u.ObjectIdentifier.Equals(objectId));
      return result.FirstOrDefault();
   }

   public async Task CreateUser(User user)
   {
      await _users.InsertOneAsync(user);
   }

   public async Task UpdateUser(User user)
   {
      var filter = Builders<User>.Filter.Eq("Id", user.Id);
      await _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
   }
}
