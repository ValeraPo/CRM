using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.ExchangeModels;
using MassTransit;

namespace CRM.APILayer.Producers
{
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

        public async Task NotifyLeadAdded(LeadModel lead)
        {

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            _logger.LogInformation("Try publish lead");
            //var lead = await _leadService.GetById(id);

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

        public async Task NotifyAccountAdded(AccountModel account)
        {

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            _logger.LogInformation("Try publish account");

            //var account = await _accountService.GetById(id);

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

    }
}

