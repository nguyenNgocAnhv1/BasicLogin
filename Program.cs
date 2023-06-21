using App;
using App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
builder.Services.Configure<CookiePolicyOptions>(options =>
{
     options.Secure = CookieSecurePolicy.Always;
});
// addd session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
     o.IdleTimeout = TimeSpan.FromSeconds(60);
     o.Cookie.HttpOnly = true;
     o.Cookie.IsEssential = true;
});
// add mail
builder.Services.AddOptions();
builder.Services.Configure<MailSettings> (builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<ISendMailService, SendMailService>();
// google
builder.Services.AddAuthentication(option =>
{
     option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
     option.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

}).AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, option =>
{
     var gconfig = builder.Configuration.GetSection("Authentication:Google");
     option.ClientId = gconfig["ClientId"];
     option.ClientSecret = gconfig["ClientSecret"];
     option.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
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
app.UseAuthentication();
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.UseRouting();
// fix bug 
app.UseCookiePolicy(new CookiePolicyOptions()
{
     MinimumSameSitePolicy = SameSiteMode.Lax
});
app.MapGet("/testmail", async context => {

        // Lấy dịch vụ sendmailservice
        var sendmailservice = context.RequestServices.GetService<ISendMailService>();

        MailContent content = new MailContent {
            To = "keyhmast1@gmail.com",
            Subject = "Kiểm tra thử",
            Body = "<p><strong>Xin chào xuanthulab.net</strong></p>"
        };

        await sendmailservice.SendMail(content);
        await context.Response.WriteAsync("Send mail");
    });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
