using System.Globalization;
using System.Windows;
using ApplianceStoreIS.Models;

namespace ApplianceStoreIS.Views
{
    public partial class ProductEditorWindow : Window
    {
        public Product Product { get; private set; }

        public ProductEditorWindow(Product product = null)
        {
            InitializeComponent();
            Product = product ?? new Product();

            if (product != null)
            {
                NameTextBox.Text = product.Name;
                CategoryTextBox.Text = product.Category;
                BrandTextBox.Text = product.Brand;
                PriceTextBox.Text = product.Price.ToString(CultureInfo.InvariantCulture);
                QuantityTextBox.Text = product.Quantity.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(CategoryTextBox.Text) || string.IsNullOrWhiteSpace(BrandTextBox.Text))
            {
                MessageBox.Show("Название, категория и бренд обязательны.", "Ошибка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом.", "Ошибка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Количество должно быть целым неотрицательным числом.", "Ошибка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Product.Name = NameTextBox.Text.Trim();
            Product.Category = CategoryTextBox.Text.Trim();
            Product.Brand = BrandTextBox.Text.Trim();
            Product.Price = price;
            Product.Quantity = quantity;

            DialogResult = true;
            Close();
        }
    }
}
