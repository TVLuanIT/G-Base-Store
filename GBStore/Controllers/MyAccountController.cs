using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GBStore.Data;
using GBStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
                return RedirectToAction("/Account/Login", new { area = "Identity" });
            }

            // Lưu ý: await ở đây cần async method
            var customer = await _context.Customers
                                         .FirstOrDefaultAsync(c => c.NameAccount == nameAccount);

            if (customer == null)
                return NotFound();

            return View(customer); // View nhận Customer, không phải Task<Customer>
        }

        // GET: MyAccount/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: MyAccount/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Customer model, IFormFile AvatarFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound();

            // Cập nhật thông tin
            user.Name = model.Name;
            user.Phone = model.Phone;
            user.CustomerAddress = model.CustomerAddress;

            // Upload avatar nếu có
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(AvatarFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await AvatarFile.CopyToAsync(stream);

                user.Avatar = "/uploads/avatars/" + fileName;
            }

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("EditProfile");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu.");
                return View(model);
            }
        }

        // Helper method: lấy Customer hiện tại dựa trên NameAccount
        private async Task<Customer?> GetCurrentUserAsync()
        {
            string? nameAccount = User.Identity?.Name;
            if (string.IsNullOrEmpty(nameAccount))
                return null;

            return await _context.Customers.FirstOrDefaultAsync(c => c.NameAccount == nameAccount);
        }
    }
}
