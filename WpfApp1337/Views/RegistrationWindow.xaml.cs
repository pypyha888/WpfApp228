using System.Windows;
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
            if (authService.Register(
                FullNameTextBox.Text,
                LoginTextBox.Text,
                PasswordBox.Password,
                ConfirmPasswordBox.Password,
                UserRole.User,
                string.Empty,
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
