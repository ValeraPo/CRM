using CRM.DataLayer.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace CRM.DataLayer.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        public string ConnectionString { get; set; }
        

        protected IDbConnection ProvideConnection() => new SqlConnection(ConnectionString);

    }
}
