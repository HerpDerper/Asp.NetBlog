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
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private IWebHostEnvironment _env;
        private readonly string[] permittedExtensios = new string[]
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif"
        };

        public PostsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int? categoryId, int pageNumber = 1)
        {
            var posts = await _context.Posts.ToListAsync();
            if (categoryId != null && categoryId != 0)
                posts = posts.Where(p => p.CategoryId == categoryId).ToList();
            int pageSize = 3;
            int count = posts.Count();
            var items = posts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
            List<Category> categories = _context.Categories.ToList();
            categories.Insert(0, new Category() { Id = 0, Name = "All categories" });
            PageViewModel paginator = new PageViewModel(count, pageNumber, pageSize);
            PostsViewModel viewModel = new PostsViewModel()
            {
                Posts = items,
                Paginator = paginator,
                Categories = new SelectList(categories, "Id", "Name")
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Content,PublishDate,PublishTime,ImagePath, CategoryId, UserName")] Post post, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    string name = uploadFile.FileName;
                    var ext = Path.GetExtension(name);
                    if (permittedExtensios.Contains(ext))
                    {
                        string path = $"/files/{name}";
                        string serverPath = _env.WebRootPath + path;
                        using (FileStream fs = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
                        {
                            await uploadFile.CopyToAsync(fs);
                        }
                        post.ImagePath = path;
                        post.UserName = User.Identity.Name;
                        _context.Posts.Add(post);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("ExtensionsError", "Errors");
                    }
                }
                else
                {
                    string name = "ROBOT.jpg";
                    var ext = Path.GetExtension(name);
                    if (permittedExtensios.Contains(ext))
                    {
                        string path = $"/files/{name}";
                        string serverPath = _env.WebRootPath + path;
                        post.ImagePath = path;
                        post.UserName = User.Identity.Name;
                        _context.Posts.Add(post);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("ExtensionsError", "Errors");
                    }
                }
            }
            var categories = _context.Categories.ToList();
            ViewData["categoryId"] = new SelectList(categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Content,PublishDate,PublishTime,ImagePath, CategoryId")] Post post, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    string name = uploadFile.FileName;
                    var ext = Path.GetExtension(name);
                    if (permittedExtensios.Contains(ext))
                    {
                        string path = $"/files/{name}";
                        string serverPath = _env.WebRootPath + path;
                        using (FileStream fs = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
                        {
                            await uploadFile.CopyToAsync(fs);
                        }
                        post.ImagePath = path;

                        _context.Update(post);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("ExtensionsError", "Errors");
                    }
                }
                else
                {
                    string name = "ROBOT.jpg";
                    var ext = Path.GetExtension(name);
                    if (permittedExtensios.Contains(ext))
                    {
                        string path = $"/files/{name}";
                        string serverPath = _env.WebRootPath + path;
                        post.ImagePath = path;

                        _context.Update(post);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("ExtensionsError", "Errors");
                    }
                }
            }
            var categories = _context.Categories.ToList();
            ViewData["categoryId"] = new SelectList(categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
