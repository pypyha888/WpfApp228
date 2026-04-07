using System;

namespace ApplianceStoreIS.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserLogin { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
    }
}
