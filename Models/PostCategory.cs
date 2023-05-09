using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class PostCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Post> Posts { get; } = new List<Post>();
}
