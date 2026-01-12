using System;
using System.Collections.Generic;

namespace GBStore.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? CustomerAddress { get; set; }

    public string? NameAccount { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
