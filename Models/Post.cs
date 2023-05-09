using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Post
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Image { get; set; }

    public DateOnly CreatedDate { get; set; }

    public int? CategoryId { get; set; }

    public string? Name { get; set; }

    public string? NameAuthor { get; set; }

    public int? CustomerId { get; set; }

    public virtual PostCategory? Category { get; set; }

    public virtual Customer? Customer { get; set; }
}
