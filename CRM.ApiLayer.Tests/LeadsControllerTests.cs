using AutoMapper;
using CRM.APILayer.Configuration;
using CRM.APILayer.Controllers;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.APILayer.Validation;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using FluentValidation;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CRM.ApiLayer.Tests
{
    public class LeadsControllerTests
    {
        private Mock<ILeadService> _leadService;
        private readonly IMapper _autoMapper;
        private Mock<ILogger<LeadsController>> _logger;
        private Mock<ICRMProducers> _crmProducers;
        private Mock<IRequestHelper> _requestHelper;
        private readonly IValidator<LeadInsertRequest> _validatorLeadInsertRequest;
        private readonly IValidator<LeadUpdateRequest> _validatorLeadUpdateRequest;
        private readonly IValidator<LeadChangePasswordRequest> _validatorLeadChangePasswordRequest;
        private LeadsController controller;


        public LeadsControllerTests()
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
            controller = new LeadsController(_leadService.Object,
                _autoMapper,
                _logger.Object,
                _crmProducers.Object,
                _requestHelper.Object,
                _validatorLeadInsertRequest,
                _validatorLeadUpdateRequest,
                _validatorLeadChangePasswordRequest);
        }

        [Test]
        public async Task AddLeadTest_ShouldAddLead()
        {
            //given
            var leadRequest = LeadControllerTestData.GetInsertModel();
            var accountModel = _autoMapper.Map<LeadModel>(leadRequest);

            //when
            await controller.AddLead(leadRequest);

            //then
            _leadService.Verify(m => m.AddLead(It.IsAny<LeadModel>()), Times.Once());
            _crmProducers.Verify(m => m.NotifyLeadAdded(It.IsAny<LeadModel>()), Times.Once());
            _crmProducers.Verify(m => m.NotifyAccountAdded(It.IsAny<int>()), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void AddLead_ModelIsEmpty_ShouldThrowBadRequestException()
        {
            // given
            LeadInsertRequest model = null; // Invalid model
            var expected = "You must specify the table details in the request body";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
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
                .ThrowsAsync<ValidationException>(async () => await controller.AddLead(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task UpdateLeadTest_ShouldUpdateLead()
        {
            //given
            var token = "token";
            var leadRequest = LeadControllerTestData.GetUpdateModel();
            var accountModel = _autoMapper.Map<LeadModel>(leadRequest);
            var leadId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.UpdateLead(leadId, leadRequest);

            //then
            _leadService.Verify(m => m.UpdateLead(leadId, It.IsAny<LeadModel>()), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyLeadAdded(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void UpdateLead_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.UpdateLead(It.IsAny<int>(), It.IsAny<LeadUpdateRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "You must specify the table details in the request body";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await controller.UpdateLead(It.IsAny<int>(), model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- Name: Name is empty Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.UpdateLead(leadId, model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- Name: Maximum lenght of Name 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.UpdateLead(leadId, model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- LastName: Last Name is empty Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.UpdateLead(leadId, model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- LastName: Maximum lenght of Last Name 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.UpdateLead(leadId, model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- Phone: PhoneNumber not valid Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.UpdateLead(leadId, model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- City: Maximum lenght of City 20 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.UpdateLead(leadId, model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task ChangeRoleLeadTest_ShouldChangeRoleLead()
        {
            //given
            var token = "token";
            var leadId = 42;
            var role = Role.Regular;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.ChangeRoleLead(leadId, role);

            //then
            _leadService.Verify(m => m.ChangeRoleLead(leadId, role), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyLeadAdded(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void ChangeRoleLead_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.ChangeRoleLead(It.IsAny<int>(), It.IsAny<Role>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void ChangeRoleLead_RoleIsNotAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.ChangeRoleLead(It.IsAny<int>(), It.IsAny<Role>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task DeleteByIdTest_ShouldDeleteLead()
        {
            //given
            var token = "token";
            var leadId = 42;
            var role = Role.Regular;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.DeleteById(leadId);

            //then
            _leadService.Verify(m => m.DeleteById(leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyLeadAdded(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void DeleteById_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.DeleteById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void DeleteById_RoleIsNotAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.DeleteById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task RestoreById_ShouldRestoreLead()
        {
            //given
            var token = "token";
            var leadId = 42;
            var role = Role.Regular;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.RestoreById(leadId);

            //then
            _leadService.Verify(m => m.RestoreById(leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyLeadAdded(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void RestoreById_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.RestoreById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void RestoreById_RoleIsNotAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.RestoreById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllLead()
        {
            //given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.GetAll();

            //then
            _leadService.Verify(m => m.GetAll(), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void GetAll_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.GetAll())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetAll_RoleIsNotAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.GetAll())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task GetAllToAuth_TokenFromAuthService_ShouldReturnAllLead()
        {
            //given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { IssuerMicroservice = Microservice.MarvelousAuth.ToString() });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.GetAllToAuth();

            //then
            _leadService.Verify(m => m.GetAllToAuth(), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public async Task GetAllToAuth_TokenFromLead_ShouldReturnAllLead()
        {
            //given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.GetAllToAuth();

            //then
            _leadService.Verify(m => m.GetAllToAuth(), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void GetAllToAuth_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.GetAllToAuth())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetAllToAuth_RoleIsNotAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.GetAllToAuth())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task GetById_ShouldReturnLead()
        {
            //given
            var token = "token";
            var leadId = 42;
            var leadIdentity = new IdentityResponseModel { Id = leadId, Role = "Vip" };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(leadIdentity);
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.GetById(leadId);

            //then
            _leadService.Verify(m => m.GetById(leadId, leadIdentity), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void GetById_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.GetById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task ChangePasswordTest_ShouldChangePassword()
        {
            //given
            var leadRequest = LeadControllerTestData.GetChangePasswordModel();
            var token = "token";
            var leadId = 42;
            var leadIdentity = new IdentityResponseModel { Id = leadId, Role = "Vip" };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(leadIdentity);
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.ChangePassword(leadRequest);

            //then
            _leadService.Verify(m => m.ChangePassword(leadId, leadRequest.OldPassword, leadRequest.NewPassword), Times.Once());
            _crmProducers.Verify(m => m.NotifyLeadAdded(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void ChangePassword_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await controller.ChangePassword(It.IsAny<LeadChangePasswordRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "You must specify the table details in the request body";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await controller.ChangePassword(model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- NewPassword: New Password is empty Severity: Error\r\n -- NewPassword: Minimum lenght of Password 8 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.ChangePassword(model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- NewPassword: Minimum lenght of Password 8 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.ChangePassword(model))!
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
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;
            var expected = "Validation failed: \r\n -- NewPassword: Maximum lenght of Password 30 symbols Severity: Error";

            //when
            var actual = Assert
                .ThrowsAsync<ValidationException>(async () => await controller.ChangePassword(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }
    }
}