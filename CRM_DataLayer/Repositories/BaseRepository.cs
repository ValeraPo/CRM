using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_DataLayer.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        string _connectionString = null;

        public BaseRepository(string conn)
        {
            _connectionString = conn;
        }
        protected IDbConnection ProvideConnection() => new SqlConnection(_connectionString);

    }
}
