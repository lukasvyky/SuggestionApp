namespace SuggestionAppUI.Components;
using Microsoft.AspNetCore.Components.Forms;

public class CustomInputRadioGroup<TValue> : InputRadioGroup<TValue>
{
   private string _name;
   private string _fieldClass;

   protected override void OnParametersSet()
   {
      var fieldClass = EditContext?.FieldCssClass(FieldIdentifier) ?? string.Empty; 
      if (fieldClass != _fieldClass || Name != _name)
      {
         _fieldClass = fieldClass;
         _name = Name;
         base.OnParametersSet();
      }
   }
}
