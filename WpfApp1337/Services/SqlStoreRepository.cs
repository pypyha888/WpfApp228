using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using ApplianceStoreIS.ApplicationData;
using ApplianceStoreIS.Models;

namespace ApplianceStoreIS.Services
{
    public class SqlStoreRepository : IStoreRepository
    {
        private readonly string dbName;
        private readonly string appConnection;
        private readonly string masterConnection;

        public SqlStoreRepository()
        {
            appConnection = ApplianceStoreEntitiesConnection.GetConnectionString();
            masterConnection = ApplianceStoreEntitiesConnection.GetMasterConnectionString();
            dbName = new SqlConnectionStringBuilder(appConnection).InitialCatalog;
            EnsureDatabase();
            EnsureTables();
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
            using (var cmd = new SqlCommand("SELECT Id, UserLogin, ProductName, Quantity, OrderDate, Status FROM Orders ORDER BY Id", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Order
                        {
                            Id = reader.GetInt32(0),
                            UserLogin = reader.GetString(1),
                            ProductName = reader.GetString(2),
                            Quantity = reader.GetInt32(3),
                            OrderDate = reader.GetDateTime(4),
                            Status = reader.GetString(5)
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

        public void AddOrder(Order order)
        {
            using (var conn = new SqlConnection(appConnection))
            using (var cmd = new SqlCommand("INSERT INTO Orders (UserLogin, ProductName, Quantity, OrderDate, Status) VALUES (@UserLogin,@ProductName,@Quantity,@OrderDate,@Status); SELECT CAST(SCOPE_IDENTITY() as int);", conn))
            {
                cmd.Parameters.AddWithValue("@UserLogin", order.UserLogin);
                cmd.Parameters.AddWithValue("@ProductName", order.ProductName);
                cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                cmd.Parameters.AddWithValue("@Status", order.Status);
                conn.Open();
                order.Id = (int)cmd.ExecuteScalar();
            }
        }

        private void EnsureDatabase()
        {
            using (var conn = new SqlConnection(masterConnection))
            using (var cmd = new SqlCommand($"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}]", conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureTables()
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

IF NOT EXISTS (SELECT 1 FROM Users WHERE Login='admin')
BEGIN
    INSERT INTO Users(Login, Password, FullName, Role)
    VALUES ('admin', 'Admin123!', N'Системный администратор', 'Admin')
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE Login='manager')
BEGIN
    INSERT INTO Users(Login, Password, FullName, Role)
    VALUES ('manager', 'Manager123!', N'Менеджер магазина', 'Manager')
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
        UserLogin NVARCHAR(50) NOT NULL,
        ProductName NVARCHAR(120) NOT NULL,
        Quantity INT NOT NULL,
        OrderDate DATETIME2 NOT NULL,
        Status NVARCHAR(40) NOT NULL
    )
END
IF COL_LENGTH('Orders', 'UserLogin') IS NULL
BEGIN
    ALTER TABLE Orders ADD UserLogin NVARCHAR(50) NULL;
    UPDATE Orders SET UserLogin = 'legacy_user' WHERE UserLogin IS NULL;
    ALTER TABLE Orders ALTER COLUMN UserLogin NVARCHAR(50) NOT NULL;
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
