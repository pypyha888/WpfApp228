using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ApplianceStoreIS.Models;
using ApplianceStoreIS.Services;

namespace ApplianceStoreIS.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly UserAccount currentUser;
        private readonly StoreDataService storeDataService;
        private readonly ObservableCollection<CartItem> cartItems;
        private ICollectionView productsView;

        public DashboardWindow(UserAccount user, AuthService authService, StoreDataService storeDataService)
        {
            InitializeComponent();
            currentUser = user;
            this.storeDataService = storeDataService;
            cartItems = new ObservableCollection<CartItem>();

            UserHeaderTextBlock.Text = $"Пользователь: {currentUser.FullName} | Роль: {currentUser.Role}";
            ProfileTextBlock.Text = $"Личный кабинет: {currentUser.FullName} ({currentUser.Login})";

            DataContext = storeDataService;
            CartDataGrid.ItemsSource = cartItems;
            OrdersDataGrid.ItemsSource = storeDataService.GetOrders();

            productsView = CollectionViewSource.GetDefaultView(storeDataService.Products);
            ProductsDataGrid.ItemsSource = productsView;

            SetupFilterAndSortOptions();
            ApplyAccessPolicy();
            ApplyFilters();
            UpdateCartSummary();
        }

        private void SetupFilterAndSortOptions()
        {
            var categories = new List<string> { "Все категории" };
            categories.AddRange(storeDataService.Products.Select(p => p.Category).Distinct().OrderBy(x => x));
            CategoryFilterComboBox.ItemsSource = categories;
            CategoryFilterComboBox.SelectedIndex = 0;

            SortComboBox.ItemsSource = new[]
            {
                "Без сортировки",
                "Название (А-Я)",
                "Название (Я-А)",
                "Цена (по возрастанию)",
                "Цена (по убыванию)"
            };
            SortComboBox.SelectedIndex = 0;
        }

        private void ApplyAccessPolicy()
        {
            bool canViewAllTables = PermissionService.CanViewAllTables(currentUser.Role);
            SuppliersTab.Visibility = canViewAllTables ? Visibility.Visible : Visibility.Collapsed;
            OrdersTab.Visibility = canViewAllTables ? Visibility.Visible : Visibility.Collapsed;

            AddProductButton.IsEnabled = PermissionService.CanAdd(currentUser.Role);
            EditProductButton.IsEnabled = PermissionService.CanEdit(currentUser.Role);
            DeleteProductButton.IsEnabled = PermissionService.CanDelete(currentUser.Role);
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var searchText = (SearchTextBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            var selectedCategory = CategoryFilterComboBox.SelectedItem?.ToString() ?? "Все категории";

            productsView.Filter = obj =>
            {
                var product = obj as Product;
                if (product == null)
                {
                    return false;
                }

                bool matchesSearch = string.IsNullOrWhiteSpace(searchText)
                    || product.Name.ToLowerInvariant().Contains(searchText)
                    || product.Brand.ToLowerInvariant().Contains(searchText);

                bool matchesCategory = selectedCategory == "Все категории" || product.Category == selectedCategory;
                return matchesSearch && matchesCategory;
            };
        }

        private void OnSortChanged(object sender, RoutedEventArgs e)
        {
            productsView.SortDescriptions.Clear();
            switch (SortComboBox.SelectedIndex)
            {
                case 1:
                    productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Name), ListSortDirection.Ascending));
                    break;
                case 2:
                    productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Name), ListSortDirection.Descending));
                    break;
                case 3:
                    productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Price), ListSortDirection.Ascending));
                    break;
                case 4:
                    productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Price), ListSortDirection.Descending));
                    break;
            }
        }

        private void OnAddProductClick(object sender, RoutedEventArgs e)
        {
            var editorWindow = new ProductEditorWindow { Owner = this };
            if (editorWindow.ShowDialog() == true)
            {
                var newProduct = editorWindow.Product;
                storeDataService.SaveProduct(newProduct);
                storeDataService.Products.Add(newProduct);
                RefreshCategories();
            }
        }

        private void OnEditProductClick(object sender, RoutedEventArgs e)
        {
            if (!(ProductsDataGrid.SelectedItem is Product selectedProduct))
            {
                MessageBox.Show("Выберите запись для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var copy = new Product
            {
                Id = selectedProduct.Id,
                Name = selectedProduct.Name,
                Category = selectedProduct.Category,
                Brand = selectedProduct.Brand,
                Price = selectedProduct.Price,
                Quantity = selectedProduct.Quantity
            };

            var editorWindow = new ProductEditorWindow(copy) { Owner = this };
            if (editorWindow.ShowDialog() == true)
            {
                selectedProduct.Name = copy.Name;
                selectedProduct.Category = copy.Category;
                selectedProduct.Brand = copy.Brand;
                selectedProduct.Price = copy.Price;
                selectedProduct.Quantity = copy.Quantity;
                storeDataService.SaveProduct(selectedProduct);
                productsView.Refresh();
                RefreshCategories();
            }
        }

        private void OnDeleteProductClick(object sender, RoutedEventArgs e)
        {
            if (!(ProductsDataGrid.SelectedItem is Product selectedProduct))
            {
                MessageBox.Show("Выберите запись для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить товар '{selectedProduct.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                storeDataService.DeleteProduct(selectedProduct.Id);
                storeDataService.Products.Remove(selectedProduct);
                RefreshCategories();
            }
        }

        private void OnAddToCartClick(object sender, RoutedEventArgs e)
        {
            if (!(ProductsDataGrid.SelectedItem is Product selectedProduct))
            {
                MessageBox.Show("Выберите товар для добавления в корзину.", "Корзина", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = cartItems.FirstOrDefault(c => c.ProductId == selectedProduct.Id);
            if (existing == null)
            {
                cartItems.Add(new CartItem
                {
                    ProductId = selectedProduct.Id,
                    ProductName = selectedProduct.Name,
                    UnitPrice = selectedProduct.Price,
                    Quantity = 1
                });
            }
            else
            {
                existing.Quantity += 1;
                CartDataGrid.Items.Refresh();
            }

            UpdateCartSummary();
        }

        private void OnRemoveFromCartClick(object sender, RoutedEventArgs e)
        {
            if (CartDataGrid.SelectedItem is CartItem selected)
            {
                cartItems.Remove(selected);
                UpdateCartSummary();
            }
        }

        private void OnCheckoutClick(object sender, RoutedEventArgs e)
        {
            if (!cartItems.Any())
            {
                MessageBox.Show("Корзина пуста.", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var item in cartItems)
            {
                storeDataService.AddOrder(item.ProductName, item.Quantity);
            }

            cartItems.Clear();
            OrdersDataGrid.ItemsSource = storeDataService.GetOrders();
            UpdateCartSummary();

            MessageBox.Show("Заказ(ы) успешно оформлен(ы).", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateCartSummary()
        {
            CartTotalTextBlock.Text = $"Итого: {cartItems.Sum(i => i.TotalPrice):N2} ₽";
        }

        private void RefreshCategories()
        {
            var selectedCategory = CategoryFilterComboBox.SelectedItem?.ToString();
            SetupFilterAndSortOptions();
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                CategoryFilterComboBox.SelectedItem = selectedCategory;
            }

            OrdersDataGrid.ItemsSource = storeDataService.GetOrders();
            ApplyFilters();
        }

        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}
