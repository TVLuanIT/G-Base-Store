using System;
using System.Collections.Generic;

namespace GBStore.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Series { get; set; }

    public int BrandId { get; set; }

    public decimal Price { get; set; }

    public string? Scale { get; set; }

    public int? Quantity { get; set; }

    public string? Size { get; set; }

    public string Manufacturer { get; set; } = null!;

    public int? TagId { get; set; }

    public string? ProductWeight { get; set; }

    public decimal? AverageRating { get; set; }

    public int? ReviewCount { get; set; }

    public string? ProductDescription { get; set; }

    public string? Note { get; set; }

    public string Picture { get; set; } = null!;

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

    public virtual Tag? Tag { get; set; }
}
