using System.ComponentModel.DataAnnotations;

namespace SuggestionAppUI.ViewModels;

public class CreateSuggestionViewModel
{
   [Required]
   [MaxLength(75)]
   public string SuggestionContent { get; set; }

   [Required]
   [MinLength(1)]
   [Display(Name = "Category")]
   public string CategoryId { get; set; }

   [MaxLength(500)]
   public string Description { get; set; }
}
