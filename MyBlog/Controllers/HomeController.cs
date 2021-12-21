using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.ViewModels;

namespace MyBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string title, string desc)
        {
            IQueryable<Post> users = _context.Posts;
            if (!String.IsNullOrEmpty(title)) users = users.Where(p => p.Title.Contains(title));
            if (!String.IsNullOrEmpty(desc)) users = users.Where(p => p.Description.Contains(desc));
            return View(await users.AsNoTracking().ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
