using CRM.BusinessLayer.Helpers;
using CRM.BusinessLayer.Models.Paypal;
using CRM.BusinessLayer.Models.Paypal.Enums;
using CRM.BusinessLayer.Models.Paypal.Request;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Logging;

namespace CRM.BusinessLayer.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPaypalRequestHelper _paypalRequestHelper;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceService> _logger;

        private readonly SemaphoreSlim _paypalSemaphoreSlim;

        public InvoiceService(IAccountRepository accountRepository, 
            IPaypalRequestHelper paypalRequestHelper,
            IInvoiceRepository invoiceRepository,
            ILogger<InvoiceService> logger)
        {
            _accountRepository = accountRepository;
            _paypalRequestHelper = paypalRequestHelper;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _paypalSemaphoreSlim = new SemaphoreSlim(1);
        }

        public async Task<string> GetNewInvoiceUrlToPay(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID =  {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            _logger.LogInformation("Passed checks on account is belongs to lead");
            _logger.LogInformation("Creating draft invoice on paypal");
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
            _logger.LogInformation("Created draft invoice on paypal");

            var invoiceId = _paypalRequestHelper.GetInvoiceIdFromLink(createdInvoiceLink);
            var linkToPayment = await _paypalRequestHelper.SendInvoice(invoiceId,
                                      new SendInvoiceRequest { SendToInvoicer = false, SendToRecipient = false });
            _logger.LogInformation($"Setted invoice to payable state, and received link for lead to pay - {linkToPayment}");
            await _invoiceRepository.Add(
                new DataLayer.Entities.Invoice()
                {                   
                    Id = invoiceId,
                    Amount = transactionModel.Amount,
                    Status = DataLayer.Enums.InvoiceStatus.Unpaid
                }, 
                transactionModel.AccountId);
            _logger.LogInformation("Added information about invoice to database");

            return linkToPayment.Href;
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

        private async Task WaitForInvoiceGetPaidStatusAndMakeTransaction(string invoiceId, TransactionRequestModel transactionModel)
        {
            InvoiceResponse invoiceInfo;
            while (true)
            {
                invoiceInfo = await _paypalRequestHelper.GetInvoice(invoiceId);
                if (invoiceInfo.Status == InvoiceStatus.PAID.ToString())
                    break;
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
            var paimentId = invoiceInfo.Payments.Transactions.First().PaymentId;
            var payment = await _paypalRequestHelper.GetPayment(paimentId);

            var netAmount = Convert.ToDecimal(payment.SellerReceivableBreakdown.NetAmount);
            transactionModel.Amount = netAmount;

        }

    }
}
