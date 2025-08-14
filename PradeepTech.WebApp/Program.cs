using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PradeepTech.Domain.Context;
using PradeepTech.Domain.Models;
using PradeepTech.WebApp.Services;
using WebEssentials.AspNetCore.Pwa;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddProgressiveWebApp(new PwaOptions
{
    RegisterServiceWorker = true,
    RegisterWebmanifest = false,  // (Manually register in Layout file)
    Strategy = ServiceWorkerStrategy.Minimal,
    OfflineRoute = "Offline.html"
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    //options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddMvc();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDataAccessService();
builder.Services.AddHealthChecks();
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var detector = new DeviceDetector(context.Request.Headers["User-Agent"].ToString());
    detector.SetCache(new DictionaryCache());
    detector.Parse();

    if (detector.IsMobile())
    {
        context.Items.Remove("isMobile");
        context.Items.Add("isMobile", true);
    }
    else
    {
        context.Items.Remove("isMobile");
        context.Items.Add("isMobile", false);
    }

    context.Items.Remove("DeviceName");
    context.Items.Add("DeviceName", detector.GetDeviceName());

    await next();
});
app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(name: "User", areaName: "User", pattern: "User/{controller=User}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(name: "Home", pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapAreaControllerRoute(name: "default", areaName: "Identity", pattern: "Identity/{action=Login}/{id?}");
    endpoints.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );

    //endpoints.MapGet("/", context =>
    //{
    //    return Task.Run(() => context.Response.Redirect("/Account/Login"));
    //});

    endpoints.MapRazorPages();
});

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapAreaControllerRoute(name: "Admin", areaName: "Admin", pattern: "Admin/{controller=Admin}/{action=Index}/{id?}");
//    endpoints.MapAreaControllerRoute(name: "Reports", areaName: "Reports", pattern: "Reports/{controller=Admin}/{action=Index}/{id?}");
//    endpoints.MapAreaControllerRoute(name: "Masters", areaName: "Masters", pattern: "Masters/{controller=Masters}/{action=Index}/{id?}");
//    endpoints.MapAreaControllerRoute(name: "SystemAdmin", areaName: "SystemAdmin", pattern: "SystemAdmin/{controller=SystemAdmin}/{action=Index}/{id?}");
//    endpoints.MapAreaControllerRoute(name: "Users", areaName: "Users", pattern: "Users/{controller=Users}/{action=Index}/{id?}");
//    endpoints.MapAreaControllerRoute(name: "ServiceRequests", areaName: "ServiceRequests", pattern: "ServiceRequests/{controller=ServiceRequests}/{action=Index}/{id?}");
//    endpoints.MapAreaControllerRoute(name: "MobileApp", areaName: "MobileApp", pattern: "MobileApp/{controller=MobileApp}/{action=Index}/{id?}");

//    endpoints.MapControllerRoute(name: "Home", pattern: "{controller=Home}/{action=CMRIndex}/{id?}");
//    endpoints.MapAreaControllerRoute(name: "default", areaName: "Identity", pattern: "Identity/{action=Login}/{id?}");
//    endpoints.MapControllerRoute(
//            name: "areas",
//            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
//          );
//    endpoints.MapGet("/", context =>
//    {
//        return Task.Run(() => context.Response.Redirect("/Account/Login"));
//    });
//    endpoints.MapRazorPages();
//});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

app.Run();