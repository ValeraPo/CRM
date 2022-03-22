using CRM.DataLayer.Configuration;
using CRM.DataLayer.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace CRM.DataLayer.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private string _connectionString;

        public BaseRepository(IOptions<DbConfiguration> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        protected IDbConnection ProvideConnection() => new SqlConnection(_connectionString);
    }
}
