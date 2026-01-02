using GBStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GBStore.ViewComponents
{
    public class BrandMenuViewComponent : ViewComponent
    {
        private readonly GbstoreContext _context;

        public BrandMenuViewComponent(GbstoreContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brands = await _context.Brands.ToListAsync();
            return View(brands);
        }
    }
}