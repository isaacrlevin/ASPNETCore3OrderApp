using System;
using System.Collections.Generic;
using System.Text;

namespace OrderApp.Shared.Model
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public string Status { get; set; }
    }
}
