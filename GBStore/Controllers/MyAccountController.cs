using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBStore.Data;
using GBStore.Models;

namespace GBStore.Controllers
{

    public class MyAccountController : Controller
    {
        private readonly GbstoreContext _context;

        public MyAccountController(GbstoreContext context)
        {
            _context = context;
        }

        // GET: MyAccount
        public async Task<IActionResult> Index()
        {
            string? nameAccount = User.Identity?.Name;

            if (string.IsNullOrEmpty(nameAccount))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Lưu ý: await ở đây cần async method
            var customer = await _context.Customers
                                         .FirstOrDefaultAsync(c => c.NameAccount == nameAccount);

            if (customer == null)
                return NotFound();

            return View(customer); // View nhận Customer, không phải Task<Customer>
        }

        // GET: /MyAccount/Edit
        public async Task<IActionResult> Edit()
        {
            string? nameAccount = User.Identity?.Name;
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.NameAccount == nameAccount);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: /MyAccount/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer updatedCustomer)
        {
            string? nameAccount = User.Identity?.Name;
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.NameAccount == nameAccount);
            if (customer == null) return NotFound();

            if (ModelState.IsValid)
            {
                customer.Name = updatedCustomer.Name;
                customer.Email = updatedCustomer.Email;
                customer.Phone = updatedCustomer.Phone;
                customer.CustomerAddress = updatedCustomer.CustomerAddress;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(updatedCustomer);
        }
    }
}
