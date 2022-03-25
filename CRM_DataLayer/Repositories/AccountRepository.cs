using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(IOptions<DbConfiguration> options, ILogger<AccountRepository> logger) : base(options)
        {
            _logger = logger;
        }

        public async Task<int> AddAccount(Account account)
        {
            _logger.LogInformation("Popytka podklucheniya k baze dannyh.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("Proizvedeno podkluchenie k baze dannyh.");

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
            _logger.LogInformation($"Account c id = {id} dobavlen v bazu dannyh.");
            return id;
        }

        public async Task UpdateAccountById(Account account)
        {
            _logger.LogInformation("Popytka podklucheniya k baze dannyh.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("Proizvedeno podkluchenie k baze dannyh.");

            await connection.ExecuteAsync(_updateProc,
                new
                {
                    account.Id,
                    account.Name
                },

                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Account c id = {account.Id} obnovlen.");

        }

        public async Task LockById(int id)
        {
            _logger.LogInformation("Popytka podklucheniya k baze dannyh.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("Proizvedeno podkluchenie k baze dannyh.");
            await connection.ExecuteAsync(_lockProc,
                new
                {
                    Id = id,
                    IsBlocked = true
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Account c id = {id} byl zablokirovan.");
        }

        public async Task UnlockById(int id)
        {
            _logger.LogInformation("Popytka podklucheniya k baze dannyh.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("Proizvedeno podkluchenie k baze dannyh.");
            await connection.ExecuteAsync(_lockProc,
                new
                {
                    Id = id,
                    IsBlocked = false
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Account c id = {id} byl razblokirovan.");
        }

        public async Task<List<Account>> GetByLead(int leadId)
        {
            _logger.LogInformation("Popytka podklucheniya k baze dannyh.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("Proizvedeno podkluchenie k baze dannyh.");

            var accounts =  connection.
                QueryAsync<Account>(
                _selectByLead,
                new { LeadId = leadId },
                commandType: CommandType.StoredProcedure)
                .Result
                .ToList();
            _logger.LogInformation($"Byli vozvracheny vse accounty leada c id = {leadId}");

            return accounts;
        }

        public async Task<Account> GetById(int id)
        {
            _logger.LogInformation("Popytka podklucheniya k baze dannyh.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("Proizvedeno podkluchenie k baze dannyh.");

            var account =  connection
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
            _logger.LogInformation($"Account c id = {id} byl vozvrachen.");
            return account;
        }

    }
}
