using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentDescription { get; set; }

    public string? PaymentInfo { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Updated { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
