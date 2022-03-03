using CRM_DataLayer.Repositories.Interfaces;
using System.Data;


namespace CRM_DataLayer.Repositories
{
    public class LeadRepositories : ILeadRepository
    {
        private const string _updateProc = "dbo.Lead_Update";
        //провести через DL connection_string
        public LeadRepositories()
        {

        }

        public void AddLead(Lead lead)
        {
            connection.Execute(_updateProc,
               new
               {
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
