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
    public class LeadChangePasswordRequestTests
    {
        private Mock<ILeadService> _leadService;
        private readonly IMapper _autoMapper;
        private Mock<ILogger<LeadsController>> _logger;
        private Mock<ICRMProducers> _crmProducers;
        private Mock<IRequestHelper> _requestHelper;
        private readonly IValidator<LeadInsertRequest> _validatorLeadInsertRequest;
        private readonly IValidator<LeadUpdateRequest> _validatorLeadUpdateRequest;
        private readonly IValidator<LeadChangePasswordRequest> _validatorLeadChangePasswordRequest;
        private LeadsController _controller;


        public LeadChangePasswordRequestTests()
        {

            _autoMapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperFromApi>()));
            _validatorLeadInsertRequest = new LeadInsertRequestValidator();
            _validatorLeadUpdateRequest = new LeadUpdateRequestValidator();
            _validatorLeadChangePasswordRequest = new LeadChangePasswordRequestValidator();
        }

        [SetUp]
        public void Setup()
        {
            _crmProducers = new Mock<ICRMProducers>();
            _leadService = new Mock<ILeadService>();
            _logger = new Mock<ILogger<LeadsController>>();
            _requestHelper = new Mock<IRequestHelper>();
            _controller = new LeadsController(_leadService.Object,
                _autoMapper,
                _logger.Object,
                _crmProducers.Object,
                _requestHelper.Object,
                _validatorLeadInsertRequest,
                _validatorLeadUpdateRequest,
                _validatorLeadChangePasswordRequest);
        }

        private void AddContext(string token)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
        }

        [Test]
        public void ChangePassword_ModelIsEmpty_ShouldThrowBadRequestException()
        {
            // given
            LeadChangePasswordRequest model = null; // Invalid model
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Vip" });
            AddContext(token);
            var expected = "You must specify the table details in the request body";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await _controller.ChangePassword(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void ChangePassword_NewPasswordIsEmpty_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = LeadControllerTestData.GetChangePasswordModel();
            model.NewPassword = "";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- NewPassword: New Password is empty Severity: Error\r\n -- NewPassword: Minimum lenght of Password 8 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.ChangePassword(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void ChangePassword_NewPasswordIsTooShort_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = LeadControllerTestData.GetChangePasswordModel();
            model.NewPassword = "123";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- NewPassword: Minimum lenght of Password 8 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.ChangePassword(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void ChangePassword_NewPasswordIsTooLong_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var model = LeadControllerTestData.GetChangePasswordModel();
            model.NewPassword = "123456789012345678901234567890123";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- NewPassword: Maximum lenght of Password 30 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.ChangePassword(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

    }
}
