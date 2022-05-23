using Marvelous.Contracts.Configurations;
using MassTransit;

namespace CRM.APILayer.Consumers
{
    // When configs are changed, we change them here
    public class ConfigConsumer : IConsumer<CrmCfg>
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ConfigConsumer> _logger;

        public ConfigConsumer(ILogger<ConfigConsumer> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public Task Consume(ConsumeContext<CrmCfg> context)
        {
            _logger.LogInformation($"Configuration {context.Message.Key} change value {_config[context.Message.Key]} to {context.Message.Value}");
            _config[context.Message.Key] = context.Message.Value;
            return Task.CompletedTask;
        }
    }
}
