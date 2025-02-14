using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie() 
.AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    googleOptions.SaveTokens = true;
    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("email");

    // OnCreatingTicket is invoked after Google responds with user info
    googleOptions.Events.OnCreatingTicket = ctx =>
    {
        // Storing the Google ID token in the identity's claims
        var idToken = ctx.AccessToken;
        if (!string.IsNullOrEmpty(idToken))
        {
            ctx.Identity?.AddClaim(new Claim("id_token", idToken));
        }
        return Task.CompletedTask;
    };
});

builder.Services.AddSession();


builder.Services.AddHttpClient("EmployeeHubApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5049/");
});


builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute("default","{controller=Account}/{action=Login}/{id?}");

app.Run();
