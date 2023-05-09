using System;
using BookStore.Models;

namespace BookStore.ModelViews
{
    public class CartItem
    {
        public Book book { get; set; }
        public int amount { get; set; }
        public double TotalMoney => (double)(amount * book.Price.Value);

    }
}
