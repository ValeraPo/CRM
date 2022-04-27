using AutoMapper;
using CRM.APILayer.Configuration;
using CRM.APILayer.Controllers;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.APILayer.Validation;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Services.Interfaces;
using FluentValidation;
using Marvelous.Contracts.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CRM.ApiLayer.Tests
{
    public class AccountUpdateRequestTests
    {
        private Mock<IAccountService> _accountService;
        private readonly IMapper _autoMapper;
        private Mock<ILogger<AccountsController>> _logger;
        private Mock<ICRMProducers> _crmProducers;
        private Mock<IRequestHelper> _requestHelper;
        private readonly IValidator<AccountInsertRequest> _validatorAccountInsertRequest;
        private readonly IValidator<AccountUpdateRequest> _validatorAccountUpdateRequest;
        private AccountsController _controller;


        public AccountUpdateRequestTests()
        {

            _autoMapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperFromApi>()));
            _validatorAccountInsertRequest = new AccountInsertRequestValidator();
            _validatorAccountUpdateRequest = new AccountUpdateRequestValidator();
        }

        [SetUp]
        public void Setup()
        {
            _accountService = new Mock<IAccountService>();
            _logger = new Mock<ILogger<AccountsController>>();
            _crmProducers = new Mock<ICRMProducers>();
            _requestHelper = new Mock<IRequestHelper>();
            _controller = new AccountsController(_accountService.Object,
                _autoMapper,
                _logger.Object,
                _crmProducers.Object,
                _requestHelper.Object,
                _validatorAccountInsertRequest,
                _validatorAccountUpdateRequest);
        }

        [Test]
        public void UpdateAccount_ModelIsEmpty_ShouldThrowBadRequestException()
        {
            // given
            var token = "token";
            AccountUpdateRequest model = null; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "You must specify the table details in the request body";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await _controller.UpdateAccount(It.IsAny<int>(), model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateAccount_NameIsEmpty_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = new AccountUpdateRequest { Name = "" }; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- Name: Name is empty Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.UpdateAccount(It.IsAny<int>(), model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateAccount_NameIsTooLong_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = new AccountUpdateRequest { Name = "12345678901234567890123" }; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- Name: Maximum lenght of Name 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.UpdateAccount(It.IsAny<int>(), model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }
    }
}
