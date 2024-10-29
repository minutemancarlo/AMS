using MudBlazor.Services;
using AMS.Web.Components;
using AMS.Web.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using Microsoft.Data.SqlClient;
using System.Data;
using AMS.Data.Repositories.Authentication;
using Blazor.SubtleCrypto;
using AMS.Data.Utilities;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
if (!string.IsNullOrWhiteSpace(environment.EnvironmentName))
{
    builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", false, true);
}
else
{
    builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
       .AddJsonFile($"appsettings.Production.json", false, true);
}

// Register your database connection
builder.Services.AddTransient<IDbConnection>(sp =>
{
    IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
    string connectionString = configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});

//Date Time Helper
builder.Services.AddTransient<DateTimeHelper>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var timeZoneId = configuration["TimeZone"];
    return new DateTimeHelper(timeZoneId);
});


// Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomEnd;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.SnackbarConfiguration.BackgroundBlurred = true;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "CustomScheme";  // You can customize this scheme name
    options.DefaultChallengeScheme = "CustomScheme";    // For example, use cookies or a custom scheme
}).AddCookie("CustomScheme", options =>
{
    options.LoginPath = "/login";  // Define your login path if using cookies
    options.LogoutPath = "/logout";  // Define your logout path
});



builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<UserAccountService>();

builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddSubtleCrypto(opt =>
    opt.Key = "tz31eOWLi0JwhjKz8hiSHvx7BXUhWyIFLGUnDNw7MG24hy-bniDwXrvoF4XyyYE5Gjzp18zQ0jRUpnN2gaQ8OQ"
);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
