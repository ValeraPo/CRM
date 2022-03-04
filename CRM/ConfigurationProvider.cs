namespace CRM_APILayer
{
    public class ConfigurationProvider : iCofigurationProvider
    {
        private readonly string _connectionString;
        public ConfigurationProvider(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
