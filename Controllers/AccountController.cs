using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace App.Controllers
{
     public class AccountController : BaseController
     {
          private readonly AppDbContext _context;
          private readonly ISendMailService _sendmailservice;
          IOptions<MailSettings> _mailSettings;
          ILogger<SendMailService> _logger;
          public AccountController(AppDbContext context, ISendMailService sendmailservice, IOptions<MailSettings> mailSettings, ILogger<SendMailService> logger)
          {
               _context = context;
               _sendmailservice = sendmailservice;
                _mailSettings = mailSettings;
               _logger = logger;
          }
          public IActionResult Login()
          {
               return View();
          }
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Login(Account model)
          {
               if (ModelState.IsValid)
               {
                    var loginUser = _context.Accounts.ToList().FirstOrDefault(m => m.username == model.username);
                    if (loginUser == null)
                    {
                         ModelState.AddModelError("", "Đăng nhập thất bại");
                         return View(model);

                    }
                    else
                    {
                         SHA256 hashMethod = SHA256.Create();
                         if (App.Util.VerifyHash(hashMethod, model.password, loginUser.password))
                         {
                              CurrentUser = loginUser.username;
                              agc.a($"Login {CurrentUser}");
                              return RedirectToAction("Index", "Home");

                         }
                         else
                         {
                              ModelState.AddModelError("", "Login False");
                              return View(model);
                         }
                    }
               }
               return View(model);
          }
          public IActionResult Register()
          {
               return View();
          }
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Register(Account model)
          {
               if (ModelState.IsValid)
               {
                    SHA256 hashMethod = SHA256.Create();
                    model.password = App.Util.GetHash(hashMethod, model.password);
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Login));
               }
               return View(model);
          }
          public IActionResult Logout()
          {
               agc.a($"Logout {CurrentUser}");
               CurrentUser = "";
               return RedirectToAction("Login");
          }
          public async Task<IActionResult> SendMail()
          {
               // var sendmailservice = context.RequestServices.GetService<ISendMailService>();

              agc.a(_mailSettings.Value.DisplayName);
               SendMailService s = new SendMailService(_mailSettings, _logger);

               MailContent content = new MailContent
               {
                    To = "keyhmast1@gmail.com",
                    Subject = "Kiểm tra thử hay v",
                    Body = "<p><strong>Xin chào xuanthulab.net</strong></p>"
               };
               await s.SendMail(content);
               // await _sendmailservice.SendMail(content);
               return Content("Send mail");
          }
     }


}
