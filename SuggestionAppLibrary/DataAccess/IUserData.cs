
namespace SuggestionAppLibrary.DataAccess;

public interface IUserData
{
   Task CreateUser(User user);
   Task<User> GetUserAsync(string id);
   Task<User> GetUserFromAuthenticationAsync(string objectId);
   Task<List<User>> GetUsersAsync();
   Task UpdateUser(User user);
}