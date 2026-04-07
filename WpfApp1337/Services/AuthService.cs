using System.Collections.Generic;
using System.Linq;
using ApplianceStoreIS.Models;

namespace ApplianceStoreIS.Services
{
    public class AuthService
    {
        public const string PrivilegedRoleCode = "ADMIN-STORE-2026";

        private readonly IStoreRepository repository;

        public AuthService(IStoreRepository repository)
        {
            this.repository = repository;
        }

        public bool Register(string fullName, string login, string password, UserRole role, out string error)
        {
            return Register(fullName, login, password, password, role, string.Empty, out error);
        }

        public bool Register(string fullName, string login, string password, string confirmPassword, UserRole role, out string error)
        {
            return Register(fullName, login, password, confirmPassword, role, string.Empty, out error);
        }

        public bool Register(string fullName, string login, string password, string confirmPassword, UserRole role, string privilegedCode, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3)
            {
                error = "ФИО должно содержать минимум 3 символа.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(login) || login.Length < 4)
            {
                error = "Логин должен содержать минимум 4 символа.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                error = "Пароль должен содержать минимум 6 символов.";
                return false;
            }

            if (password != confirmPassword)
            {
                error = "Пароль и подтверждение пароля не совпадают.";
                return false;
            }

            if ((role == UserRole.Admin || role == UserRole.Manager) && privilegedCode != PrivilegedRoleCode)
            {
                error = "Неверный специальный код для роли Администратор/Менеджер.";
                return false;
            }

            return repository.AddUser(new UserAccount
            {
                FullName = fullName.Trim(),
                Login = login.Trim(),
                Password = password,
                Role = role
            }, out error);
        }

        public UserAccount Authenticate(string login, string password)
        {
            return repository.GetUsers().FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public IReadOnlyList<UserAccount> GetAllUsers()
        {
            return repository.GetUsers().ToList();
        }
    }
}
