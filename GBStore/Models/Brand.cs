using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Models;

[Table("Brand")]
[Index("BrandName", Name = "UQ__Brand__2206CE9BA6F8756A", IsUnique = true)]
public partial class Brand
{
    [Key]
    public int BrandId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string BrandName { get; set; } = null!;

    [InverseProperty("Brand")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
