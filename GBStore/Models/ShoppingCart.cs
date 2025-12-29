using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Models;

[Table("ShoppingCart")]
public partial class ShoppingCart
{
    [Key]
    public int ShoppingCartId { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    public string? ShoppingCartStatus { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("ShoppingCarts")]
    public virtual User Customer { get; set; } = null!;

    [InverseProperty("ShoppingCart")]
    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
