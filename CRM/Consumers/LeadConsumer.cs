using AutoMapper;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.ExchangeModels;
using MassTransit;

namespace CRM.APILayer.Consumers
{
    public class LeadConsumer : IConsumer<LeadShortExchangeModel[]>
    {
        private readonly ILogger<LeadConsumer> _logger;
        private readonly ILeadService _leadService;

        public LeadConsumer(ILogger<LeadConsumer> logger, ILeadService leadService)
        {
            _logger = logger;
            _leadService = leadService;
        }

        public async Task Consume(ConsumeContext<LeadShortExchangeModel[]> context)
        {
            _logger.LogInformation($"Getting list of lead  ");
             await _leadService.ChangeRoleListLead(context.Message);
            _logger.LogInformation($"All roles was changed ");
        }
    }
}
