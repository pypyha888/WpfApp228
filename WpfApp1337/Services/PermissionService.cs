using ApplianceStoreIS.Models;

namespace ApplianceStoreIS.Services
{
    public static class PermissionService
    {
        public static bool CanViewAllTables(UserRole role)
        {
            return role == UserRole.Admin || role == UserRole.Manager;
        }

        public static bool CanAdd(UserRole role)
        {
            return role == UserRole.Admin || role == UserRole.Manager;
        }

        public static bool CanEdit(UserRole role)
        {
            return role == UserRole.Admin || role == UserRole.Manager;
        }

        public static bool CanDelete(UserRole role)
        {
            return role == UserRole.Admin;
        }
    }
}
