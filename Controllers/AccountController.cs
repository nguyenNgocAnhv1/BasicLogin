using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace App.Controllers
{
     public class AccountController : BaseController
     {
          private readonly AppDbContext _context;
          public AccountController(AppDbContext context)
          {
               _context = context;
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
     }

}
