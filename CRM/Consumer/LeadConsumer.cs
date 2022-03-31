using AutoMapper;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.ExchangeModels;
using MassTransit;

namespace CRM.APILayer.Consumer
{
    public class LeadConsumer : IConsumer<LeadShortExchangeModel>
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

        public async Task Consume(ConsumeContext<LeadShortExchangeModel> context)
        {
            _logger.LogInformation($"Getting lead {context.Message.Id}");
            var model = _mapper.Map<LeadModel>(context.Message);
            foreach (var item in model.GetType().GetProperties())
            {
                _logger.LogInformation($"{item.Name}: {item.GetValue(model)}");
            }
            _logger.LogInformation($"");
            await _leadService.ChangeRoleLead(context.Message.Id, (int)context.Message.Role);
        }
    }
}
