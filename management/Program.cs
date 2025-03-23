using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using management.Models;
using management;
using management.Interface;
using management.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ManagementDb")));

// Add Identity and configure it to use ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()  // Make sure you're linking to the correct DbContext
    .AddDefaultTokenProviders();

// Register services for business logic (Person, Account, and Transaction services)
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Add authentication services (including Google Authentication)
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

// Configure application cookies for login redirect
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";  // Ensure this matches the route for the Login action
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.Cookie.MaxAge = TimeSpan.FromDays(14); // Set cookie expiration time to 14 days
});

// Add services to the container for MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Security feature for HTTPS redirection in production
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ensure Authentication and Authorization middlewares are added
app.UseAuthentication();  // Handles user authentication
app.UseAuthorization();   // Handles authorization (checking if user is authorized to access resources)

// Default route for all controllers (including AuthController)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();