using GBStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GBStore.ViewComponents
{
    public class TagMenuViewComponent : ViewComponent
    {
        private readonly GbstoreContext _context;

        public TagMenuViewComponent(GbstoreContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tags = await _context.Tags.OrderBy(t=>t.TagName).ToListAsync();
            return View(tags);
        }
    }
}