using CRM.DataLayer.Configuration;
using CRM.DataLayer.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace CRM.DataLayer.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        public string ConnectionString { get; set; }

        public BaseRepository(IOptions<DbConfiguration> options)
        {
            ConnectionString = options.Value.ConnectionString;
        }

        protected IDbConnection ProvideConnection() => new SqlConnection(ConnectionString);
    }
}
