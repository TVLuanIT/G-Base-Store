using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Models;

[Table("Customer")]
[Index("Email", Name = "UQ__User__A9D10534411002EB", IsUnique = true)]
[Index("NameAccount", Name = "UQ__User__E218DB2C06D904BD", IsUnique = true)]
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

    [InverseProperty("Customer")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Customer")]
    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
