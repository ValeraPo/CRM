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

        public CRMProducer(ILeadService leadService, IAccountService accountService,
            ILogger<CRMProducer> logger)
        {
            _leadService = leadService;
            _accountService = accountService;
            _logger = logger;
        }

        public async Task NotifyLeadAdded(int id)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("rabbitmq://80.78.240.16", hst =>
                {
                    hst.Username("nafanya");
                    hst.Password("qwe!23");

                });

            });
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                var lead = await _leadService.GetById(id);

                await busControl.Publish<ILeadFullExchangeModel>(new
                {
                    Id = lead.Id,
                    Name = lead.Name,
                    LastName = lead.LastName,
                    BirthDate = lead.BirthDate,
                    Email = lead.Email,
                    Phone = lead.Phone,
                    Password = lead.Password,
                    City = lead.City,
                    Role = lead.Role,
                    IsBanned = lead.IsBanned
                });
                _logger.LogInformation("Lead published");
            }
            finally
            {
                _logger.LogWarning("Account not published");
                await busControl.StopAsync();
            }

            _logger.LogInformation("Lead published");
        }

        public async Task NotifyAccountAdded(int id)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("rabbitmq://80.78.240.16", hst =>
                {
                    hst.Username("nafanya");
                    hst.Password("qwe!23");

                });

            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                var lead = await _accountService.GetById(id);

                await busControl.Publish<IAccountExchangeModel>(new
                {
                    Id = lead.Id,
                    Name = lead.Name,
                    CurrencyType = lead.CurrencyType,
                    LeadId = lead.Lead.Id,
                    IsBlocked = lead.IsBlocked,
                    LockDate = lead.LockDate

                });
                _logger.LogInformation("Account published");

            }
            finally
            {
                _logger.LogWarning("Accoun not published");
                await busControl.StopAsync();
            }

        }
    }
}

