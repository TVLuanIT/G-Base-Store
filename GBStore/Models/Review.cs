using System;
using System.Collections.Generic;

namespace GBStore.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public string CustomerId { get; set; } = null!;

    public int ProductId { get; set; }

    public decimal Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual AspNetUser Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
