#region Import namespaces

using Microsoft.AspNetCore.Identity; // To use IdentityUser.
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore; // To use UseSqlServer method.
using Northwind.EntityModels; // To use AddNorthwindContext method.
using Northwind.Mvc.Data; // To use ApplicationDbContext.
using System.Net.Http.Headers; // To use MediaTypeWithQualityHeaderValue.

#endregion

#region Configure the host web server including services

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

string? sqlServerConnection = builder.Configuration.GetConnectionString("NorthwindConnection");

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString)); // Or UseSqlite.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


if (sqlServerConnection is null)
{
    Console.WriteLine("SQL Server database connection string is missing!");
}
else
{
    // If you are using SQL Server authentication then disable
    // Windows Integrated authentication and set user and password.
    SqlConnectionStringBuilder sql = new(sqlServerConnection)
    {
        IntegratedSecurity = true,
        //UserID = Environment.GetEnvironmentVariable("MY_SQL_USR"),
        //Password = Environment.GetEnvironmentVariable("MY_SQL_PWD")
    };

    builder.Services.AddNorthwindContext(sql.ConnectionString);
}

builder.Services.AddOutputCache(option =>
{
    option.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(20);
    option.AddPolicy("views", p => p.SetVaryByQuery("alterstyle"));
});

builder.Services.AddHttpClient(name: "Northwind.WebApi", configureClient: options =>
{
    options.BaseAddress = new Uri("https://localhost:5151/");
    options.DefaultRequestHeaders.Accept
    .Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json", quality: 1.0));
});

var app = builder.Build();

#endregion

#region Configure the HTTP request pipeline.

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseOutputCache();

app
    .MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//.CacheOutput(policyName: "views");

app.MapRazorPages();

app.MapGet("/notcached", () => DateTime.Now.ToString());
app.MapGet("/cached", () => DateTime.Now.ToString()).CacheOutput();

#endregion

#region Start the host web server listening for HTTP requests.

app.Run(); // This is a blocking call.

#endregion
