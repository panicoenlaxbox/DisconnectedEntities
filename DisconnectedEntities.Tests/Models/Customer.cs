using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DisconnectedEntities.Tests.Models
{
    public class Customer
    {
        public Customer()
        {
            Orders = new Collection<Order>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}