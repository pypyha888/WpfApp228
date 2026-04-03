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
    }
}
