using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public LeadRepository(IOptions<DbConfiguration> options) : base(options)
        {
        }

        public int AddLead(Lead lead)
        {
            logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            logger.Debug("Произведено подключение к базе данных.");
            var id = connection.QueryFirstOrDefault<int>(
                    _insertProc,
                    new
                    {
                        Name = lead.Name,
                        LastName = lead.LastName,
                        BirthDate = lead.BirthDate,
                        Email = lead.Email,
                        Phone = lead.Phone,
                        Passord = lead.Password,
                        City = lead.City,
                        Role = Role.Regular
                    },
                    commandType: CommandType.StoredProcedure
                );
            logger.Debug($"Лид с id = {id} добавлен в базу данных.");
            return id;
        }

        public void UpdateLeadById(Lead lead)
        {
            logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            logger.Debug("Произведено подключение к базе данных.");

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
            logger.Debug($"Лид с id = {lead.Id} был обновлен.");
        }

        public void DeleteById(int id)
        {
            logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            logger.Debug("Произведено подключение к базе данных.");

            connection.Execute(_banProc,
                new
                {
                    Id = id,
                    IsBanned = true
                },
                commandType: CommandType.StoredProcedure);
            logger.Debug($"Лид с id = {id} был удален.");
        }

        public void RestoreById(int id)
        {
            logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            logger.Debug("Произведено подключение к базе данных.");
            connection.Execute(_banProc,
                new
                {
                    Id = id,
                    IsBanned = false
                },
                commandType: CommandType.StoredProcedure);
            logger.Debug($"Лид с id = {id} был восстановлен.");
        }

        public List<Lead> GetAll()
        {
            logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            logger.Debug("Произведено подключение к базе данных.");

            var leads =  connection.
                Query<Lead>(
                _selectAll,
                commandType: CommandType.StoredProcedure)
                .ToList();
            logger.Debug($"Были возвращены все лиды");
            return leads;
        }

        public Lead GetById(int id)
        {
            logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            logger.Debug("Произведено подключение к базе данных.");

            var lead = connection
                .Query<Lead, Account, Lead>(
                _selectById,
                (lead, account) =>
                {
                    lead.Accounts.Add(account);
                    return lead;
                },
                new 
                { 
                    Id = id 
                },
                splitOn: "LeadId",
                commandType: CommandType.StoredProcedure).
                FirstOrDefault();
            logger.Debug($"Были возвращен лид с id = {id}");
            return lead;
        }

        public Lead GetByEmail(string email)
        {
            logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            logger.Debug("Произведено подключение к базе данных.");

            var lead =  connection
                .QueryFirstOrDefault<Lead>(
                _selectByEmail,
                new { Email = email },
                commandType: CommandType.StoredProcedure);
            logger.Debug($"Были возвращен лид с email = {email.Encryptor()}");
            return lead;
        }

        public void ChangePassword(int id, string hashPassword)
        {
            logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            logger.Debug("Произведено подключение к базе данных.");

            connection
                .Execute(_changePassword,
                new
                {
                    id,
                    hashPassword,
                },
                commandType: CommandType.StoredProcedure);
            logger.Debug($"Были изменен пароль у лида id {id}");
        }

    }
}


