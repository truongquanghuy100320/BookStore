using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using AspNetCoreHero.ToastNotification.Notyf;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TransactionStatusController : Controller
    {
        private readonly DatabaseContext _context;
        public INotyfService _notyfService { get; }
    public TransactionStatusController(DatabaseContext context,INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/TransactionStatus
        public async Task<IActionResult> Index(int page = 1)
        {
            var pageNumber = page;
            var pageSize = 10;
            IQueryable<TransactionStatus> transactionStatuses = _context.TransactionStatuses.AsQueryable();
            var pagedCustomers = transactionStatuses.OrderBy(x => x.TransactStatusId)
                                          .ToPagedList(pageNumber, pageSize);
            return View(pagedCustomers);



        }

        // GET: Admin/TransactionStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TransactionStatuses == null)
            {
                return NotFound();
            }

            var transactionStatus = await _context.TransactionStatuses
                .FirstOrDefaultAsync(m => m.TransactStatusId == id);
            if (transactionStatus == null)
            {
                return NotFound();
            }

            return View(transactionStatus);
        }

        // GET: Admin/TransactionStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TransactionStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactStatusId,Status,Description")] TransactionStatus transactionStatus)
        {
            if (ModelState.IsValid)
            {
                transactionStatus.Created = DateTime.Now;
                _context.Add(transactionStatus);
                await _context.SaveChangesAsync();
                _notyfService.Success("Successfully added new");
                return RedirectToAction(nameof(Index));
            }
            return View(transactionStatus);
        }

        // GET: Admin/TransactionStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TransactionStatuses == null)
            {
                return NotFound();
            }

            var transactionStatus = await _context.TransactionStatuses.FindAsync(id);
            if (transactionStatus == null)
            {
                return NotFound();
            }
            return View(transactionStatus);
        }

        // POST: Admin/TransactionStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactStatusId,Status,Description")] TransactionStatus transactionStatus)
        {
            if (id != transactionStatus.TransactStatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTransactionStatus = await _context.TransactionStatuses.FindAsync(transactionStatus.TransactStatusId);
                    if (existingTransactionStatus == null)
                    {
                        return NotFound();
                    }

                    transactionStatus.Created = existingTransactionStatus.Created; // gán lại giá trị Created từ cơ sở dữ liệu
                    transactionStatus.Updated = DateTime.Now;
                    _context.Entry(existingTransactionStatus).State = EntityState.Detached; // đánh dấu đối tượng cũ là đã bị detach để tránh lỗi tracking
                    _context.Update(transactionStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionStatusExists(transactionStatus.TransactStatusId))
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
            return View(transactionStatus);
        }

        // GET: Admin/TransactionStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TransactionStatuses == null)
            {
                return NotFound();
            }

            var transactionStatus = await _context.TransactionStatuses
                .FirstOrDefaultAsync(m => m.TransactStatusId == id);
            if (transactionStatus == null)
            {
                return NotFound();
            }

            return View(transactionStatus);
        }

        // POST: Admin/TransactionStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TransactionStatuses == null)
            {
                return Problem("Entity set 'DatabaseContext.TransactionStatuses'  is null.");
            }
            var transactionStatus = await _context.TransactionStatuses.FindAsync(id);
            if (transactionStatus != null)
            {
                _context.TransactionStatuses.Remove(transactionStatus);
            }
            
            await _context.SaveChangesAsync();
            _notyfService.Success("Delete successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionStatusExists(int id)
        {
          return (_context.TransactionStatuses?.Any(e => e.TransactStatusId == id)).GetValueOrDefault();
        }
    }
}
