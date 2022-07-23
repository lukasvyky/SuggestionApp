using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace SuggestionAppUI;

public static class RegisterServices
{
   public static void ConfigureServices(this WebApplicationBuilder @this)
   {
      @this.Services.AddRazorPages();
      @this.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
      @this.Services.AddMemoryCache();
      @this.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

      @this.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
         .AddMicrosoftIdentityWebApp(@this.Configuration.GetSection("AzureAdB2C"));

      @this.Services.AddAuthorization(options =>
      {
         options.AddPolicy("Admin", policy =>
         {
            policy.RequireClaim("jobTitle", "Admin");
         });
      });

      @this.Services.AddSingleton<IDbConnection, DbConnection>();
      @this.Services.AddSingleton<ICategoryData, MongoCategoryData>();
      @this.Services.AddSingleton<IUserData, MongoUserData>();
      @this.Services.AddSingleton<IStatusData, MongoStatusData>();
      @this.Services.AddSingleton<ISuggestionData, MongoSuggestionData>();
   }
}
