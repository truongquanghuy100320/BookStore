using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ShipDate { get; set; }

    public int TransactStatusId { get; set; }

    public bool Deleted { get; set; }

    public bool Paid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int TotalMoney { get; set; }

    public string Note { get; set; } = null!;

    public string? Address { get; set; }

    public string DesTransactionStatus { get; set; } = null!;

    public string? DesPayment { get; set; }

    public int? PaymentId { get; set; }

    public string? Status { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();

    public virtual Payment? Payment { get; set; }

    public virtual TransactionStatus TransactStatus { get; set; } = null!;
}
