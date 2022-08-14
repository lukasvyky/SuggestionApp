namespace SuggestionAppUI.Pages;

public partial class Profile
{
   private User loggedInUser;
   private List<Suggestion> submissions;
   private List<Suggestion> approved;
   private List<Suggestion> archived;
   private List<Suggestion> pending;
   private List<Suggestion> rejected;
   protected async override Task OnInitializedAsync()
   {
      loggedInUser = await authProvider.GetUserFromAuth(userData);
      var result = await suggestionData.GetUsersSuggestions(loggedInUser.Id);
      if (loggedInUser is not null && result is not null)
      {
         submissions = result.OrderByDescending(s => s.DateCreated).ToList();
         approved = submissions.Where(s => s.ApprovedForRelease && !s.Archived && !s.Rejected).ToList();
         archived = submissions.Where(s => s.Archived && !s.Rejected).ToList();
         pending = submissions.Where(s => !s.ApprovedForRelease && !s.Rejected).ToList();
         rejected = submissions.Where(s => s.Rejected).ToList();
      }
   }

   private void ClosePage()
   {
      navManager.NavigateTo("/");
   }
}