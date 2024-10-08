using Eduhunt;
using Eduhunt.AppSettings;
using Eduhunt.Data;
using Eduhunt.Infrastructures.Cloud;
using Eduhunt.Infrastructures.Middleware;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No default connection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .Configure<IdentitySetting>(builder.Configuration.GetSection(IdentitySetting.IdentitySettingName));

builder.Services
    .Configure<CloudinarySetting>(builder.Configuration.GetSection(CloudinarySetting.CloudinarySettingName));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        var identitySettings = builder.Configuration.GetSection(IdentitySetting.IdentitySettingName).Get<IdentitySetting>();
        if (identitySettings != null)
        {
            options.SignIn.RequireConfirmedAccount = identitySettings.RequireConfirmedAccount;
            options.Password.RequireDigit = identitySettings.RequireDigit;
            options.Password.RequiredLength = identitySettings.RequiredLength;
            options.Password.RequireNonAlphanumeric = identitySettings.RequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.RequireUppercase;
            options.Password.RequireLowercase = identitySettings.RequireLowercase;
            options.Lockout.DefaultLockoutTimeSpan = identitySettings.DefaultLockoutTimeSpan;
            options.Lockout.MaxFailedAccessAttempts = identitySettings.MaxFailedAccessAttempts;
        }
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();


builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySetting"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<CloudinarySetting>>().Value);
builder.Services.AddTransient<CloudinaryService>();
// Call your extension method here to register custom services
builder.Services.AddAllCustomServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string role = "Student";
    if (!(await roleManager.RoleExistsAsync(role)))
    {
        await roleManager.CreateAsync(new IdentityRole("Student"));
        await roleManager.CreateAsync(new IdentityRole("Mentor"));
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseTokenValidation();
app.UseTokenRefresh();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
