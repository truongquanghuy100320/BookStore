using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using PagedList.Core;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PaymentsController : Controller
    {
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }

        public PaymentsController(DatabaseContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/Payments
        public async Task<IActionResult> Index(int page = 1)
        {
            var pageNumber = page;
            var pageSize = 10;
            IQueryable<Payment> payments = _context.Payments.AsQueryable();
            var pagedCustomers = payments.OrderBy(x => x.PaymentId)
                                          .ToPagedList(pageNumber, pageSize);
            return View(pagedCustomers);



        }

        // GET: Admin/Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Admin/Payments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,PaymentMethod,PaymentDescription,PaymentInfo")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.Created = DateTime.Now;
                _context.Add(payment);
                await _context.SaveChangesAsync();
                _notyfService.Success("Successfully added new");
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Admin/Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        // POST: Admin/Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,PaymentMethod,PaymentDescription,PaymentInfo")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    payment.Updated = DateTime.Now; // tự động cập nhật thời gian
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Successfully updated ");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            return View(payment);
        }

        // GET: Admin/Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Admin/Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Payments == null)
            {
                return Problem("Entity set 'DatabaseContext.Payments'  is null.");
            }
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }
            
            await _context.SaveChangesAsync();
            _notyfService.Success("Delete successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
          return (_context.Payments?.Any(e => e.PaymentId == id)).GetValueOrDefault();
        }
    }
}
