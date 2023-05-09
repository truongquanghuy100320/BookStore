using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime? CreatedDate { get; set; }
}
