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
    public class OrdersController : Controller
    {
        private readonly GbstoreContext _context;

        public OrdersController(GbstoreContext context)
        {
            _context = context;
        }

        // GET: /Orders/OrderSuccess/5
        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: /Orders/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
                return Redirect("/Identity/Account/Login");

            int customerId = int.Parse(customerIdClaim.Value);

            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}
