using System;
using System.Collections.Generic;

namespace GBStore.Models;

public partial class ShoppingCart
{
    public int ShoppingCartId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ShoppingCartStatus { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
