using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmtpSender.Services.Configurations;
using UsersService.Models;
using UsersService.Services.Configuration;
using UsersService.Services.Configurations;
using UsersService.Services;
using UsersService.Data;
using Microsoft.AspNetCore.Authorization;
using Google.Api;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DbConnectionStringForUsersService");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddIdentity<User, Role>(option => {
        option.SignIn.RequireConfirmedAccount = false;
        option.Password.RequireDigit = false;
        option.Password.RequireLowercase = false;
        option.Password.RequireNonAlphanumeric = false;
        option.Password.RequireUppercase = false;
        option.Password.RequiredLength = 0;
    })
    .AddSignInManager<SignInManager<User>>()
    .AddUserManager<UserManager<User>>()
    .AddRoleManager<RoleManager<Role>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddIdentityServer(
		options =>
		{
			options.Authentication.CookieSameSiteMode = SameSiteMode.Strict;
		}
    )
    .AddAspNetIdentity<User>()
    .AddDeveloperSigningCredential()
    .AddInMemoryApiResources(IdentityServerConfiguration.ApiResources)
    .AddInMemoryIdentityResources(IdentityServerConfiguration.IdentityResources)
    .AddInMemoryApiScopes(IdentityServerConfiguration.ApiScope)
    .AddInMemoryClients(IdentityServerConfiguration.Clients);


builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.Name = "Bank.Identity.Cookie";
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.Cookie.SameSite = SameSiteMode.Strict;
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7115";
        options.RequireHttpsMetadata = true;
        options.Audience = "api name";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });*/

builder.Services.AddGrpc();

var app = builder.Build();
await app.ConfigureIdentityAsync();

app.UseAuthentication();
//app.UseAuthorization();
app.UseIdentityServer();
//app.UseHttpsRedirection();
app.UseStaticFiles();


// Configure the HTTP request pipeline.
app.MapGrpcService<UserService>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();