namespace SuggestionAppUI.Pages;

public partial class Index
{
   private User loggedInUser;
   private List<Suggestion> suggestions;
   private List<Category> categories;
   private List<Status> statuses;
   private Suggestion archivingSuggestion;
   private string selectedCategory = "All";
   private string selectedStatus = "All";
   private string searchText = string.Empty;
   private bool isSortedByNew = true;
   private bool showCategories = false;
   private bool showStatuses = false;
   private void LoadCreatePage()
   {
      if (loggedInUser is not null)
      {
         navManager.NavigateTo("/Create");
      }

      navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
   }

   protected async override Task OnInitializedAsync()
   {
      categories = await categoryData.GetCategories();
      statuses = await statusData.GetStatuses();
      await LoadAndVerifyUser();
   }

   private async Task ArchiveSuggestion()
   {
      archivingSuggestion.Archived = true;
      await suggestionData.UpdateSuggestion(archivingSuggestion);
      suggestions.Remove(archivingSuggestion);
      archivingSuggestion = null;
      //await FilterSuggestions(); Not going to DB, cache will be refreshed after 1 min
   }

   private async Task LoadAndVerifyUser()
   {
      var authState = await authProvider.GetAuthenticationStateAsync();
      string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;
      if (!string.IsNullOrWhiteSpace(objectId))
      {
         loggedInUser = await userData.GetUserFromAuthentication(objectId) ?? new();
         string firstName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value;
         string lastName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value;
         string displayName = authState.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
         string email = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;
         bool isDirty = false;
         if (loggedInUser.ObjectIdentifier != objectId)
         {
            isDirty = true;
            loggedInUser.ObjectIdentifier = objectId;
         }

         if (loggedInUser.FirstName != firstName)
         {
            isDirty = true;
            loggedInUser.FirstName = firstName;
         }

         if (loggedInUser.LastName != lastName)
         {
            isDirty = true;
            loggedInUser.LastName = lastName;
         }

         if (loggedInUser.DisplayName != displayName)
         {
            isDirty = true;
            loggedInUser.DisplayName = displayName;
         }

         if (loggedInUser.EmailAddress != email)
         {
            isDirty = true;
            loggedInUser.EmailAddress = email;
         }

         if (isDirty)
         {
            if (string.IsNullOrWhiteSpace(loggedInUser.Id))
            {
               await userData.CreateUser(loggedInUser);
            }
            else
            {
               await userData.UpdateUser(loggedInUser);
            }
         }
      }
   }

   protected async override Task OnAfterRenderAsync(bool firstRender)
   {
      if (firstRender)
      {
         await LoadFilterState();
         await FilterSuggestions();
         StateHasChanged();
      }
   }

   private async Task LoadFilterState()
   {
      var stringResults = await sessionStorage.GetAsync<string>(nameof(selectedCategory));
      selectedCategory = stringResults.Success ? stringResults.Value : "All";
      stringResults = await sessionStorage.GetAsync<string>(nameof(selectedStatus));
      selectedStatus = stringResults.Success ? stringResults.Value : "All";
      stringResults = await sessionStorage.GetAsync<string>(nameof(searchText));
      searchText = stringResults.Success ? stringResults.Value : string.Empty;
      var boolResults = await sessionStorage.GetAsync<bool>(nameof(isSortedByNew));
      isSortedByNew = stringResults.Success ? boolResults.Value : true;
   }

   private async Task SaveFilterState()
   {
      await sessionStorage.SetAsync(nameof(selectedCategory), selectedCategory);
      await sessionStorage.SetAsync(nameof(selectedStatus), selectedStatus);
      await sessionStorage.SetAsync(nameof(searchText), searchText);
      await sessionStorage.SetAsync(nameof(isSortedByNew), isSortedByNew);
   }

   private async Task FilterSuggestions()
   {
      var output = await suggestionData.GetApprovedSuggestions();
      if (selectedCategory != "All")
      {
         output = output.Where(s => s.Category?.Name == selectedCategory).ToList();
      }

      if (selectedStatus != "All")
      {
         output = output.Where(s => s.Status?.Name == selectedStatus).ToList();
      }

      if (!String.IsNullOrWhiteSpace(searchText))
      {
         output = output.Where(s => s.SuggestionContent.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || s.Description.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
      }

      if (isSortedByNew)
      {
         output = output.OrderByDescending(s => s.DateCreated).ToList();
      }
      else
      {
         output = output.OrderByDescending(s => s.UserVotes.Count).ThenByDescending(s => s.DateCreated).ToList();
      }

      suggestions = output;
      await SaveFilterState();
   }

   private async Task OrderByNew()
   {
      isSortedByNew = !isSortedByNew;
      await FilterSuggestions();
   }

   private async Task OnSearchInput(string searchInput)
   {
      searchText = searchInput;
      await FilterSuggestions();
   }

   private async Task OnCategoryClick(string category = "All")
   {
      selectedCategory = category;
      showCategories = false;
      await FilterSuggestions();
   }

   private async Task OnStatusClick(string status = "All")
   {
      selectedStatus = status;
      showStatuses = false;
      await FilterSuggestions();
   }

   private async Task VoteUp(Suggestion suggestion)
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
         if (!isSortedByNew)
         {
            suggestions = suggestions.OrderByDescending(s => s.UserVotes.Count).ThenByDescending(s => s.DateCreated).ToList();
         }
      }
   }

   private string GetUpvoteTopText(Suggestion suggestion)
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

   private string GetUpvoteBottomText(Suggestion suggestion)
   {
      bool isPlural = suggestion.UserVotes?.Count > 1;
      return $"Upvote{(isPlural ? "s" : string.Empty)}";
   }

   private void OpenDetails(Suggestion suggestion)
   {
      navManager.NavigateTo($"details/{suggestion.Id}");
   }

   private string GetVoteClass(Suggestion suggestion)
   {
      if (suggestion.UserVotes is null || suggestion.UserVotes.Count == 0)
      {
         return "suggestion-entry-no-votes";
      }
      else if (suggestion.UserVotes.Contains(loggedInUser?.Id))
      {
         return "suggestion-entry-voted";
      }
      else
      {
         return "suggestion-entry-not-voted";
      }
   }

   private string GetSuggestionStatusClass(Suggestion suggestion)
   {
      if (suggestion is null || suggestion.Status is null)
      {
         return "suggestion-entry-status-none";
      }

      string output = suggestion.Status.Name switch
      {
         "Completed" => "suggestion-entry-status-completed",
         "Watching" => "suggestion-entry-status-watching",
         "Upcoming" => "suggestion-entry-status-upcoming",
         "Dismissed" => "suggestion-entry-status-dismissed",
         _ => "suggestion-entry-status-none",
      };
      return output;
   }

   private string GetSelectedCategory(string category = "All")
   {
      if (category == selectedCategory)
      {
         return "selected-category";
      }

      return string.Empty;
   }

   private string GetSelectedStatus(string status = "All")
   {
      if (status == selectedStatus)
      {
         return "selected-status";
      }

      return string.Empty;
   }
}