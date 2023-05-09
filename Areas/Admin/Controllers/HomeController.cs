using AspNetCoreHero.ToastNotification.Abstractions;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
     
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(DatabaseContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }


        public IActionResult Index()
        {

            var customers = _context.Customers.ToList();
            if (customers != null)
            {
                ViewBag.Customer = customers;
            }
            var orders = _context.Orders.ToList();
            if (orders != null)
            {
                
                ViewBag.Order = orders;    
            }

            var books = _context.Books.ToList();

            if (books != null)
            {
                ViewBag.Book = books;
            }

            DateTime today = DateTime.Today;
            var ordersToday = _context.Orders
                                  .Where(o => o.OrderDate >= today && o.OrderDate < today.AddDays(1))
                                  .ToList();

            int totalOrdersToday = ordersToday.Count;

            ViewData["HttpContext"] = HttpContext;
            return View();

        }

    }
}
