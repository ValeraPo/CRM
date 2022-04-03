using AutoMapper;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using MassTransit;

namespace CRM.APILayer.Consumer
{
    public class LeadConsumer : IConsumer<LeadShortExchangeModel[]>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<LeadConsumer> _logger;
        private readonly ILeadService _leadService;

        public LeadConsumer(IMapper mapper, ILogger<LeadConsumer> logger, ILeadService leadService)
        {
            _mapper = mapper;
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
