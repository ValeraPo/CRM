using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
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

        public AccountRepository(IOptions<DbConfiguration> options) : base(options)
        {
        }

        public int AddAccount(Account account)
        {
            using IDbConnection connection = ProvideConnection();

            return connection.QueryFirstOrDefault<int>(
                    _insertProc,
                    new
                    {
                        Name = account.Name,
                        CurrencyType = account.CurrencyType,
                        LeadId = account.Lead.Id,
                        IsBlocked = false
                    },
                    commandType: CommandType.StoredProcedure
                );
        }

        public void UpdateAccountById(Account account)
        {
            using IDbConnection connection = ProvideConnection();

            connection.Execute(_updateProc,
                new
                {
                    account.Id,
                    account.Name
                },

                commandType: CommandType.StoredProcedure);
        }

        public void LockById(int id)
        {
            using IDbConnection connection = ProvideConnection();
            connection.Execute(_lockProc,
                new
                {
                    Id = id,
                    IsBlocked = true,
                    LockDate = DateTime.Today
                },
                commandType: CommandType.StoredProcedure);
        }

        public void UnlockById(int id)
        {
            using IDbConnection connection = ProvideConnection();
            connection.Execute(_lockProc,
                new
                {
                    Id = id,
                    IsBlocked = false,
                    LockDate = DateTime.Now
                },
                commandType: CommandType.StoredProcedure);
        }

        public List<Account> GetByLead(int leadId)
        {
            using IDbConnection connection = ProvideConnection();

            return connection.
                Query<Account>(
                _selectByLead,
                new { LeadId = leadId },
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public Account GetById(int id)
        {
            using IDbConnection connection = ProvideConnection();

            return connection
                .QueryFirstOrDefault<Account>(
                _selectById,
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

    }
}
