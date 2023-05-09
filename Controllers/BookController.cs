using AspNetCoreHero.ToastNotification.Abstractions;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }
        public BookController(DatabaseContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [Route("shop.html", Name = "ShopBook")]
        public async Task<IActionResult> Index(int page = 1, int? CategoryId = null, int? AuthorId = null, string searchString = "")
        {
            var pageNumber = page;
            var pageSize = 12;

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
                Text = c.Name,
               

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


            var categorys = _context.Categories.ToList();
            if (categorys != null)
            {
              
                ViewBag.Categories = categorys;      
               

            }

            var Categories = _context.Categories.ToList();
            var categoryBookCounts = new Dictionary<int, int>();
            foreach (var category in Categories)
            {
                var bookCount = _context.Books.Count(b => b.CategoryId == category.Id);
                categoryBookCounts.Add(category.Id, bookCount);
            }
            ViewData["CategoryBookCounts"] = categoryBookCounts;





            ViewBag.pageNumber = pageNumber;
            ViewBag.SearchString = searchString;
            return View(pagedBooks);
        }
       

        [Route("/{Name}-{id}.html", Name = "BookDetails")]

        public IActionResult Details( int id, int? CategoryId = null, int? AuthorId = null)
        {
            try
            {
                var book = _context.Books
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .FirstOrDefault(x => x.Id == id);
                var category = _context.Books.Where(b => b.CategoryId == book.CategoryId && b.Id != id).Take(4).ToList();
                ViewBag.Categories = category;                 
                if (book == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Book = book;
                    return View(book);
                }
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }



    }
}
