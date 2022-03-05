﻿using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using System.Data;


namespace CRM.DataLayer.Repositories
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        private const string _updateProc = "dbo.Lead_Update";
        private const string _insertProc = "dbo.Lead_Insert";
        private const string _banProc = "dbo.Lead_Ban";
        private const string _selectById = "dbo.Lead_SelectById";
        private const string _selectAll = "dbo.Lead_SelectAll";
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
                        BirthDate = lead.BirthDate,
                        Email = lead.Email,
                        Phone = lead.Phone,
                        Passord = lead.Password,
                        
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
                    lead.BirthDate,
                    lead.Phone,
                    lead.Role
                },

                commandType: CommandType.StoredProcedure);
        }

        public void DeleteById(int id)
        {
            using IDbConnection connection = ProvideConnection();
            connection.Execute(_banProc,
                new
                {
                    Id = id,
                    IsBanned = true
                },
                commandType: CommandType.StoredProcedure);
        }

        public void RestoreById(int id)
        {
            using IDbConnection connection = ProvideConnection();
            connection.Execute(_banProc,
                new
                {
                    Id = id,
                    IsBanned = false
                },
                commandType: CommandType.StoredProcedure);
        }

        public List<Lead> GetAll()
        {
            using IDbConnection connection = ProvideConnection();

            return connection.
                Query<Lead>(
                _selectAll, 
                commandType: CommandType.StoredProcedure)
                .ToList();
        }
        
        public Lead GetById(int id)
        {
            using IDbConnection connection = ProvideConnection();

            return connection
                .QueryFirstOrDefault<Lead>(
                _selectById,
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

    }
}


