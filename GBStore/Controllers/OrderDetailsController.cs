using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GBStore.Data;
using GBStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly GbstoreContext _context;

        public OrderDetailsController(GbstoreContext context)
        {
            _context = context;
        }

        // GET: /OrderDetails/Index/7
        public async Task<IActionResult> Index(int id)
        {
            if (User?.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Account");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .SingleOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                return NotFound();

            int customerId = customer.CustomerId;

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.CustomerId == customer.CustomerId);

            if (order == null)
                return NotFound();

            return View(order.OrderDetails);
        }
    }
}
