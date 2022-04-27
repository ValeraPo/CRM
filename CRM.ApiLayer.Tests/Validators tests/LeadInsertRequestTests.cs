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
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CRM.ApiLayer.Tests
{
    public class LeadInsertRequestTests
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


        public LeadInsertRequestTests()
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


        [Test]
        public void AddLead_ModelIsEmpty_ShouldThrowBadRequestException()
        {
            // given
            LeadInsertRequest model = null; // Invalid model
            var expected = "You must specify the table details in the request body";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_PasswordIsEmpty_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.Password = "";
            var expected = "Validation failed: \r\n -- Password: Password is empty Severity: Error\r\n -- Password: Minimum lenght of Password 8 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_PasswordIsTooShort_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.Password = "123";
            var expected = "Validation failed: \r\n -- Password: Minimum lenght of Password 8 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_PasswordIsTooLong_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.Password = "123456789012345678901234567890123";
            var expected = "Validation failed: \r\n -- Password: Maximum lenght of Password 30 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_EmailIsEmpty_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.Email = "";
            var expected = "Validation failed: \r\n -- Email: Email is empty Severity: Error\r\n -- Email: Email address is not valid Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_EmailIsNotValid_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.Email = "popopo";
            var expected = "Validation failed: \r\n -- Email: Email address is not valid Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_NameIsEmpty_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.Name = "";
            var expected = "Validation failed: \r\n -- Name: Name is empty Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_NameIsTooLong_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.Name = "123456789012345678901234567890123";
            var expected = "Validation failed: \r\n -- Name: Maximum lenght of Name 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_LastNameIsEmpty_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.LastName = "";
            var expected = "Validation failed: \r\n -- LastName: Last Name is empty Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_LastNameIsTooLong_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.LastName = "123456789012345678901234567890123";
            var expected = "Validation failed: \r\n -- LastName: Maximum lenght of Last Name 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_PhoneIsNotValid_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.Phone = "ghkjhjkh";
            var expected = "Validation failed: \r\n -- Phone: PhoneNumber not valid Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddLead_CityIsTooLong_ShouldThrowValidationException()
        {
            // given
            var model = LeadControllerTestData.GetInsertModel();
            model.City = "123456789012345678901234567890123";
            var expected = "Validation failed: \r\n -- City: Maximum lenght of City 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await _controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }
    }
}
