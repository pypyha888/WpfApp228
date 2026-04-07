using System;
using System.Collections.ObjectModel;
using System.Linq;
using ApplianceStoreIS.Models;

namespace ApplianceStoreIS.Services
{
    public class StoreDataService
    {
        private readonly IStoreRepository repository;

        public ObservableCollection<Product> Products { get; }

        public ObservableCollection<Supplier> Suppliers { get; }

        public ObservableCollection<Order> Orders { get; }

        public StoreDataService(IStoreRepository repository)
        {
            this.repository = repository;
            Products = repository.GetProducts();
            Suppliers = repository.GetSuppliers();
            Orders = repository.GetOrders();
        }

        public void SaveProduct(Product product)
        {
            repository.SaveProduct(product);
        }

        public void DeleteProduct(int productId)
        {
            repository.DeleteProduct(productId);
        }

        public void AddOrder(string userLogin, string productName, int quantity)
        {
            var order = new Order
            {
                UserLogin = userLogin,
                ProductName = productName,
                Quantity = quantity,
                OrderDate = DateTime.Now,
                Status = "Новый"
            };

            repository.AddOrder(order);
            if (!Orders.Contains(order))
            {
                Orders.Add(order);
            }
        }

        public ObservableCollection<Order> GetOrdersForUser(string userLogin, bool includeAll)
        {
            if (includeAll)
            {
                return new ObservableCollection<Order>(Orders);
            }

            return new ObservableCollection<Order>(Orders.Where(o => o.UserLogin == userLogin));
        }
    }
}
