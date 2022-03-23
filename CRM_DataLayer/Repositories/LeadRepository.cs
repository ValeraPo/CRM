using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using Marvelous.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
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
        private const string _selectAllEmails = "dbo.Lead_SelectAllEmails";
        private const string _changePassword = "dbo.Lead_ChangePassword";
        private readonly ILogger<LeadRepository> _logger;

        public LeadRepository(IOptions<DbConfiguration> options, ILogger<LeadRepository> logger) : base(options)
        {
            _logger = logger;
        }

        public async Task<int> AddLead(Lead lead)
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");
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
                        Role = Role.Regular
                    },
                    commandType: CommandType.StoredProcedure
                );
            _logger.LogDebug($"Лид с id = {id} добавлен в базу данных.");
            return id;
        }

        public async Task UpdateLeadById(Lead lead)
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");

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
            _logger.LogDebug($"Лид с id = {lead.Id} был обновлен.");
        }

        public async Task ChangeRoleLead(Lead lead)
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");

            await connection.ExecuteAsync(_changeRoleProc,
                new
                {
                    lead.Id,
                    lead.Role
                },

                commandType: CommandType.StoredProcedure);
            _logger.LogDebug($"Лид с id = {lead.Id} был обновлен.");
        }

        public async Task DeleteById(int id)
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");

            await connection.ExecuteAsync(_banProc,
                new
                {
                    Id = id,
                    IsBanned = true
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogDebug($"Лид с id = {id} был удален.");
        }

        public async Task RestoreById(int id)
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");
            await connection.ExecuteAsync(_banProc,
                new
                {
                    Id = id,
                    IsBanned = false
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogDebug($"Лид с id = {id} был восстановлен.");
        }

        public async Task<List<Lead>> GetAll()
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");

            var leads = connection.QueryAsync<Lead>(
                _selectAll,
                commandType: CommandType.StoredProcedure)
                .Result
                .ToList();
            _logger.LogDebug($"Были возвращены все лиды");
            return leads;
        }

        public async Task<List<string>> GetAllEmails()
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");

            var emails = connection.
                QueryAsync<string>(
                _selectAllEmails,
                commandType: CommandType.StoredProcedure)
                .Result
                .ToList();
            _logger.LogDebug($"Были возвращены все email");
            return emails;
        }

        public async Task<Lead> GetById(int id)
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");

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
            _logger.LogDebug($"Были возвращен лид с id = {id}");
            return lead;
        }

        public async Task<Lead> GetByEmail(string email)
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");

            var lead = await connection
                .QueryFirstOrDefaultAsync<Lead>(
                _selectByEmail,
                new { email },
                commandType: CommandType.StoredProcedure);
            _logger.LogDebug($"Были возвращен лид с email = {email.Encryptor()}");
            return lead;
        }

        public async Task ChangePassword(int id, string hashPassword)
        {
            _logger.LogDebug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogDebug("Произведено подключение к базе данных.");

            await connection.ExecuteAsync(_changePassword,
                new
                {
                    id,
                    hashPassword,
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogDebug($"Был изменен пароль у лида id {id}");
        }

    }
}


