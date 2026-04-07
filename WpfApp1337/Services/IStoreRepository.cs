using System.Collections.ObjectModel;
using ApplianceStoreIS.Models;

namespace ApplianceStoreIS.Services
{
    public interface IStoreRepository
    {
        ObservableCollection<Product> GetProducts();

        ObservableCollection<Supplier> GetSuppliers();

        ObservableCollection<Order> GetOrders();

        ObservableCollection<UserAccount> GetUsers();

        bool AddUser(UserAccount user, out string error);

        void SaveProduct(Product product);

        void DeleteProduct(int productId);

        void AddOrder(Order order);
    }
}
