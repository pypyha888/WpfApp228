namespace ApplianceStoreIS.Models
{
    public class UserAccount
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public UserRole Role { get; set; }
    }
}
