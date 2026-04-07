using System.Windows;
using ApplianceStoreIS.Services;

namespace ApplianceStoreIS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService authService;
        private readonly StoreDataService storeDataService;

        public LoginWindow()
        {
            InitializeComponent();

            var repository = RepositoryFactory.Create(out bool usingDatabase);
            authService = new AuthService(repository);
            storeDataService = new StoreDataService(repository);

            if (!usingDatabase)
            {
                MessageBox.Show(
                    "Не удалось подключиться к LocalDB. Приложение запущено в резервном режиме (данные в памяти).",
                    "Предупреждение БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            var user = authService.Authenticate(LoginTextBox.Text.Trim(), PasswordBox.Password);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dashboardWindow = new DashboardWindow(user, authService, storeDataService);
            dashboardWindow.Show();
            Close();
        }

        private void OnOpenRegistrationClick(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow(authService);
            registrationWindow.Owner = this;
            registrationWindow.ShowDialog();
        }
    }
}
