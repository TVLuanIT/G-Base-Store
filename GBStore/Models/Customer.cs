using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Models;

[Table("Customer")]
[Index("Email", Name = "UQ__Customer__A9D10534C8CD6FF3", IsUnique = true)]
[Index("NameAccount", Name = "UQ__Customer__E218DB2CF981F4AD", IsUnique = true)]
public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? CustomerAddress { get; set; }

    [StringLength(100)]
    public string? NameAccount { get; set; }

    [StringLength(255)]
    public string? Avatar { get; set; }

    [StringLength(450)]
    public string? UserId { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Customer")]
    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    [ForeignKey("UserId")]
    [InverseProperty("Customers")]
    public virtual AspNetUser? User { get; set; }
}
