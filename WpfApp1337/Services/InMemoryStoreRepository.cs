using System;
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
            users = new ObservableCollection<UserAccount>
            {
                new UserAccount { Login = "admin", Password = "Admin123!", FullName = "Системный администратор", Role = UserRole.Admin },
                new UserAccount { Login = "manager", Password = "Manager123!", FullName = "Менеджер магазина", Role = UserRole.Manager },
                new UserAccount { Login = "user", Password = "User123!", FullName = "Покупатель", Role = UserRole.User }
            };

            products = new ObservableCollection<Product>
            {
                new Product { Id = 1, Name = "Стиральная машина", Category = "Крупная техника", Price = 45990m, Quantity = 8, Brand = "LG" },
                new Product { Id = 2, Name = "Пылесос", Category = "Малая техника", Price = 12990m, Quantity = 15, Brand = "Samsung" },
                new Product { Id = 3, Name = "Холодильник", Category = "Крупная техника", Price = 73990m, Quantity = 4, Brand = "Bosch" }
            };

            suppliers = new ObservableCollection<Supplier>
            {
                new Supplier { Id = 1, Name = "ТехноОпт", ContactPhone = "+7 (495) 100-10-10" },
                new Supplier { Id = 2, Name = "БытПоставка", ContactPhone = "+7 (495) 200-20-20" }
            };

            orders = new ObservableCollection<Order>
            {
                new Order { Id = 1, UserLogin = "user", ProductName = "Стиральная машина", Quantity = 2, OrderDate = DateTime.Today.AddDays(-1), Status = "Новый" },
                new Order { Id = 2, UserLogin = "user", ProductName = "Холодильник", Quantity = 1, OrderDate = DateTime.Today, Status = "В обработке" }
            };
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
