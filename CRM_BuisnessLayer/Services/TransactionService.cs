using CRM.BusinessLayer.Helpers;
using CRM.BusinessLayer.Models.Paypal;
using CRM.BusinessLayer.Models.Paypal.Enums;
using CRM.BusinessLayer.Models.Paypal.Request;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<TransactionService> _logger;
        private readonly IPaypalRequestHelper _paypalRequestHelper;
        private readonly SemaphoreSlim _paypalSemaphoreSlim;

        public TransactionService(IAccountRepository accountRepository,
            IRequestHelper requestHelper,
            ILogger<TransactionService> logger,
            IPaypalRequestHelper paypalRequestHelper)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _requestHelper = requestHelper;
            _paypalRequestHelper = paypalRequestHelper;
            _paypalSemaphoreSlim = new SemaphoreSlim(1);
        }

        public async Task<string> AddDeposit(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID =  {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);

            Link createdInvoiceLink;
            await _paypalSemaphoreSlim.WaitAsync();
            try
            {
                var invoiceNumberResponse = await _paypalRequestHelper.GetNextInvoiceNumber();
                var draftInvoiceRequest = GetDraftInvoiceRequest(invoiceNumberResponse.InvoiceNumber,
                                                   entity.CurrencyType.ToString(),
                                                   transactionModel.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                createdInvoiceLink = await _paypalRequestHelper.CreateDraftInvoice(draftInvoiceRequest);
                int stop = 1;
            }
            finally
            {
                _paypalSemaphoreSlim.Release();
            }

            var invoiceId = _paypalRequestHelper.GetInvoiceIdFromLink(createdInvoiceLink);
            var linkToPayment = await _paypalRequestHelper.SendInvoice(invoiceId,
                                      new SendInvoiceRequest { SendToInvoicer = false, SendToRecipient = false });


            //_logger.LogInformation($"Send request.");
            //var response = await _requestHelper.SendRequest(TransactionEndpoints.Url, TransactionEndpoints.Deposit, Method.Post, transactionModel);
            //_logger.LogInformation($"Request successful.");

            return linkToPayment.Href;
        }

        public async Task<RestResponse> AddTransfer(TransferRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Transfer request received from account with ID {transactionModel.AccountIdFrom} to account with ID {transactionModel.AccountIdTo}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountIdFrom);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdFrom, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            var accountTo = await _accountRepository.GetById(transactionModel.AccountIdTo);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdTo, accountTo);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendRequest(TransactionEndpoints.Url, TransactionEndpoints.Transfer, Method.Post, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<RestResponse> Withdraw(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received withdrawal request from account with ID = {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendRequest(TransactionEndpoints.Url, TransactionEndpoints.Withdraw, Method.Post, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<decimal> GetBalance(List<int> ids, Currency currency)
        {
            _logger.LogInformation($"Received get balance request from account with ID = {String.Join(", ", ids.ToArray())}.");
            foreach (var id in ids)
            {
                var entity = await _accountRepository.GetById(id);
                ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            }
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.GetBalance(TransactionEndpoints.Url, ids, currency);
            _logger.LogInformation($"Request successful.");

            return Convert.ToDecimal(response.Content);
        }


        public async Task<RestResponse> GetTransactionsByAccountId(int id, int leadId)
        {
            _logger.LogInformation($"Popytka polucheniia transakcii accounta id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            _logger.LogInformation($"Otpravka zaprosa na poluchenie transakcii accounta id = {id}.");
            var response = await _requestHelper.GetTransactions(TransactionEndpoints.Url, "by-accountIds", id);
            _logger.LogInformation($"Poluchen otvet na poluchenie transakcii accounta id = {id}.");

            return response;
        }

        private InvoiceRequest GetDraftInvoiceRequest(string invoiceNumber, string currencyCode, string amount)
        {
            var invoiceRequest = new InvoiceRequest();

            var invoiceDetail = new InvoiceDetail
            {
                InvoiceNumber = invoiceNumber,
                CurrencyCode = currencyCode,
                PaymentTerm = new PaymentTerm() { TermType = TermType.DUE_ON_RECEIPT.ToString() }
            };
            invoiceRequest.InvoiceDetail = invoiceDetail;

            //TODO: fill invoicer fields
            var invoicer = new Invoicer
            {
                Email = "sb-3f243y15611093@business.example.com",
                Website = "https://piter-education.ru"
            };
            invoiceRequest.Invoicer = invoicer;

            var item = new Item
            {
                Name = "Deposit",
                Description = "Deposit to account",
                Quantity = "1",
                UnitAmount = new UnitAmount
                {
                    CurrencyCode = currencyCode,
                    Value = amount
                },
                UnitOfMeasure = UnitOfMeasure.AMOUNT.ToString()
            };
            invoiceRequest.Items = new List<Item>();
            invoiceRequest.Items.Add(item);

            return invoiceRequest;

        }
    }
}
