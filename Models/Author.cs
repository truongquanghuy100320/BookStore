using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Author
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string Publish { get; set; } = null!;

    public string? Image { get; set; }

    public string? Story { get; set; }

    public DateTime? Created { get; set; }

    public virtual ICollection<Book> Books { get; } = new List<Book>();
}
