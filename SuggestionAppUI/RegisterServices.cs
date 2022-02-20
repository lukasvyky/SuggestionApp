namespace SuggestionAppUI;

public static class RegisterServices
{
   public static void ConfigureServices(this WebApplicationBuilder @this)
   {
      @this.Services.AddRazorPages();
      @this.Services.AddServerSideBlazor();
      @this.Services.AddMemoryCache();
   }
}
