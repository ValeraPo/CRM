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
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
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
        private LeadsController _controller;


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
        public async Task AddLeadTest_ShouldAddLead()
        {
            //given
            var leadRequest = LeadControllerTestData.GetInsertModel();
            var accountModel = _autoMapper.Map<LeadModel>(leadRequest);

            //when
            await _controller.AddLead(leadRequest);

            //then
            _leadService.Verify(m => m.AddLead(It.IsAny<LeadModel>()), Times.Once());
            _crmProducers.Verify(m => m.NotifyLeadAdded(It.IsAny<LeadModel>()), Times.Once());
            _crmProducers.Verify(m => m.NotifyAccountAdded(It.IsAny<int>()), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
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
            AddContext(token);

            //when
            await _controller.UpdateLead(leadId, leadRequest);

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
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.UpdateLead(It.IsAny<int>(), It.IsAny<LeadUpdateRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
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
            AddContext(token);

            //when
            await _controller.ChangeRoleLead(leadId, role);

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
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.ChangeRoleLead(It.IsAny<int>(), It.IsAny<Role>()))!
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
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.ChangeRoleLead(It.IsAny<int>(), It.IsAny<Role>()))!
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
            AddContext(token);

            //when
            await _controller.DeleteById(leadId);

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
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.DeleteById(It.IsAny<int>()))!
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
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.DeleteById(It.IsAny<int>()))!
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
            AddContext(token);

            //when
            await _controller.RestoreById(leadId);

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
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.RestoreById(It.IsAny<int>()))!
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
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.RestoreById(It.IsAny<int>()))!
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
            AddContext(token);

            //when
            await _controller.GetAll();

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
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.GetAll())!
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
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.GetAll())!
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
            AddContext(token);

            //when
            await _controller.GetAllToAuth();

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
            AddContext(token);

            //when
            await _controller.GetAllToAuth();

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
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.GetAllToAuth())!
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
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.GetAllToAuth())!
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
            AddContext(token);

            //when
            await _controller.GetById(leadId);

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
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.GetById(It.IsAny<int>()))!
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
            AddContext(token);

            //when
            await _controller.ChangePassword(leadRequest);

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
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _controller.ChangePassword(It.IsAny<LeadChangePasswordRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }
    }
}