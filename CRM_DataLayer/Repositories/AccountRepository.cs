using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(IOptions<DbConfiguration> options, ILogger<AccountRepository> logger) : base(options)
        {
            _logger = logger;
        }

        public async Task<int> AddAccount(Account account)
        {
            _logger.LogInformation("Try to connection DB.");
            // Connecting with data base
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            var id = await connection.QueryFirstOrDefaultAsync<int>(
                    _insertProc,
                    new
                    {
                        account.Name,
                        account.CurrencyType,
                        LeadId = account.Lead.Id
                    },
                    commandType: CommandType.StoredProcedure
                );
            _logger.LogInformation($"Account with ID = {id} successfully added.");
            return id;
        }

        public async Task UpdateAccountById(Account account)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            await connection.ExecuteAsync(_updateProc,
                new
                {
                    account.Id,
                    account.Name
                },

                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Account with ID = {account.Id} successfulle upated.");

        }

        public async Task LockById(int id)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");
            await connection.ExecuteAsync(_lockProc,
                new
                {
                    Id = id,
                    IsBlocked = true
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Account with ID = {id} locked.");
        }

        public async Task UnlockById(int id)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");
            await connection.ExecuteAsync(_lockProc,
                new
                {
                    Id = id,
                    IsBlocked = false
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Account with ID = {id} restored.");
        }

        public async Task<List<Account>> GetByLead(int leadId)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            var accounts = connection.
                QueryAsync<Account>(
                _selectByLead,
                new { LeadId = leadId },
                commandType: CommandType.StoredProcedure)
                .Result
                .ToList();
            _logger.LogInformation($"All account lead with ID = {leadId} returned");

            return accounts;
        }

        public async Task<Account> GetById(int id)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            var account = connection
                .QueryAsync<Account, Lead, Account>(
                _selectById,
                (account, lead) =>
                {
                    account.Lead = lead;
                    return account;
                },
                new { Id = id },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure)
                .Result
                .FirstOrDefault();
            _logger.LogInformation($"Account with ID = {id} returned.");
            return account;
        }

    }
}
