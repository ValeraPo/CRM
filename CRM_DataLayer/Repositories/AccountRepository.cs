using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;
using NLog;
using System.Data;

namespace CRM.DataLayer.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private const string _insertProc = "dbo.Account_Insert";
        private const string _lockProc = "dbo.Account_Lock";
        private const string _selectById = "dbo.Account_SelectById";
        private const string _selectByLead = "dbo.Account_SelectByLead";
        private const string _updateProc = "dbo.Account_Update";
        private static Logger _logger;

        public AccountRepository(IOptions<DbConfiguration> options) : base(options)
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public int AddAccount(Account account)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            var id =  connection.QueryFirstOrDefault<int>(
                    _insertProc,
                    new
                    {
                        account.Name,
                        account.CurrencyType,
                        LeadId = account.Lead.Id
                    },
                    commandType: CommandType.StoredProcedure
                );
            _logger.Debug($"Аккаунт с id = {id} добавлен в базу данных.");
            return id;
        }

        public void UpdateAccountById(Account account)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            connection.Execute(_updateProc,
                new
                {
                    account.Id,
                    account.Name
                },

                commandType: CommandType.StoredProcedure);
            _logger.Debug($"Аккаунт с id = {account.Id} обновлен.");

        }

        public void LockById(int id)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");
            connection.Execute(_lockProc,
                new
                {
                    Id = id,
                    IsBlocked = true
                },
                commandType: CommandType.StoredProcedure);
            _logger.Debug($"Аккаунт с id = {id} был заблокирован.");
        }

        public void UnlockById(int id)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");
            connection.Execute(_lockProc,
                new
                {
                    Id = id,
                    IsBlocked = false
                },
                commandType: CommandType.StoredProcedure);
            _logger.Debug($"Аккаунт с id = {id} был разблокирован.");
        }

        public List<Account> GetByLead(int leadId)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            var accounts =  connection.
                Query<Account>(
                _selectByLead,
                new { LeadId = leadId },
                commandType: CommandType.StoredProcedure)
                .ToList();
            _logger.Debug($"Были возвращены все аккаунты лида с id = {leadId}");

            return accounts;
        }

        public Account GetById(int id)
        {
            _logger.Debug("Попытка подключения к базе данных.");
            using IDbConnection connection = ProvideConnection();
            _logger.Debug("Произведено подключение к базе данных.");

            var account =  connection
                .QueryFirstOrDefault<Account>(
                _selectById,
                new { Id = id },
                commandType: CommandType.StoredProcedure);
            _logger.Debug($"Аккаунт с id = {id} был возвращен.");

            return account;
        }

    }
}
