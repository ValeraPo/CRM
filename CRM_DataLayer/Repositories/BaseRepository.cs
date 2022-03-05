using CRM.DataLayer.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace CRM.DataLayer.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        string _connectionString;

        public BaseRepository(string conn)
        {
            _connectionString = conn;
        }
        protected IDbConnection ProvideConnection() => new SqlConnection(_connectionString);

    }
}
