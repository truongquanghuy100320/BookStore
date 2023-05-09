using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Book
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? AuthorId { get; set; }

    public string? NameAuthor { get; set; }

    public string? Image { get; set; }

    public int? CategoryId { get; set; }

    public string? NameCate { get; set; }

    public string? Publisher { get; set; }

    public int? Quantity { get; set; }

    public string? Language { get; set; }

    public DateTime? PublicationDate { get; set; }

    public string? IllustrationsNote { get; set; }

    public string? Isbn10 { get; set; }

    public string? Isbn13 { get; set; }

    public DateTime? Created { get; set; }

    public bool? Status { get; set; }

    public virtual Author? Author { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();
}
