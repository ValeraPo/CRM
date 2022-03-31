using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;


namespace CRM.DataLayer.Repositories
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        private const string _updateProc = "dbo.Lead_Update";
        private const string _changeRoleProc = "dbo.Lead_ChangeRole";
        private const string _insertProc = "dbo.Lead_Insert";
        private const string _banProc = "dbo.Lead_Ban";
        private const string _selectById = "dbo.Lead_SelectById";
        private const string _selectByEmail = "dbo.Lead_SelectByEmail";
        private const string _selectAll = "dbo.Lead_SelectAll";
        private const string _selectAllToAuth = "dbo.Lead_SelectAllToAuth";
        private const string _changePassword = "dbo.Lead_ChangePassword";
        private readonly ILogger<LeadRepository> _logger;

        public LeadRepository(IOptions<DbConfiguration> options, ILogger<LeadRepository> logger) : base(options)
        {
            _logger = logger;
        }

        public async Task<int> AddLead(Lead lead)
        {
            _logger.LogInformation("Try to connection DB");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");
            var id = await connection.QueryFirstOrDefaultAsync<int>(
                    _insertProc,
                    new
                    {
                        lead.Name,
                        lead.LastName,
                        lead.BirthDate,
                        lead.Email,
                        lead.Phone,
                        lead.Password,
                        lead.City,
                        Role = Role.Regular,
                        IsBanned = false
                    },
                    commandType: CommandType.StoredProcedure
                );
            _logger.LogInformation($"Lead with ID = {id} added to DB.");
            return id;
        }

        public async Task UpdateLeadById(Lead lead)
        {
            _logger.LogInformation("Try to connection DB");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            await connection.ExecuteAsync(_updateProc,
                new
                {
                    lead.Id,
                    lead.Name,
                    lead.LastName,
                    lead.BirthDate,
                    lead.Phone,
                },

                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Lead with ID = {lead.Id} updated.");
        }

        public async Task ChangeRoleLead(Lead lead)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            await connection.ExecuteAsync(_changeRoleProc,
                new
                {
                    lead.Id,
                    lead.Role
                },

                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Lead role with id = {lead.Id} updated.");
        }

        public async Task DeleteById(int id)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            await connection.ExecuteAsync(_banProc,
                new
                {
                    Id = id,
                    IsBanned = true
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Lead with ID = {id} deleted.");
        }

        public async Task RestoreById(int id)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");
            await connection.ExecuteAsync(_banProc,
                new
                {
                    Id = id,
                    IsBanned = false
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Lead with ID = {id} restored.");
        }

        public async Task<List<Lead>> GetAll()
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            var leads = connection.QueryAsync<Lead>(
                _selectAll,
                commandType: CommandType.StoredProcedure)
                .Result
                .ToList();
            _logger.LogInformation($"All leads returned.");
            return leads;
        }

        public async Task<List<LeadAuthExchangeModel>> GetAllToAuth()
        {
            _logger.LogInformation("Popytka podklucheniya k baze dannyh.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("Proizvedeno podkluchenie k baze dannyh.");

            var leads = connection.QueryAsync<LeadAuthExchangeModel>(
                _selectAllToAuth,
                commandType: CommandType.StoredProcedure)
                .Result
                .ToList();
            _logger.LogInformation($"Byly vozvracheny vse leady");
            return leads;
        }

        public async Task<Lead> GetById(int id)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            var lead = connection
                .QueryAsync<Lead, Account, Lead>(
                _selectById,
                (lead, account) =>
                {
                    lead.Accounts = new List<Account>();
                    lead.Accounts.Add(account);
                    return lead;
                },
                new { Id = id },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure)
                .Result
                .FirstOrDefault();
            _logger.LogInformation($"Lead with ID = {id} get returned.");
            return lead;
        }

        public async Task<Lead> GetByEmail(string email)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            var lead = await connection
                .QueryFirstOrDefaultAsync<Lead>(
                _selectByEmail,
                new { email },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Lead with Email = {email.Encryptor()} get returned.");
            return lead;
        }

        public async Task ChangePassword(int id, string hashPassword)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            await connection.ExecuteAsync(_changePassword,
                new
                {
                    id,
                    Password = hashPassword,
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Lead with ID = {id} changed password");
        }

    }
}


