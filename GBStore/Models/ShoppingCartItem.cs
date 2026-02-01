using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Models;

[Table("ShoppingCartItem")]
public partial class ShoppingCartItem
{
    [Key]
    public int ShoppingCartItemId { get; set; }

    public int ShoppingCartId { get; set; }

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ShoppingCartItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("ShoppingCartId")]
    [InverseProperty("ShoppingCartItems")]
    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
}
