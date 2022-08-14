namespace SuggestionAppUI.Pages;

public partial class AdminApproval
{
   private List<Suggestion> submissions;
   private string currentEditingTitle = string.Empty;
   private string editedTitle = string.Empty;
   private string currentEditingDescription = string.Empty;
   private string editedDescription = string.Empty;
   protected async override Task OnInitializedAsync()
   {
      submissions = await suggestionData.GetAllSuggestionsWaitingForApproval();
   }

   private async Task ApproveSubmission(Suggestion submission)
   {
      submission.ApprovedForRelease = true;
      submissions.Remove(submission);
      await suggestionData.UpdateSuggestion(submission);
   }

   private async Task RejectSubmission(Suggestion submission)
   {
      submission.Rejected = true;
      submissions.Remove(submission);
      await suggestionData.UpdateSuggestion(submission);
   }

   private void EditTitle(Suggestion model)
   {
      editedTitle = model.SuggestionContent;
      currentEditingTitle = model.Id;
      currentEditingDescription = string.Empty;
   }

   private async Task SaveTitle(Suggestion model)
   {
      currentEditingTitle = string.Empty;
      model.SuggestionContent = editedTitle;
      await suggestionData.UpdateSuggestion(model);
   }

   private void EditDescription(Suggestion model)
   {
      editedDescription = model.Description;
      currentEditingTitle = string.Empty;
      currentEditingDescription = model.Id;
   }

   private async Task SaveDescription(Suggestion model)
   {
      currentEditingDescription = string.Empty;
      model.Description = editedDescription;
      await suggestionData.UpdateSuggestion(model);
   }

   private void ClosePage()
   {
      navManager.NavigateTo("/");
   }
}