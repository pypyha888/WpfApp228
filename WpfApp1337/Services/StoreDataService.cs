using System;
using System.Collections.ObjectModel;
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

        public void AddOrder(string productName, int quantity)
        {
            var order = new Order
            {
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

        public ObservableCollection<Order> GetOrders()
        {
            return new ObservableCollection<Order>(Orders);
        }
    }
}
