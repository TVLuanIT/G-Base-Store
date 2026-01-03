using GBStore.Models; // namespace chứa class Product

namespace GBStore.Extensions
{
    public static class ProductExtensions
    {
        public static string GetImagePath(this Product product)
        {
            var extensions = new[] { ".jpg", ".png" }; // danh sách định dạng
            foreach (var ext in extensions)
            {
                var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", product.Picture + ext);
                if (File.Exists(file))
                    return "/images/" + product.Picture + ext;
            }
            return "/images/default.png"; // fallback nếu không tìm thấy
        }
    }
}