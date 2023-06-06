using App.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCors();
// thiet lapdich vu sql
var connectString = builder.Configuration.GetConnectionString("AppDb"); // chuuoi ket noi
builder.Services.AddDbContext<AppDbContext>(o =>
{
    o.UseSqlServer(connectString).LogTo(Console.WriteLine, LogLevel.None);
});
// addd session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession( o =>
{
    o.IdleTimeout= TimeSpan.FromSeconds(60);
    o.Cookie.HttpOnly= true;
    o.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Enable Cors
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseCors("AllowAnyCorsPolicy");
// khai bao su dung session
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
