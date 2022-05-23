using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.EmailMessageModels;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using Marvelous.Contracts.RequestModels;
using MassTransit;

namespace CRM.APILayer.Producers
{
    // with the help of the RabbitMQ we notify other services
    public class CRMProducer : ICRMProducers
    {
        private readonly ILeadService _leadService;
        private readonly IAccountService _accountService;
        private readonly ILogger<CRMProducer> _logger;
        private readonly IBus _bus;

        public CRMProducer(ILeadService leadService, IAccountService accountService,
            ILogger<CRMProducer> logger, IBus bus)
        {
            _leadService = leadService;
            _accountService = accountService;
            _logger = logger;
            _bus = bus;
        }

        //When lead added
        public async Task NotifyLeadAdded(int id)
        {
            var lead = await _leadService.GetById(id);
            await NotifyLeadAdded(lead);
        }

        public async Task NotifyLeadAdded(LeadModel lead)
        {

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            _logger.LogInformation("Try publish lead");

            await _bus.Publish<LeadFullExchangeModel>(new
            {
                lead.Id,
                lead.Name,
                lead.LastName,
                lead.BirthDate,
                lead.Email,
                lead.Phone,
                lead.Password,
                lead.City,
                lead.Role,
                lead.IsBanned
            },
            source.Token);
            _logger.LogInformation("Lead published");
        }

        // When account added
        public async Task NotifyAccountAdded(int id)
        {
            var account = await _accountService.GetById(id);
            await NotifyAccountAdded(account);
        }


        public async Task NotifyAccountAdded(AccountModel account)
        {

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            _logger.LogInformation("Try publish account");

            await _bus.Publish<AccountExchangeModel>(new
            {
                account.Id,
                account.Name,
                account.CurrencyType,
                LeadId = account.Lead.Id,
                account.IsBlocked,
                account.LockDate

            },
            source.Token);
            _logger.LogInformation("Account published");
        }

        // When withdrawed
        public async Task NotifyWhithdraw(int leadId, TransactionRequestModel transaction)
        {

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            _logger.LogInformation("Try publish email message");
            var lead = await _leadService.GetById(leadId);
            var account = await _accountService.GetById(transaction.AccountId);

            await _bus.Publish<EmailInfoMessage>(new
            {
               EmailTo = lead.Email,
               NameTo = lead.Name,
               Subject = TransactionType.Withdraw.ToString(),
               TextMessage = $"There was a withdrawal of money in the amount of {transaction.Amount} " +
               $"{account.CurrencyType.ToString()} from account {account.Name}. " +
               $"If you did not do this, contact the support service."
            },
            source.Token);
            _logger.LogInformation("Message published");
        }

    }
}

