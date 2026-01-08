using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GBStore.ViewComponents
{
    public class UserAvatarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string size = "sm")
        {
            // Chưa đăng nhập
            if (!User.Identity!.IsAuthenticated)
            {
                return View("Guest");
            }

            // Đã đăng nhập
            var name = User.Identity.Name ?? "U";
            var firstLetter = name.Substring(0, 1).ToUpper();

            ViewBag.Size = size;
            return View("LoggedIn", firstLetter);
        }
    }
}
