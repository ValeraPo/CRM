using CRM_DataLayer.Repositories.Interfaces;
using System.Data;


namespace CRM_DataLayer.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private const string _updateProc = "dbo.Lead_Update";
        private const string _insertProc = "dbo.Lead_Insert";
        //провести через DL connection_string
        public LeadRepository()
        {

        }

        public void AddLead(Lead lead)
        {
            connection.Execute(_insertProc,
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
                    Role = lead.Role
                },

                commandType: CommandType.StoredProcedure);
        }

    }
}

﻿using Dapper;
using System.Data;

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
