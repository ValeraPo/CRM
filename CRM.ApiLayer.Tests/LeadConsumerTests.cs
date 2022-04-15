using CRM.APILayer.Consumer;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.ExchangeModels;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CRM.ApiLayer.Tests
{
    public class LeadConsumerTests
    {
        private Mock<ILogger<LeadConsumer>> _logger;
        private Mock<ILeadService> _leadService;
        private LeadConsumer sut;

       

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<LeadConsumer>>();
            _leadService = new Mock<ILeadService>();
            sut = new LeadConsumer(_logger.Object, _leadService.Object);
        }

        [Test]
        public async Task Consume_ShouldChangeRole()
        {
            //given
            

            //when
            //await sut.Consume(It.IsAny<ConsumeContext<LeadShortExchangeModel[]>>());

            ////then
            //_leadService.ChangeRoleListLead(It.IsAny<LeadShortExchangeModel[]>());
            //VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }
    }
}
