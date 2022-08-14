using Microsoft.AspNetCore.Components;

namespace SuggestionAppUI.Pages;

 public partial class Details
 {
     [Parameter]
     public string Id { get; set; }

     private Suggestion suggestion;
     private User loggedInUser;
     private List<Status> statuses;
     private string settingStatus = string.Empty;
     private string urlText = string.Empty;
     protected async override Task OnInitializedAsync()
     {
         suggestion = await suggestionData.GetSuggestion(Id);
         loggedInUser = await authProvider.GetUserFromAuth(userData);
         statuses = await statusData.GetStatuses();
     }

     private async Task CompleteSetStatus()
     {
         switch (settingStatus)
         {
             case "completed":
                 if (string.IsNullOrWhiteSpace(urlText))
                 {
                     return;
                 }

                 suggestion.Status = statuses.Where(s => s.Name.ToLower() == settingStatus.ToLower()).First();
                 suggestion.AdminNotes = $"You are right, this is good. Check this out: <a href='{urlText}' target='_blank'>{urlText}></a>";
                 break;
             case "watching":
                 suggestion.Status = statuses.Where(s => s.Name.ToLower() == settingStatus.ToLower()).First();
                 suggestion.AdminNotes = $"Good idea. With enough points we may do it.";
                 break;
             case "upcoming":
                 suggestion.Status = statuses.Where(s => s.Name.ToLower() == settingStatus.ToLower()).First();
                 suggestion.AdminNotes = $"On it!";
                 break;
             case "dismissed":
                 suggestion.Status = statuses.Where(s => s.Name.ToLower() == settingStatus.ToLower()).First();
                 suggestion.AdminNotes = $"Not cool!";
                 break;
             default:
                 return;
         }

         settingStatus = null;
         await suggestionData.UpdateSuggestion(suggestion);
     }

     private void ClosePage()
     {
         navManager.NavigateTo("/");
     }

     private string GetUpvoteTopText()
     {
         if (suggestion.UserVotes?.Count > 0)
         {
             return suggestion.UserVotes.Count.ToString("00");
         }

         if (suggestion.Author.Id == loggedInUser?.Id)
         {
             return "Awaiting";
         }

         return "Click To";
     }

     private string GetUpvoteBottomText()
     {
         bool isPlural = suggestion.UserVotes?.Count > 1;
         return $"Upvote{(isPlural ? "s" : string.Empty)}";
     }

     private async Task VoteUp()
     {
         if (loggedInUser is null)
         {
             navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
         }
         else
         {
             if (suggestion.Author.Id == loggedInUser.Id)
             {
                 return;
             }

             if (!suggestion.UserVotes.Add(loggedInUser.Id))
             {
                 suggestion.UserVotes.Remove(loggedInUser.Id);
             }

             await suggestionData.UpvoteSuggestion(suggestion.Id, loggedInUser.Id);
         }
     }

     private string GetVoteClass()
     {
         if (suggestion.UserVotes is null || suggestion.UserVotes.Count == 0)
         {
             return "suggestion-detail-no-votes";
         }
         else if (suggestion.UserVotes.Contains(loggedInUser?.Id))
         {
             return "suggestion-detail-voted";
         }
         else
         {
             return "suggestion-detail-not-voted";
         }
     }

     private string GetStatusClass()
     {
         if (suggestion is null || suggestion.Status is null)
         {
             return "suggestion-detail-status-none";
         }

         string output = suggestion.Status.Name switch
         {
             "Completed" => "suggestion-detail-status-completed",
             "Watching" => "suggestion-detail-status-watching",
             "Upcoming" => "suggestion-detail-status-upcoming",
             "Dismissed" => "suggestion-detail-status-dismissed",
             _ => "suggestion-detail-status-none",
         };
         return output;
     }
 }