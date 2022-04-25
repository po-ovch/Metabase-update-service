using Microsoft.EntityFrameworkCore;
using Service_for_database.Models;

var builder = WebApplication.CreateBuilder(args);

DbConnectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContext<DatabaseContext>(options =>
{
	options.UseSqlServer(DbConnectionString);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Database}/{action=Index}/");

app.Run();

internal partial class Program
{
	public static string DbConnectionString = null!;
	public static string MetabaseHost = "https://localhost:7187";
}