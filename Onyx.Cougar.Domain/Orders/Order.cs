using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Onyx.Cougar.Domain.Catalog;

namespace Onyx.Cougar.Domain.Orders
{
    public class Order
    {
        public int Id {get; set;}
        public List<OrderItem> Items {get; set;}
        public DateTime CreatedDate {get; set;}
        public decimal TotalPrice => Items.Sum(i => i.Price);
        
    }

    public class OrderItem
    {
        public int Id {get; set;}
        // public Item Item {get; set;}
        public Onyx.Cougar.Domain.Catalog.Item Item { get; set; } // Fully qualified name
        public int Quantity {get; set;}
        public decimal Price => Item.Price * Quantity;
    }
}