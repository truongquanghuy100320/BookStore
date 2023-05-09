using System.Security.Claims;
using System.Text.Encodings.Web;
using AspNetCoreHero.ToastNotification.Abstractions;
using Castle.Core.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BookStore.Extention;
using BookStore.Helpper;
using BookStore.Models;
using BookStore.ModelViews;
using System.Net.Mail;
using MimeKit;
using System.Net;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }
        private readonly ILogger<AccountsController> _logger;


        public AccountsController(DatabaseContext context, INotyfService notyfService, ILogger<AccountsController> logger)
        {
            _context = context;
            _notyfService = notyfService;
            _logger = logger;
            
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachhang != null)
                    return Json(data: "Phone number : " + Phone + "used");

                return Json(data: true);
                
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                    return Json(data: "Email : " + Email + " used");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    var lsDonHang = _context.Orders
                        .Include(x => x.TransactStatus)
                        .AsNoTracking()
                        .Where(x => x.CustomerId == khachhang.CustomerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.DanhSachDonHang = lsDonHang;
                    return View(khachhang);

                }

            }
            return RedirectToAction("Login");
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html",Name ="DangKy")]
        public IActionResult DangkyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html",Name ="DangKy")]
        public async Task<IActionResult> DangkyTaiKhoan(RegisterViewModel taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    Customer khachhang = new Customer
                    {
                        FullName = taikhoan.FullName,
                        Phone = taikhoan.PhoneNumber.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        Role = 3,
                        Description = "User",
                        Created = DateTime.Now
                    };

                    try
                    {
                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        //Lưu Session MaKh
                        HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());

                       


                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,khachhang.FullName),
                            new Claim("CustomerId", khachhang.CustomerId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notyfService.Success("Sign Up Success");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch 
                    {
                        return RedirectToAction("DangkyTaiKhoan", "Accounts");
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch 
            {
                return View(taikhoan);
            }
        }
       
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }




        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name ="DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);

                    if (khachhang == null) return RedirectToAction("DangkyTaiKhoan");

                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if (khachhang.Password != pass)
                    {
                        var loginAttempts = 0;
                        var isLockedOut = false;

                        if (HttpContext.Session.GetInt32("LoginAttempts") != null)
                        {
                            loginAttempts = (int)HttpContext.Session.GetInt32("LoginAttempts");
                        }

                        loginAttempts++;

                        if (loginAttempts >= 5)
                        {
                            _notyfService.Warning("Wrong login too many times. Wait 30 seconds before logging in again.");
                            //Thread.Sleep(30000);
                            HttpContext.Session.SetString("LockoutTime", DateTime.Now.ToString());

                            isLockedOut = true;
                        }
                        else
                        {
                            _notyfService.Error("Login with wrong account or password");
                        }

                        HttpContext.Session.SetInt32("LoginAttempts", loginAttempts);

                        if (isLockedOut)
                        {
                            // Thực hiện hành động khóa tài khoản ở đây
                            return RedirectToAction("Login", "Accounts");
                        }
                        else
                        {
                            return View(customer);
                        }
                    }

                    //kiem tra xem account co bi disable hay khong
                    if (khachhang.Active == false)
                    {
                        return RedirectToAction("ThongBao", "Accounts");
                    }


                    //Luu Session MaKh
                    HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");
                    // Lưu thông tin admin vào session
                    var session = new UserLogin();
                    session.Email = khachhang.Email;
                    session.CustomerId = khachhang.CustomerId;
                    session.FullName = khachhang.FullName;
                    session.Desciption = khachhang.Description;
                    session.Role = khachhang.Role ?? 0;
                    HttpContext.Session.SetString(Func.USER_SESSION, JsonConvert.SerializeObject(session));
                    TempData[Func.USER_SESSION] = JsonConvert.SerializeObject(session);

                    //Identity
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, khachhang.FullName),
                new Claim("CustomerId", khachhang.CustomerId.ToString())
            };
                    if (khachhang.Role == 1 || khachhang.Role == 2)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Successful login");
                    if (khachhang.Role == 1 || khachhang.Role == 2)
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (khachhang.Role == 3)
                    {
                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            _logger.LogInformation("Redirecting to Dashboard page.");
                            return RedirectToAction("Dashboard", "Accounts");
                        }
                        else
                        {
                            _logger.LogInformation($"Redirecting to {returnUrl}.");
                            return LocalRedirect(returnUrl);
                        }
                    }
                }
            }
            catch
            {
                return RedirectToAction("DangkyTaiKhoan", "Accounts");
            }
            return View(customer);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (taikhoanID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("Login", "Accounts");
                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    {
                        string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                        taikhoan.Password = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Change password successfully");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Success("Password change failed");
                return RedirectToAction("Dashboard", "Accounts");
            }
            _notyfService.Success("Password change failed");
            return RedirectToAction("Dashboard", "Accounts");
        }
        [HttpGet]
        [Route("dang-xuat.html",Name ="DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


       
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            // Xử lý logic quên mật khẩu ở đây
            // ...

            // Hiển thị thông báo thành công và chuyển hướng về trang đăng nhập
            ViewBag.Message = "A reset password link has been sent to your email";
            return View();
        }


    }
}
