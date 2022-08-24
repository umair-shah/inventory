using Microsoft.EntityFrameworkCore;
using UnitTestExample.Web.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UnitTestExampleDbContext>(options =>
{
    //string connectionString = (string)builder.Configuration.GetValue(typeof(string), "ConnectionStrings:DefaultConnectionStrings");
    //string connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnectionStrings");
    string connectionString = builder.Configuration["ConnectionStrings:DefaultConnectionStrings"];
    options.UseSqlServer(connectionString);
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
