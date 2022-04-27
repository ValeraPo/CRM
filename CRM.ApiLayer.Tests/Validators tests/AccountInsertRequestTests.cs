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
    public class AccountInsertRequestTests
    {
        private Mock<IAccountService> _accountService;
        private readonly IMapper _autoMapper;
        private Mock<ILogger<AccountsController>> _logger;
        private Mock<ICRMProducers> _crmProducers;
        private Mock<IRequestHelper> _requestHelper;
        private readonly IValidator<AccountInsertRequest> _validatorAccountInsertRequest;
        private readonly IValidator<AccountUpdateRequest> _validatorAccountUpdateRequest;
        private AccountsController _controller;


        public AccountInsertRequestTests()
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
        public void AddAccount_ModelIsEmpty_ShouldThrowBadRequestException()
        {
            // given
            var token = "token";
            AccountInsertRequest model = null; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel());
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "You must specify the table details in the request body";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await _controller.AddAccount(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddAccount_NameIsEmpty_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = new AccountInsertRequest { Name = "", CurrencyType = 2 }; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel());
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- Name: Name is empty Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddAccount(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddAccount_NameIsTooLong_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = new AccountInsertRequest { Name = "12345678901234567890123", CurrencyType = 88 }; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel());
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- Name: Maximum lenght of Name 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddAccount(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddAccount_CurrencyTypeIsLessDiapason_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = new AccountInsertRequest { Name = "Money", CurrencyType = -1 }; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel());
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- CurrencyType: Currency Type is from 1 to 113 Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddAccount(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddAccount_CurrencyTypeIsEmpty_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = new AccountInsertRequest { Name = "Money", CurrencyType = 0 }; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel());
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- CurrencyType: Currency Type is empty Severity: Error\r\n -- CurrencyType: Currency Type is from 1 to 113 Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddAccount(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddAccount_CurrencyTypeIsGreaterDiapason_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = new AccountInsertRequest { Name = "Money", CurrencyType = 200 }; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel());
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- CurrencyType: Currency Type is from 1 to 113 Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddAccount(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }
    }
}
