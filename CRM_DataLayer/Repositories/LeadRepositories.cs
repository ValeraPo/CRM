using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_DataLayer.Repositories
{
    public class LeadRepositories
    {
        private const string _updateProc = "dbo.Lead_Update";
        //провести через DL connection_string
        public LeadRepositories()
        {

        }

        public void UpdateLeadById(Lead lead)
        {
            //вставить тут connection_string
            connection.Execute(_updateProc,
                new
                {
                    Id = lead.Id,
                    Name = lead.Name,
                    LastName = lead.LastName,
                    DateBirth = lead.DateBirth,
                    Email = lead.Email,
                    Phone = lead.Phone,
                    Password = lead.Password,
                    Role = lead.Role
                },

                commandType: CommandType.StoredProcedure);
        }

    }
}
