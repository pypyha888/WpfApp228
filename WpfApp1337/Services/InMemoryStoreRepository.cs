using System.Collections.ObjectModel;
using System.Linq;
using ApplianceStoreIS.Models;

namespace ApplianceStoreIS.Services
{
    public class InMemoryStoreRepository : IStoreRepository
    {
        private readonly ObservableCollection<Product> products;
        private readonly ObservableCollection<Supplier> suppliers;
        private readonly ObservableCollection<Order> orders;
        private readonly ObservableCollection<UserAccount> users;

        public InMemoryStoreRepository()
        {
            users = new ObservableCollection<UserAccount>();
            products = new ObservableCollection<Product>();
            suppliers = new ObservableCollection<Supplier>();
            orders = new ObservableCollection<Order>();
        }

        public ObservableCollection<Product> GetProducts() => products;

        public ObservableCollection<Supplier> GetSuppliers() => suppliers;

        public ObservableCollection<Order> GetOrders() => orders;

        public ObservableCollection<UserAccount> GetUsers() => users;

        public bool AddUser(UserAccount user, out string error)
        {
            error = string.Empty;
            if (users.Any(u => u.Login == user.Login))
            {
                error = "Пользователь с таким логином уже существует.";
                return false;
            }

            users.Add(user);
            return true;
        }

        public void SaveProduct(Product product)
        {
            var existing = products.FirstOrDefault(p => p.Id == product.Id);
            if (existing == null)
            {
                product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;
                return;
            }

            existing.Name = product.Name;
            existing.Category = product.Category;
            existing.Brand = product.Brand;
            existing.Price = product.Price;
            existing.Quantity = product.Quantity;
        }

        public void DeleteProduct(int productId)
        {
            var existing = products.FirstOrDefault(p => p.Id == productId);
            if (existing != null)
            {
                products.Remove(existing);
            }
        }

        public void AddOrder(Order order)
        {
            order.Id = orders.Any() ? orders.Max(o => o.Id) + 1 : 1;
            orders.Add(order);
        }
    }
}
