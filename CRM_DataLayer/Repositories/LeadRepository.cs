using CRM_DataLayer.Repositories.Interfaces;
using Dapper;
using System.Data;


namespace CRM_DataLayer.Repositories
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        private const string _updateProc = "dbo.Lead_Update";
        private const string _insertProc = "dbo.Lead_Insert";

        public LeadRepository(string conn) : base(conn)
        {
        }

        public void AddLead(Lead lead)
        {
            using IDbConnection connection = ProvideConnection();

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

﻿
