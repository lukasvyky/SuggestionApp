namespace SuggestionAppLibrary.Models;

public class BasicUser
{
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; }
   public string DisplayName { get; set; }

   public BasicUser()
   {

   }

   public BasicUser(User user)
   {
      Id = user.Id;
      DisplayName = user.DisplayName;
   }
}
