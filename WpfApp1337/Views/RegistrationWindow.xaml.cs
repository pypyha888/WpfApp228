using System.Windows;
using System.Windows.Controls;
using ApplianceStoreIS.Models;
using ApplianceStoreIS.Services;

namespace ApplianceStoreIS.Views
{
    public partial class RegistrationWindow : Window
    {
        private readonly AuthService authService;

        public RegistrationWindow(AuthService authService)
        {
            InitializeComponent();
            this.authService = authService;
        }

        private void OnRegisterClick(object sender, RoutedEventArgs e)
        {
            var selectedRole = (RoleComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "User";
            var role = (UserRole)System.Enum.Parse(typeof(UserRole), selectedRole);

            if (authService.Register(
                FullNameTextBox.Text,
                LoginTextBox.Text,
                PasswordBox.Password,
                ConfirmPasswordBox.Password,
                UserRole.User,
                string.Empty,
                role,
                PrivilegedCodePasswordBox.Password,
                out string error))
            {
                MessageBox.Show("Регистрация выполнена успешно.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show(error, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
