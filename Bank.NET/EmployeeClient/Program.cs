using EmployeeClient.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Logging;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var portAuthService = Environment.GetEnvironmentVariable("PortUsersService");
var ipAuthService = Environment.GetEnvironmentVariable("IpUsersService");


builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options => {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies", options => {
        options.Cookie.SameSite = SameSiteMode.Strict;
        // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect("oidc", options => {
        options.Authority = $"http://{ipAuthService}:{portAuthService}";
        options.ClientId = "EmployeeClient";
        options.ResponseType = "code";
        options.RequireHttpsMetadata = false;
        options.SaveTokens = true;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add(ClaimTypes.Role);
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Role, "role");

        // options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.NonceCookie.SameSite = SameSiteMode.Strict;
        // options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.CorrelationCookie.SameSite = SameSiteMode.Strict;
    });


builder.Services.AddAuthorization(options => {
    options.AddPolicy("Employee", policy => { policy.RequireClaim(ClaimTypes.Role, "Employee"); });
});

builder.Services.AddSignalR();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IClientService, ClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
    IdentityModelEventSource.ShowPII = true;
} else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
    IdentityModelEventSource.ShowPII = true;
}


var webSocketOptions = new WebSocketOptions {
    KeepAliveInterval = TimeSpan.FromMinutes(1)
};
app.UseWebSockets(webSocketOptions);


//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints => {
    endpoints.MapDefaultControllerRoute()
        .RequireAuthorization();
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}");


app.Run();