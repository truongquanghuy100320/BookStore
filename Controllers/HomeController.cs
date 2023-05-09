using AspNetCoreHero.ToastNotification.Abstractions;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(ILogger<HomeController> logger, DatabaseContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var categorys = _context.Categories.ToList();
            if (categorys != null)
            {

                ViewBag.Categories = categorys;


            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}