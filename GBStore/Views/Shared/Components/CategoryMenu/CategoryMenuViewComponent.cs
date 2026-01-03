using GBStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GBStore.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly GbstoreContext _context;

        public CategoryMenuViewComponent(GbstoreContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories.OrderBy(c=>c.CategoryName).ToListAsync();
            return View(categories);
        }
    }
}