using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using PagedList.Core;
using PagedList.Core.Mvc;
using X.PagedList;
using BookStore.Helpper;
using System.Reflection.Metadata;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }

        public BooksController(DatabaseContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }


        // GET: Admin/Books
        public async Task<IActionResult> Index(int page = 1, int? CategoryId = null , int? AuthorId = null, string searchString = "")
        {
            var pageNumber = page;
            var pageSize = 8;



            IQueryable<Book> books = _context.Books.AsQueryable();

            if (CategoryId.HasValue)
            {
                books = books.Where(x => x.CategoryId == CategoryId.Value);
            }
            if (AuthorId.HasValue)
            {
                books = books.Where(x => x.AuthorId == AuthorId.Value);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(x => x.Name.ToLower().Contains(searchString.ToLower()));
            }

            var pagedBooks = await books.ToPagedListAsync(pageNumber, pageSize);


            var categoryList = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            ViewBag.CategoryId = CategoryId;
            ViewBag.CategoryList = new SelectList(categoryList, "Value", "Text", CategoryId);

            var authorList = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();
            ViewBag.AuthorId = null;
            ViewBag.AuthorList = new SelectList(authorList, "Value", "Text", AuthorId);
                ViewBag.pageNumber = pageNumber;
            ViewBag.SearchString = searchString;
            return View(pagedBooks);
        }


        [HttpPost]
        public IActionResult Filter(int CategoryId = 0, string NameCate = null)
        {
            var url = $"/Admin/Books?CategoryId={CategoryId}&NameCate={NameCate}";
            if (CategoryId == 0)
            {
                url = "/Admin/Books";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        [HttpPost]
        public IActionResult Filter2(int AuthorId = 0, string MameAuthor = null)
        {
            var url = $"/Admin/Books?AuthorId={AuthorId}&NameAuthor={MameAuthor}";
            if (AuthorId == 0)
            {
                url = "/Admin/Books";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        public async Task<IActionResult> Search(string searchString = "", int page = 1)
        {
            try
            {
                var pageNumber = page;
                var pageSize = 8;

                IQueryable<Book> books = _context.Books.Include(b => b.Author).Include(b => b.Category).AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    books = books.Where(b => b.Name.ToLower().Contains(searchString.ToLower()));
                }


                var pagedBooks = await books.ToPagedListAsync(pageNumber, pageSize);

                var categoryList = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                ViewBag.CategoryId = null;
                ViewBag.CategoryList = new SelectList(categoryList, "Value", "Text");

                var authorList = _context.Authors.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();
                ViewBag.AuthorId = null;
                ViewBag.AuthorList = new SelectList(authorList, "Value", "Text");
                if (pagedBooks == null || !pagedBooks.Any())
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.pageNumber = pageNumber;
                    return View("Index", pagedBooks);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // GET: Admin/Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Admin/Books/Create
        public IActionResult Create(int? Id = null, int? Id2 = null)
        {
            // Lấy danh sách category
            var categoryList = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            ViewBag.Id = Id2;
            ViewBag.CategoryList = new SelectList(categoryList, "Value", "Text", Id2);

            // Lấy danh sách author
            var authorList = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();
            ViewBag.Id = Id;
            ViewBag.AuthorList = new SelectList(authorList, "Value", "Text",Id);




            return View();
        }

        // POST: Admin/Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Title,Description,Price,AuthorId,NameAuthor,Image,CategoryId,NameCate,Publisher,Quantity,Language,PublicationDate,IllustrationsNote,Isbn10,Isbn13,Created,Status")] Book book, IFormFile fImage)
        {
            if (ModelState.IsValid)
            {
                book.Title = Utilities.ToTitleCase(book.Title);
                if (fImage != null)
                {
                    string extension = Path.GetExtension(fImage.FileName);
                    string fileName = Guid.NewGuid().ToString() + extension;
                    book.Image = await Utilities.UploadFile(fImage, @"tours", fileName);
                }

                if (string.IsNullOrEmpty(book.Image))
                {
                    book.Image = "default.jpg";
                }

                // Thêm thuộc tính Tiltle vào object tourDetail
                book.Title = Utilities.ToTitleCase(book.Title);
                book.Created = DateTime.Now;

                _context.Add(book);
                await _context.SaveChangesAsync();
                _notyfService.Success("Successfully added new");
                return RedirectToAction(nameof(Index));
            }
            // Lấy danh sách category
            var categoryList = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            ViewBag.Id = book.CategoryId;
            ViewBag.CategoryList = new SelectList(categoryList, "Value", "Text", book.CategoryId);

            // Lấy danh sách author
            var authorList = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();
            ViewBag.Id = book.AuthorId;
            ViewBag.AuthorList = new SelectList(authorList, "Value", "Text", book.AuthorId);



            return View(book);
        }

        // GET: Admin/Books/Edit/5
        public async Task<IActionResult> Edit(int? id, int? Id2 = null, int? Id = null)
        {
           

            var book = await _context.Books.FindAsync(id);
            if (id == null)
            {
                return NotFound();
            }
            var categoryList = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            ViewBag.Id = Id2;
            ViewBag.CategoryList = new SelectList(categoryList, "Value", "Text", Id2);

            // Lấy danh sách author
            var authorList = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();
            ViewBag.Id = Id;
            ViewBag.AuthorList = new SelectList(authorList, "Value", "Text", Id);
            return View(book);
        }

        // POST: Admin/Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Title,Description,Price,AuthorId,NameAuthor,Image,CategoryId,NameCate,Publisher,Quantity,Language,PublicationDate,IllustrationsNote,Isbn10,Isbn13,Created,Status")] Book book, IFormFile fImage)
        {
            if (id != book.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                book.Title = Utilities.ToTitleCase(book.Title);

                if (fImage != null)
                {
                    string extension = Path.GetExtension(fImage.FileName);
                    string fileName = Guid.NewGuid().ToString() + extension;
                    book.Image = await Utilities.UploadFile(fImage, @"tours", fileName);
                }
                else if (string.IsNullOrWhiteSpace(book.Image)) // nếu không upload ảnh mới và book.Image là null hoặc chuỗi rỗng
                {
                    book.Image = "default.jpg"; // giữ nguyên ảnh cũ
                }


                book.Title = Utilities.ToTitleCase(book.Title);
                book.Created = DateTime.Now;

                _context.Update(book);
                await _context.SaveChangesAsync();
                _notyfService.Success("Successfully updated ");
                return RedirectToAction(nameof(Index));
            }

            var categoryList = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            ViewBag.Id = book.CategoryId;
            ViewBag.CategoryList = new SelectList(categoryList, "Value", "Text", book.CategoryId);

            // Lấy danh sách author
            var authorList = _context.Authors.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();
            ViewBag.Id = book.AuthorId;
            ViewBag.AuthorList = new SelectList(authorList, "Value", "Text", book.AuthorId);

            return View(book);
        }

        // GET: Admin/Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Admin/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'DatabaseContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            _notyfService.Success("Delete successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
