using AutoMapper;
using CRM.APILayer.Controllers;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.APILayer.Validation;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Xunit;

namespace CRM.ApiLayer.Tests
{
    public class Tests
    {
        private Mock<IAccountService> _accountService;
        private readonly IMapper _autoMapper;
        private readonly Mock<ILogger<AccountsController>> _logger;
        private readonly Mock<ICRMProducers> _crmProducers;
        private readonly Mock<IRequestHelper> _requestHelper;
        private readonly Mock<IConfiguration> _configuration;
        private readonly IValidator<AccountInsertRequest> _validatorAccountInsertRequest;
        private readonly IValidator<AccountUpdateRequest> _validatorAccountUpdateRequest;

        public Tests()
        {
            _accountService = new Mock<IAccountService>();
            _autoMapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));
            _logger = new Mock<ILogger<AccountsController>>();
            _crmProducers = new Mock<ICRMProducers>();
            _requestHelper = new Mock<IRequestHelper>();
            _configuration = new Mock<IConfiguration>();
            _validatorAccountInsertRequest = new AccountInsertRequestValidator();
            _validatorAccountUpdateRequest = new AccountUpdateRequestValidator();
        }
        

        
        [Test]
        public async Task InvalidModelTest()
        {

            // Arrange
            var model = new AccountInsertRequest { Name = "" }; // Invalid model
            var controller = new AccountsController(_accountService.Object,
                _autoMapper,
                _logger.Object,
                _crmProducers.Object,
                _requestHelper.Object,
                _configuration.Object,
                _validatorAccountInsertRequest,
                _validatorAccountUpdateRequest);

            // Have to explictly add this
            //controller.ModelState.AddModelError("Slug", "Required");

            // Act
            //var result = await controller.AddAccount(model);

            // Assert etc
            Assert.ThrowsAsync<ValidationException>(async () => await controller.AddAccount(model));

        }

    }
}