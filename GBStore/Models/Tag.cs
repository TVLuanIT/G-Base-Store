using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Models;

[Table("Tag")]
[Index("TagName", Name = "UQ__Tag__BDE0FD1D761A70EC", IsUnique = true)]
public partial class Tag
{
    [Key]
    public int TagId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TagName { get; set; } = null!;

    [InverseProperty("Tag")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
