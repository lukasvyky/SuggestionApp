namespace SuggestionAppLibrary.Models;

public class BasicSuggestion
{
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; }
   public string SuggestionContent { get; set; }

   public BasicSuggestion()
   {
      
   }

   public BasicSuggestion(Suggestion suggestion)
   {
      Id = suggestion.Id;
      SuggestionContent = suggestion.SuggestionContent;
   }
}
