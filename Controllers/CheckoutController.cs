using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Extension;
using BookStore.Helpper;
using BookStore.Models;
using BookStore.ModelViews;
using BookStore.Extention;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }
        public CheckoutController(DatabaseContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [Route("checkout.html",Name ="Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
               // model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
              
            }
            var peyment = _context.Payments.ToList();
            if (peyment != null)
            {

                ViewBag.Payments = peyment;


            }
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name ="Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            //Lay ra gio hang de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
               // model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;                      
                khachhang.Address = muaHang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();
            }
            try
            {
                if (ModelState.IsValid)
                {

                    var peyment = _context.Payments.ToList();
                    if (peyment != null)
                    {

                        ViewBag.Payments = peyment;


                    }
                    //Khoi tao don hang
                    Order donhang = new Order();

                    if (string.IsNullOrEmpty(muaHang.Note))
                    {
                        donhang.Note = "You don't have any notes";
                    }
                    else
                    {
                        donhang.Note = muaHang.Note;
                    }

                    donhang.CustomerId = model.CustomerId;
                    donhang.Address = model.Address;

                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusId = 1;//Don hang moi
                    donhang.Deleted = false;
                    donhang.Paid = false;
                    int paymentId = int.Parse(Request.Form["paymentId"]);
                    string desPayment = Request.Form["desPayment"];
                    donhang.PaymentId = paymentId;
                    donhang.DesPayment = desPayment;


                    donhang.ShipDate = DateTime.Now.AddDays(7);

                    donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));          
                    _context.Add(donhang);
                    _context.SaveChanges();
                    //tao danh sach don hang

                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.BooksId = item.book.Id;
                        orderDetail.Amount = item.amount;
                        orderDetail.TotalMoney = donhang.TotalMoney;
                        orderDetail.Price = (int)item.book.Price;
                        orderDetail.CreateDate = DateTime.Now;
                        _context.Add(orderDetail);
                    }
                    _context.SaveChanges();
                    //clear gio hang
                    HttpContext.Session.Remove("GioHang");
                    //Xuat thong bao
                    _notyfService.Success("Đơn hàng đặt thành công");
                    //cap nhat thong tin khach hang
                    return RedirectToAction("Success");


                }
            }
            catch 
            {
                //ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location","Name");
                ViewBag.GioHang = cart;
                return View(model);
            }
           // ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location","Name");
            ViewBag.GioHang = cart;
            return View(model);
        }
        [Route("dat-hang-thanh-cong.html",Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders
                    .Where(x => x.CustomerId == Convert.ToInt32(taikhoanID))
                    .OrderByDescending(x => x.OrderDate)
                    .FirstOrDefault();
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachhang.FullName;
                successVM.DonHangID = donhang.OrderId;
                successVM.Phone = khachhang.Phone;
                successVM.Address = khachhang.Address;
                //successVM.PhuongXa = donhang.Ward;
                //successVM.TinhThanh = donhang.District;
                return View(successVM);
            }
            catch 
            {
                return View();
            }
        }
        
    }
}
