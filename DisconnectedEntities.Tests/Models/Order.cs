using System;

namespace DisconnectedEntities.Tests.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Units { get; set; }
        public decimal Amount { get; set; }
    }
}