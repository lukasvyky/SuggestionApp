using SuggestionAppUI.ViewModels;

namespace SuggestionAppUI.Pages;

public partial class Create
{
   private CreateSuggestionViewModel suggestion = new();
   private List<Category> categories;
   private User loggedInUser;
   protected async override Task OnInitializedAsync()
   {
      categories = await categoryData.GetCategories();
      loggedInUser = await authProvider.GetUserFromAuth(userData);
   }

   private void ClosePage()
   {
      navManager.NavigateTo("/");
   }

   private async Task CreateSuggestion()
   {
      Suggestion s = new();
      s.SuggestionContent = suggestion.SuggestionContent;
      s.Description = suggestion.Description;
      s.Author = new BasicUser(loggedInUser);
      s.Category = categories.FirstOrDefault(c => c.Id == suggestion.CategoryId);
      await suggestionData.CreateSuggestion(s);
      suggestion = new();
      ClosePage();
   }
}