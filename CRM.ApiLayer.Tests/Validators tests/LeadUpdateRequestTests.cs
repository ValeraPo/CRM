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
    public class LeadUpdateRequestTests
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


        public LeadUpdateRequestTests()
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
        public void UpdateLead_ModelIsEmpty_ShouldThrowBadRequestException()
        {
            // given
            var token = "token";
            LeadUpdateRequest model = null; // Invalid model
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            AddContext(token);
            var expected = "You must specify the table details in the request body";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await _controller.UpdateLead(It.IsAny<int>(), model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateLead_NameIsEmpty_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var leadId = 42;
            var model = LeadControllerTestData.GetUpdateModel();
            model.Name = "";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- Name: Name is empty Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.UpdateLead(leadId, model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateLead_NameIsTooLong_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var leadId = 42;
            var model = LeadControllerTestData.GetUpdateModel();
            model.Name = "123456789012345678901234567890123";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- Name: Maximum lenght of Name 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.UpdateLead(leadId, model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateLead_LastNameIsEmpty_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var leadId = 42;
            var model = LeadControllerTestData.GetUpdateModel();
            model.LastName = "";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- LastName: Last Name is empty Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.UpdateLead(leadId, model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateLead_LastNameIsTooLong_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var leadId = 42;
            var model = LeadControllerTestData.GetUpdateModel();
            model.LastName = "123456789012345678901234567890123";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- LastName: Maximum lenght of Last Name 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.UpdateLead(leadId, model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateLead_PhoneIsNotValid_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var leadId = 42;
            var model = LeadControllerTestData.GetUpdateModel();
            model.Phone = "ghkjhjkh";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- Phone: PhoneNumber not valid Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.UpdateLead(leadId, model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateLead_CityIsTooLong_ShouldThrowValidationException()
        {
            // given
            var token = "token";
            var leadId = 42;
            var model = LeadControllerTestData.GetUpdateModel();
            model.City = "123456789012345678901234567890123";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            var expected = "Validation failed: \r\n -- City: Maximum lenght of City 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.UpdateLead(leadId, model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

    }
}
