using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class TransactionStatus
{
    public int TransactStatusId { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Updated { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
