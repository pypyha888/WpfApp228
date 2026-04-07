using ApplianceStoreIS.Models;
using ApplianceStoreIS.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplianceStoreIS.Tests
{
    [TestClass]
    public class AuthAndPermissionTests
    {
        [TestMethod]
        public void Register_ThenAuthenticate_User_ReturnsUser()
        {
            var authService = new AuthService(new InMemoryStoreRepository());
            bool registered = authService.Register("Иван Иванов", "ivanov", "Qwerty12", "Qwerty12", UserRole.User, string.Empty, out _);
            var user = authService.Authenticate("ivanov", "Qwerty12");

            Assert.IsTrue(registered);
            Assert.IsNotNull(user);
            Assert.AreEqual(UserRole.User, user.Role);
        }

        [TestMethod]
        public void Authenticate_WithValidAdminCredentials_ReturnsAdminUser()
        {
            var authService = new AuthService(new InMemoryStoreRepository());
            bool registered = authService.Register("Администратор", "admin", "Admin123!", "Admin123!", UserRole.Admin, AuthService.PrivilegedRoleCode, out _);
            var user = authService.Authenticate("admin", "Admin123!");

            Assert.IsTrue(registered);
            Assert.IsNotNull(user);
            Assert.AreEqual(UserRole.Admin, user.Role);
        }

        [TestMethod]
        public void Register_ManagerWithoutPrivilegedCode_ReturnsFalse()
        {
            var authService = new AuthService(new InMemoryStoreRepository());
            bool result = authService.Register("Менеджер", "manager1", "Qwerty12", "Qwerty12", UserRole.Manager, "", out string error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Contains("специальный код"));
        }

        [TestMethod]
        public void Register_WithDuplicateLogin_ReturnsFalse()
        {
            var authService = new AuthService(new InMemoryStoreRepository());
            authService.Register("Иван Иванов", "ivanov", "Qwerty12", "Qwerty12", UserRole.User, string.Empty, out _);

            bool duplicateResult = authService.Register("Иван Петров", "ivanov", "Qwerty12", "Qwerty12", UserRole.User, string.Empty, out string error);

            Assert.IsFalse(duplicateResult);
            Assert.IsTrue(error.Length > 0);
        }

        [TestMethod]
        public void Register_WithPasswordMismatch_ReturnsFalse()
        {
            var authService = new AuthService(new InMemoryStoreRepository());
            bool result = authService.Register("Пользователь", "user1", "Qwerty12", "Qwerty13", UserRole.User, string.Empty, out string error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Contains("не совпадают"));
        }

        [TestMethod]
        public void PermissionService_UserRole_CannotDelete()
        {
            Assert.IsFalse(PermissionService.CanDelete(UserRole.User));
        }

        [TestMethod]
        public void StoreDataService_HasNoSeedProductsByDefault()
        {
            var service = new StoreDataService(new InMemoryStoreRepository());
            Assert.AreEqual(0, service.Products.Count);
        }
    }
}
