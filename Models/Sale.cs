using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Sale
{
    public int Id { get; set; }

    public int? DiscountPercent { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
