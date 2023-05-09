using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using PagedList.Core;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }

        public AdminOrdersController(DatabaseContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminOrders
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 8;
            var Orders = _context.Orders
           .Include(o => o.Customer)
           .Include(o => o.TransactStatus)
           .AsNoTracking()
           .OrderBy(o => o.OrderDate);



            IPagedList<Order> models = new PagedList<Order>(Orders, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }


        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var Chitietdonhang = _context.OrderDetails
                .Include(x => x.Books)
                .AsNoTracking()
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x => x.OrderDetailId)
                .ToList();
            ViewBag.ChiTiet = Chitietdonhang;
            return View(order);
        }

        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .AsNoTracking()
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            if (order.Customer == null)
            {
                // Thực hiện xử lý nếu order.Customer bằng null ở đây
                // Ví dụ: trả về một lỗi hoặc thông báo cho người dùng biết
                return BadRequest("Order không có thông tin khách hàng");
            }
            ViewBag.Trangthai = new SelectList(_context.TransactionStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return PartialView("ChangeStatus", order);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, [Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,TotalMoney,Note,Address,DesTransactionStatus,DesPayment,PaymentId,Status")] Order order)
        {
            var donhang = await _context.Orders
                .Include(x => x.Customer)
                .Include(x => x.TransactStatus)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (donhang == null)
            {
                return NotFound();
            }

            donhang.Paid = order.Paid;
            donhang.Deleted = order.Deleted;
            donhang.TransactStatusId = order.TransactStatusId;

            if (donhang.Paid)
            {
                donhang.PaymentDate = DateTime.Now;
            }

            if (donhang.TransactStatusId == 5)
            {
                donhang.Deleted = true;
            }

            if (donhang.TransactStatusId == 3)
            {
                donhang.ShipDate = DateTime.Now;
            }

            try
            {
                _context.Update(donhang);
                await _context.SaveChangesAsync();
                _notyfService.Success("Cập nhật trạng thái đơn hàng thành công");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Lấy danh sách trạng thái giao dịch và truyền vào view
            var transactionStatuses = await _context.TransactionStatuses.ToListAsync();
            ViewBag.Trangthai = new SelectList(transactionStatuses, "TransactStatusId", "Status", donhang.TransactStatusId);

            return PartialView("ChangeStatus", donhang);
        }



        // GET: Admin/AdminOrders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId");
            ViewData["TransactStatusId"] = new SelectList(_context.TransactionStatuses, "TransactStatusId", "TransactStatusId");
            return View();
        }

        // POST: Admin/AdminOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,TotalMoney,Note,Address,DesTransactionStatus,DesPayment,PaymentId,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", order.PaymentId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactionStatuses, "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // GET: Admin/AdminOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", order.PaymentId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactionStatuses, "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,TotalMoney,Note,Address,DesTransactionStatus,DesPayment,PaymentId,Status")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", order.PaymentId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactionStatuses, "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // GET: Admin/AdminOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var Chitietdonhang = _context.OrderDetails
                .Include(x => x.Books)
                .AsNoTracking()
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x => x.OrderDetailId)
                .ToList();
            ViewBag.ChiTiet = Chitietdonhang;

            return View(order);
        }

        // POST: Admin/AdminOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Deleted = true;
            _context.Update(order);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa đơn hàng thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
