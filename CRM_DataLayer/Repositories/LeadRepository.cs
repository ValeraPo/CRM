using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_DataLayer.Repositories
{
    public class LeadRepository : BaseRepository 
    {
        private const string _updateProc = "dbo.Lead_Update";

        public LeadRepository(string conn) : base(conn)
        {
        }

        public void UpdateLeadById(Lead lead)
        {
            using IDbConnection connection = ProvideConnection();

            connection.Execute(_updateProc,
                new
                {
                    Id = lead.Id,
                    Name = lead.Name,
                    LastName = lead.LastName,
                    DateBirth = lead.DateBirth,
                    Email = lead.Email,
                    Phone = lead.Phone,
                    Role = lead.Role
                },

                commandType: CommandType.StoredProcedure);
        }

    }
}
