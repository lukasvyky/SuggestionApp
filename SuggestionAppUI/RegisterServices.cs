namespace SuggestionAppUI;

public static class RegisterServices
{
   public static void ConfigureServices(this WebApplicationBuilder @this)
   {
      @this.Services.AddRazorPages();
      @this.Services.AddServerSideBlazor();
      @this.Services.AddMemoryCache();

      @this.Services.AddSingleton<IDbConnection, DbConnection>();
      @this.Services.AddSingleton<ICategoryData, MongoCategoryData>();
      @this.Services.AddSingleton<IUserData, MongoUserData>();
      @this.Services.AddSingleton<IStatusData, MongoStatusData>();
      @this.Services.AddSingleton<ISuggestionData, MongoSuggestionData>();
   }
}
