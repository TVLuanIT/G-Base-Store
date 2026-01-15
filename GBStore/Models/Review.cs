using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Models;

[Table("Review")]
public partial class Review
{
    [Key]
    public int ReviewId { get; set; }

    [StringLength(450)]
    public string CustomerId { get; set; } = null!;

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal Rating { get; set; }

    public string? Comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Reviews")]
    public virtual AspNetUser Customer { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("Reviews")]
    public virtual Product Product { get; set; } = null!;
}
