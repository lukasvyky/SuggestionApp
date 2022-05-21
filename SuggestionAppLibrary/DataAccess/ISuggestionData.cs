namespace SuggestionAppLibrary.DataAccess;

public interface ISuggestionData
{
   Task CreateSuggestion(Suggestion suggestion);
   Task<List<Suggestion>> GetAllSuggestionsWaitingForApproval();
   Task<List<Suggestion>> GetApprovedSuggestions();
   Task<Suggestion> GetSuggestion(string id);
   Task<List<Suggestion>> GetSuggestions();
   Task<List<Suggestion>> GetUsersSuggestions(string userId);
   Task UpdateSuggestion(Suggestion suggestion);
   Task UpvoteSuggestion(string suggestionId, string userId);
}