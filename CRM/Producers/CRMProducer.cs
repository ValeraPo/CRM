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

        public async Task NotifyLeadAdded(int id)
        {

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            _logger.LogInformation("Try publish lead");
            var lead = await _leadService.GetById(id);

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

        public async Task NotifyAccountAdded(int id)
        {

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            _logger.LogInformation("Try publish account");

            var lead = await _accountService.GetById(id);

            await _bus.Publish<AccountExchangeModel>(new
            {
                lead.Id,
                lead.Name,
                lead.CurrencyType,
                LeadId = lead.Lead.Id,
                lead.IsBlocked,
                lead.LockDate

            },
            source.Token);
            _logger.LogInformation("Account published");
        }

    }
}

