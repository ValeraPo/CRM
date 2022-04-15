using CRM.DataLayer.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Enums;
using CRM.DataLayer.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;

namespace CRM.DataLayer.Repositories
{
    public class InvoiceRepository : BaseRepository, IInvoiceRepository
    {
        private readonly ILogger<InvoiceRepository> _logger;
        private const string _selectByIdProc = "dbo.Invoice_SelectById";
        private const string _selectAllProc = "dbo.Invoice_SelectAll";
        private const string _insertProc = "dbo.Invoice_SelectById";
        private const string _updateStatusProc = "dbo.Invoice_UpdateStatus";

        public InvoiceRepository(IOptions<DbConfiguration> options, ILogger<InvoiceRepository> logger) : base(options)
        {
            _logger = logger;
        }

        public async Task<Invoice> GetById(string id)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            var invoiceInDb = connection
                .QueryAsync<Invoice, Account, Invoice>(
                _selectByIdProc,
                (invoice, account) =>
                {
                    invoice.Account = account;
                    return invoice;
                },
                new { Id = id },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure)
                .Result
                .FirstOrDefault();
            _logger.LogInformation($"Invoice with ID = {id} get returned.");
            return invoiceInDb;
        }

        public async Task<List<Invoice>> GetAll()
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            var invoices = connection.QueryAsync<Invoice>(
                _selectAllProc,
                commandType: CommandType.StoredProcedure)
                .Result
                .ToList();
            _logger.LogInformation($"All invoices returned.");
            return invoices;
        }

        public async Task<int> Add(Invoice invoice, int accountId)
        {
            _logger.LogInformation("Try to connection DB");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");
            var id = await connection.ExecuteAsync(
                    _insertProc,
                    new
                    {
                        invoice.Id,
                        AccountId = accountId
                        invoice.Amount,
                        invoice.Status
                    },
                    commandType: CommandType.StoredProcedure
                );
            _logger.LogInformation($"Invoice with ID = {invoice.Id} added to DB.");
            return id;
        }

        public async Task UpdateStatus(string id, InvoiceStatus status)
        {
            _logger.LogInformation("Try to connection DB.");
            using IDbConnection connection = ProvideConnection();
            _logger.LogInformation("DB connection established successfully.");

            await connection.ExecuteAsync(_updateStatusProc,
                new
                {
                    Id = id,
                    Status = status,
                },
                commandType: CommandType.StoredProcedure);
            _logger.LogInformation($"Invoice with ID = {id} changed status to {status} ({(int)status})");
        }
    }
}
