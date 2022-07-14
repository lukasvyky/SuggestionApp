using SuggestionAppUI;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
   app.UseExceptionHandler("/Error");
   app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRewriter(new RewriteOptions().Add(
   context =>
   {
      if (context.HttpContext.Request.Path == "/MicrosoftIdentity/Account/Signedout")
      {
         context.HttpContext.Response.Redirect("/");
      }
   }));

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();