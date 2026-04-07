namespace ApplianceStoreIS.Services
{
    public static class RepositoryFactory
    {
        public static IStoreRepository Create(out bool usingDatabase)
        {
            try
            {
                usingDatabase = true;
                return new SqlStoreRepository();
            }
            catch
            {
                usingDatabase = false;
                return new InMemoryStoreRepository();
            }
        }
    }
}
