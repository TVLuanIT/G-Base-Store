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
    public class ProductsController : Controller
    {
        private readonly GbstoreContext _context;

        public ProductsController(GbstoreContext context)
        {
            _context = context;
        }

        // Action hiển thị sản phẩm theo tag
        public IActionResult ProductsByTag(int id)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Tag)
                .Where(p => p.TagId == id)
                .ToList();

            // Lấy tên tag để hiển thị trên view
            ViewBag.Title = _context.Tags
                .Where(b => b.TagId == id)
                .Select(b => b.TagName)
                .FirstOrDefault();

            return View("Products", products);
        }

        // Action hiển thị sản phẩm theo category
        public IActionResult ProductsByCategory(int id)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id)
                .ToList();

            // Lấy tên category để hiển thị trên view
            ViewBag.Title = _context.Categories
                .Where(b => b.CategoryId == id)
                .Select(b => b.CategoryName)
                .FirstOrDefault();

            return View("Products", products);
        }

        // Action hiển thị sản phẩm theo brand
        public IActionResult ProductsByBrand(int id)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.BrandId == id)
                .ToList();

            ViewBag.Title = _context.Brands
                .Where(b => b.BrandId == id)
                .Select(b => b.BrandName)
                .FirstOrDefault();

            return View("Products", products);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var gbstoreContext = _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Tag);
            return View(await gbstoreContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "TagId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Sku,CategoryId,Series,BrandId,Price,Scale,Quantity,Size,Manufacturer,TagId,ProductWeight,AverageRating,ReviewCount,ProductDescription,Note,Picture")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "TagId", product.TagId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "TagId", product.TagId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Sku,CategoryId,Series,BrandId,Price,Scale,Quantity,Size,Manufacturer,TagId,ProductWeight,AverageRating,ReviewCount,ProductDescription,Note,Picture")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            ViewData["TagId"] = new SelectList(_context.Tags, "TagId", "TagId", product.TagId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
