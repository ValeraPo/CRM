using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using System.Data;


namespace CRM.DataLayer.Repositories
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        private const string _updateProc = "dbo.Lead_Update";
        private const string _insertProc = "dbo.Lead_Insert";
        string _connectionString;

        public LeadRepository(string conn) : base(conn)
        {
            _connectionString = conn;
        }

        public int AddLead(Lead lead)
        {
            using IDbConnection connection = ProvideConnection();

            return connection.QueryFirstOrDefault<int>(
                    _insertProc,
                    new
                    {
                        Name = lead.Name,
                        LastName = lead.LastName,
                        DateBirth = lead.DateBirth,
                        Email = lead.Email,
                        Phone = lead.Phone,
                        Passord = lead.Password,
                        Role = lead.Role
                    },
                    commandType: CommandType.StoredProcedure
                );
        }

        public void UpdateLeadById(Lead lead)
        {
            using IDbConnection connection = ProvideConnection();

            connection.Execute(_updateProc,
                new
                {
                    lead.Id,
                    lead.Name,
                    lead.LastName,
                    lead.DateBirth,
                    lead.Email,
                    lead.Phone,
                    lead.Role
                },

                commandType: CommandType.StoredProcedure);
        }

    }
}


