using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? Salt { get; set; }

    public bool? Active { get; set; }

    public int? Role { get; set; }

    public string? Description { get; set; }

    public DateTime? Created { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<Post> Posts { get; } = new List<Post>();
}
