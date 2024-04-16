﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApplication.Web.Data;
using MyApplication.Web.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MyApplication.Web.Controllers
{


    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        //------------------------------------------------

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            bool userNameExists = await _context.Users.AnyAsync(u => u.UserName == model.UserName);
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);

            if (userNameExists || emailExists)
            {
                if (userNameExists)
                {
                    ModelState.AddModelError("UserName", "Bu kullanıcı adı zaten kullanımda.");
                }
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanımda.");
                }
                return View("Register");
            }

            var user = new User { UserName = model.UserName, Email = model.Email, Password = model.Password };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Profile()
        {
            var model = new User();
            var userName = HttpContext.Session.GetString("UserName");//
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            model = user ?? new User();

            return View(model);
        }

        public IActionResult EditProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName && u.Password == model.Password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.UserName);

                return View("Profile");
            }
            return View("Index");
        }


        public IActionResult Logout() //"Profile" de koyucam.
        {
            return RedirectToAction("Index");
        }

    }
}
