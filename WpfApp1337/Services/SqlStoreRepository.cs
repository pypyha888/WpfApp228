using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using ApplianceStoreIS.Models;

namespace ApplianceStoreIS.Services
{
    public class SqlStoreRepository : IStoreRepository
    {
        private const string MasterConnection = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Initial Catalog=master;";
        private const string DbName = "ApplianceStoreISDb";
        private readonly string appConnection = $"Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Initial Catalog={DbName};";

        public SqlStoreRepository()
        {
            EnsureDatabase();
            EnsureTablesAndSeed();
        }

        public ObservableCollection<Product> GetProducts()
        {
            var items = new ObservableCollection<Product>();
            using (var conn = new SqlConnection(appConnection))
            using (var cmd = new SqlCommand("SELECT Id, Name, Category, Price, Quantity, Brand FROM Products ORDER BY Id", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Category = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            Quantity = reader.GetInt32(4),
                            Brand = reader.GetString(5)
                        });
                    }
                }
            }

            return items;
        }

        public ObservableCollection<Supplier> GetSuppliers()
        {
            var items = new ObservableCollection<Supplier>();
            using (var conn = new SqlConnection(appConnection))
            using (var cmd = new SqlCommand("SELECT Id, Name, ContactPhone FROM Suppliers ORDER BY Id", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Supplier
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ContactPhone = reader.GetString(2)
                        });
                    }
                }
            }

            return items;
        }

        public ObservableCollection<Order> GetOrders()
        {
            var items = new ObservableCollection<Order>();
            using (var conn = new SqlConnection(appConnection))
            using (var cmd = new SqlCommand("SELECT Id, ProductName, Quantity, OrderDate, Status FROM Orders ORDER BY Id", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Order
                        {
                            Id = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Quantity = reader.GetInt32(2),
                            OrderDate = reader.GetDateTime(3),
                            Status = reader.GetString(4)
                        });
                    }
                }
            }

            return items;
        }

        public ObservableCollection<UserAccount> GetUsers()
        {
            var items = new ObservableCollection<UserAccount>();
            using (var conn = new SqlConnection(appConnection))
            using (var cmd = new SqlCommand("SELECT Login, Password, FullName, Role FROM Users ORDER BY Login", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new UserAccount
                        {
                            Login = reader.GetString(0),
                            Password = reader.GetString(1),
                            FullName = reader.GetString(2),
                            Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString(3))
                        });
                    }
                }
            }

            return items;
        }

        public bool AddUser(UserAccount user, out string error)
        {
            error = string.Empty;
            using (var conn = new SqlConnection(appConnection))
            using (var cmd = new SqlCommand("INSERT INTO Users (Login, Password, FullName, Role) VALUES (@Login, @Password, @FullName, @Role)", conn))
            {
                cmd.Parameters.AddWithValue("@Login", user.Login);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@FullName", user.FullName);
                cmd.Parameters.AddWithValue("@Role", user.Role.ToString());

                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException)
                {
                    error = "Пользователь с таким логином уже существует.";
                    return false;
                }
            }
        }

        public void SaveProduct(Product product)
        {
            using (var conn = new SqlConnection(appConnection))
            {
                conn.Open();

                if (product.Id == 0)
                {
                    using (var insert = new SqlCommand("INSERT INTO Products (Name, Category, Price, Quantity, Brand) VALUES (@Name,@Category,@Price,@Quantity,@Brand); SELECT CAST(SCOPE_IDENTITY() as int);", conn))
                    {
                        insert.Parameters.AddWithValue("@Name", product.Name);
                        insert.Parameters.AddWithValue("@Category", product.Category);
                        insert.Parameters.AddWithValue("@Price", product.Price);
                        insert.Parameters.AddWithValue("@Quantity", product.Quantity);
                        insert.Parameters.AddWithValue("@Brand", product.Brand);
                        product.Id = (int)insert.ExecuteScalar();
                    }
                }
                else
                {
                    using (var update = new SqlCommand("UPDATE Products SET Name=@Name, Category=@Category, Price=@Price, Quantity=@Quantity, Brand=@Brand WHERE Id=@Id", conn))
                    {
                        update.Parameters.AddWithValue("@Id", product.Id);
                        update.Parameters.AddWithValue("@Name", product.Name);
                        update.Parameters.AddWithValue("@Category", product.Category);
                        update.Parameters.AddWithValue("@Price", product.Price);
                        update.Parameters.AddWithValue("@Quantity", product.Quantity);
                        update.Parameters.AddWithValue("@Brand", product.Brand);
                        update.ExecuteNonQuery();
                    }
                }
            }
        }

        public void DeleteProduct(int productId)
        {
            using (var conn = new SqlConnection(appConnection))
            using (var cmd = new SqlCommand("DELETE FROM Products WHERE Id=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", productId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureDatabase()
        {
            using (var conn = new SqlConnection(MasterConnection))
            using (var cmd = new SqlCommand($"IF DB_ID('{DbName}') IS NULL CREATE DATABASE [{DbName}]", conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureTablesAndSeed()
        {
            const string script = @"
IF OBJECT_ID('Users','U') IS NULL
BEGIN
    CREATE TABLE Users (
        Login NVARCHAR(50) NOT NULL PRIMARY KEY,
        Password NVARCHAR(100) NOT NULL,
        FullName NVARCHAR(120) NOT NULL,
        Role NVARCHAR(20) NOT NULL
    )
END
IF OBJECT_ID('Products','U') IS NULL
BEGIN
    CREATE TABLE Products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(120) NOT NULL,
        Category NVARCHAR(80) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL,
        Brand NVARCHAR(80) NOT NULL
    )
END
IF OBJECT_ID('Suppliers','U') IS NULL
BEGIN
    CREATE TABLE Suppliers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(120) NOT NULL,
        ContactPhone NVARCHAR(40) NOT NULL
    )
END
IF OBJECT_ID('Orders','U') IS NULL
BEGIN
    CREATE TABLE Orders (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductName NVARCHAR(120) NOT NULL,
        Quantity INT NOT NULL,
        OrderDate DATETIME2 NOT NULL,
        Status NVARCHAR(40) NOT NULL
    )
END

IF NOT EXISTS(SELECT 1 FROM Users)
BEGIN
    INSERT INTO Users(Login, Password, FullName, Role) VALUES
    ('admin','Admin123!','Системный администратор','Admin'),
    ('manager','Manager123!','Менеджер магазина','Manager'),
    ('user','User123!','Покупатель','User')
END

IF NOT EXISTS(SELECT 1 FROM Products)
BEGIN
    INSERT INTO Products(Name, Category, Price, Quantity, Brand) VALUES
    (N'Стиральная машина',N'Крупная техника',45990,8,N'LG'),
    (N'Пылесос',N'Малая техника',12990,15,N'Samsung'),
    (N'Холодильник',N'Крупная техника',73990,4,N'Bosch')
END

IF NOT EXISTS(SELECT 1 FROM Suppliers)
BEGIN
    INSERT INTO Suppliers(Name, ContactPhone) VALUES
    (N'ТехноОпт',N'+7 (495) 100-10-10'),
    (N'БытПоставка',N'+7 (495) 200-20-20')
END

IF NOT EXISTS(SELECT 1 FROM Orders)
BEGIN
    INSERT INTO Orders(ProductName, Quantity, OrderDate, Status) VALUES
    (N'Стиральная машина',2,DATEADD(DAY,-1,GETDATE()),N'Новый'),
    (N'Холодильник',1,GETDATE(),N'В обработке')
END";

            using (var conn = new SqlConnection(appConnection))
            using (var cmd = new SqlCommand(script, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
