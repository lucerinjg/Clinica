using Clinic.Data;
using Clinic.Models.Auth;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var clientOptions = builder.Configuration.GetSection(ClientOptions.Client)
        .Get<ClientOptions>() ?? throw new InvalidCastException("'ClientOptions' not found");

builder.Services.Configure<ClientOptions>(builder.Configuration.GetSection(ClientOptions.Client));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Configure base Pages
builder.Services.AddRazorPages(options =>
{
    options.RootDirectory = "/Pages";
});

// Add services to the container.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
});

// Configure Identity Options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 4;
    // User settings
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Review time
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Patients", policy => policy.RequireRole("Patient"))
    .AddPolicy("Doctors", policy => policy.RequireRole("Doctor"))
    .AddPolicy("Administrator", policy => policy.RequireRole("Sudo"));


var defaultCulture = new CultureInfo("es-MX");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
CultureInfo[] supportedCultures = [defaultCulture];

// Configure App Builder
FileExtensionContentTypeProvider provider = new();
provider.Mappings[".webmanifest"] = "application/manifest+json";
provider.Mappings[".map"] = "application/json";

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(culture: "es-MX", uiCulture: "es-MX");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders =
    [
        new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
    ];
    options.SetDefaultCulture("es-MX");
});


var app = builder.Build();


// Add initial data to the database
InitialData.ExecuteSeeds(app.Services.CreateScope());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();

app.UseRequestLocalization(opt =>
{
    opt.AddSupportedCultures("es-MX");
    opt.AddSupportedUICultures("es-MX");
    opt.SetDefaultCulture("es-MX");
});

app.UseStaticFiles(new StaticFileOptions()
{
    ContentTypeProvider = provider
});

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
