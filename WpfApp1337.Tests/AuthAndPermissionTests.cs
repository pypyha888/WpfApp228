using ApplianceStoreIS.Models;
using ApplianceStoreIS.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplianceStoreIS.Tests
{
    [TestClass]
    public class AuthAndPermissionTests
    {
        [TestMethod]
        public void Authenticate_WithValidAdminCredentials_ReturnsAdminUser()
        {
            var authService = new AuthService(new InMemoryStoreRepository());
            var user = authService.Authenticate("admin", "Admin123!");

            Assert.IsNotNull(user);
            Assert.AreEqual(UserRole.Admin, user.Role);
        }

        [TestMethod]
        public void Register_WithDuplicateLogin_ReturnsFalse()
        {
            var authService = new AuthService(new InMemoryStoreRepository());
            bool result = authService.Register("Новый пользователь", "admin", "Qwerty12", UserRole.User, out string error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Length > 0);
        }

        [TestMethod]
        public void Register_WithCorrectData_AddsUser()
        {
            var authService = new AuthService(new InMemoryStoreRepository());
            bool result = authService.Register("Иван Иванов", "ivanov", "Qwerty12", UserRole.User, out _);

            var user = authService.Authenticate("ivanov", "Qwerty12");
            Assert.IsTrue(result);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void PermissionService_UserRole_CannotDelete()
        {
            Assert.IsFalse(PermissionService.CanDelete(UserRole.User));
        }

        [TestMethod]
        public void StoreDataService_DefaultProducts_IsNotEmpty()
        {
            var service = new StoreDataService(new InMemoryStoreRepository());
            Assert.IsTrue(service.Products.Count >= 3);
        }
    }
}
