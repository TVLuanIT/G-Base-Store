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
    public class BrandsController : Controller
    {
        private readonly GbstoreContext _context;

        public BrandsController(GbstoreContext context)
        {
            _context = context;
        }

        // Action hiển thị sản phẩm theo brand
        public IActionResult Products(int id)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.BrandId == id)
                .ToList();

            // Lấy tên brand để hiển thị trên view
            ViewBag.BrandName = _context.Brands
                .Where(b => b.BrandId == id)
                .Select(b => b.BrandName)
                .FirstOrDefault();

            return View(products);
        }
    }
}
