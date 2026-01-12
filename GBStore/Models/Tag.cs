using System;
using System.Collections.Generic;

namespace GBStore.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
