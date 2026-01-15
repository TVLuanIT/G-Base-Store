using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Models;

[Table("Product")]
[Index("Sku", Name = "UQ__Product__CA1ECF0D885ACA85", IsUnique = true)]
public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("SKU")]
    [StringLength(50)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    public int CategoryId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Series { get; set; }

    public int BrandId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Scale { get; set; }

    public int? Quantity { get; set; }

    [StringLength(50)]
    public string? Size { get; set; }

    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;

    public int? TagId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ProductWeight { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal? AverageRating { get; set; }

    public int? ReviewCount { get; set; }

    public string? ProductDescription { get; set; }

    [StringLength(255)]
    public string? Note { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Picture { get; set; } = null!;

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand Brand { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Product")]
    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

    [ForeignKey("TagId")]
    [InverseProperty("Products")]
    public virtual Tag? Tag { get; set; }
}
