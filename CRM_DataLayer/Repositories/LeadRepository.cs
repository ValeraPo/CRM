using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using Marvelous.Contracts;
using Microsoft.Extensions.Options;
using NLog;
using System.Data;


namespace CRM.DataLayer.Repositories
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        private const string _updateProc = "dbo.Lead_Update";
        private const string _insertProc = "dbo.Lead_Insert";
        private const string _banProc = "dbo.Lead_Ban";
        private const string _selectById = "dbo.Lead_SelectById";
        private const string _selectByEmail = "dbo.Lead_SelectByEmail";
        private const string _selectAll = "dbo.Lead_SelectAll";
        private const string _changePassword = "dbo.Lead_ChangePassword";
        private static Logger _logger;

        public LeadRepository(IOptions<DbConfiguration> options) : base(options)
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public int AddLead(Lead lead)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");
            var id = connection.QueryFirstOrDefault<int>(
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
            _logger.Debug($"Лид с id = {id} добавлен в базу данных.");
            return id;
        }

        public void UpdateLeadById(Lead lead)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

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
            _logger.Debug($"Лид с id = {lead.Id} был обновлен.");
        }

        public void DeleteById(int id)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            connection.Execute(_banProc,
                new
                {
                    Id = id,
                    IsBanned = true
                },
                commandType: CommandType.StoredProcedure);
            _logger.Debug($"Лид с id = {id} был удален.");
        }

        public void RestoreById(int id)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");
            connection.Execute(_banProc,
                new
                {
                    Id = id,
                    IsBanned = false
                },
                commandType: CommandType.StoredProcedure);
            _logger.Debug($"Лид с id = {id} был восстановлен.");
        }

        public List<Lead> GetAll()
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            var leads =  connection.
                Query<Lead>(
                _selectAll,
                commandType: CommandType.StoredProcedure)
                .ToList();
            _logger.Debug($"Были возвращены все лиды");
            return leads;
        }

        public Lead GetById(int id)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            var lead = connection
                .Query<Lead, Account, Lead>(
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
                .FirstOrDefault();
            _logger.Debug($"Были возвращен лид с id = {id}");
            return lead;
        }

        public Lead GetByEmail(string email)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            var lead =  connection
                .QueryFirstOrDefault<Lead>(
                _selectByEmail,
                new { Email = email },
                commandType: CommandType.StoredProcedure);
            _logger.Debug($"Были возвращен лид с email = {email.Encryptor()}");
            return lead;
        }

        public void ChangePassword(int id, string hashPassword)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            connection
                .Execute(_changePassword,
                new
                {
                    id,
                    hashPassword,
                },
                commandType: CommandType.StoredProcedure);
            _logger.Debug($"Был изменен пароль у лида id {id}");
        }

    }
}


