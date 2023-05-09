using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using BookStore.Extention;
using BookStore.Helpper;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomersController : Controller
    {
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }

        public CustomersController(DatabaseContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/Customers
        public async Task<IActionResult> Index(int page = 1)
        {
            var pageNumber = page;
            var pageSize = 10;
            IQueryable<Customer> customers = _context.Customers.AsQueryable();
            var pagedCustomers = customers.OrderBy(x => x.CustomerId)
                                          .ToPagedList(pageNumber, pageSize);
            return View(pagedCustomers);



        }

        // GET: Admin/Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Role,Description,FullName,Email,Phone,Password,Active,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Tạo salt mới
                string salt = Utilities.GetRandomKey();
                Customer khachhang = new Customer
                {
                    Email = customer.Email.Trim().ToLower(),
                    Password = (customer.Password + salt.Trim()).ToMD5(),
                    Salt = salt,
                    // Copy các trường khác
                    Role = customer.Role,
                    Description = customer.Description,
                    FullName = customer.FullName,
                    Phone = customer.Phone,
                    Active = customer.Active,
                    Address = customer.Address,
                    Created = DateTime.Now
                };


                // Lưu khách hàng mới vào database
                _context.Add(khachhang);
                await _context.SaveChangesAsync();
                _notyfService.Success("Successfully added new");
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Role,Description,FullName,Avatar,Email,Phone,Password,Salt,Active,Address")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Tìm khách hàng cần sửa trong database
                var khachhang = await _context.Customers.FindAsync(id);
                if (khachhang == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin khách hàng
                khachhang.Email = customer.Email.Trim().ToLower();
                khachhang.FullName = customer.FullName;
                khachhang.Phone = customer.Phone;
                khachhang.Role = customer.Role;
                khachhang.Description = customer.Description;
                khachhang.Active = customer.Active;
                khachhang.Address = customer.Address;

                // Nếu người dùng cập nhật password, tạo salt mới và mã hóa password
                if (!string.IsNullOrWhiteSpace(customer.Password))
                {
                    // Cập nhật Salt và Password mới nếu có mật khẩu mới
                    string salt = Utilities.GetRandomKey();
                    if (!string.IsNullOrWhiteSpace(salt))
                    {
                        khachhang.Password = (customer.Password + salt.Trim()).ToMD5();
                        khachhang.Salt = salt;
                    }
                }
                else
                {
                    // Giữ nguyên giá trị password trong database nếu không cập nhật mật khẩu mới
                    khachhang.Password = khachhang.Password;
                    khachhang.Salt = khachhang.Salt;
                }






                try
                {
                    // Lưu thông tin khách hàng vào database
                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Successfully updated");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'DatabaseContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            _notyfService.Success("Successfully Deleted");
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
