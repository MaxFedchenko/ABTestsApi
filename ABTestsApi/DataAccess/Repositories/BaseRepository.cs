namespace ABTestsApi.DataAccess
{
    public abstract class BaseRepository
    {
        protected readonly IDatabaseManager DatabaseManager;

        public BaseRepository(IDatabaseManager databaseManager)
        {
            DatabaseManager = databaseManager;
        }
    }
}
