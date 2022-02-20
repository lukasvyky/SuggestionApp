namespace SuggestionAppLibrary.Models;
public class User
{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; }
   public string ObjectIdentifier { get; set; }
   public string FirstName { get; set; }
   public string LastName { get; set; }
   public string DisplayName { get; set; }
   public string EmailAddress { get; set; }
   public List<BasicSuggestion> AuthoredSuggestions { get; set; } = new();
   public List<BasicSuggestion> VotedOnSuggestions { get; set; } = new();
}
