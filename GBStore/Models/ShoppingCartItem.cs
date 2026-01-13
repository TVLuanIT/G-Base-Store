using System;
using System.Collections.Generic;

namespace GBStore.Models;

public partial class ShoppingCartItem
{
    public int ShoppingCartItemId { get; set; }

    public int ShoppingCartId { get; set; }

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
}
