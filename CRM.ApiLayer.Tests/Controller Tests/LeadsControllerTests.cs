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
        private LeadsController _sut;


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
            _sut = new LeadsController(_leadService.Object,
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
            _sut.ControllerContext.HttpContext = context;
        }

        #region AddLeadTests
        [Test]
        public async Task AddLeadTest_ShouldAddLead()
        {
            //given
            var leadRequest = LeadControllerTestData.GetInsertModel();

            //when
            await _sut.AddLead(leadRequest);

            //then
            _leadService.Verify(m => m.AddLead(It.IsAny<LeadModel>()), Times.Once());
            _crmProducers.Verify(m => m.NotifyLeadAdded(It.IsAny<LeadModel>()), Times.Once());
            _crmProducers.Verify(m => m.NotifyAccountAdded(It.IsAny<int>()), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void AddLeadTest_EmailExists_ShouldThrowDuplicationException()
        {
            //given
            var expected = $"Try to singup. Email e***l is already exists.";
            var leadModel = LeadControllerTestData.GetInsertModel();
            _leadService
                .Setup(m => m.AddLead(It.IsAny<LeadModel>()))
                .Callback(() => throw new DuplicationException(expected));

            //when
            var actual = Assert
                .ThrowsAsync<DuplicationException>(async () => await _sut.AddLead(leadModel))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.AddLead(It.IsAny<LeadModel>()), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Received a request to create a new lead.");
        }
        #endregion

        #region UpdateLeadTests
        [Test]
        public async Task UpdateLeadTest_ShouldUpdateLead()
        {
            //given
            var token = "token";
            var leadRequest = LeadControllerTestData.GetUpdateModel();
            var leadId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.UpdateLead(leadId, leadRequest);

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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.UpdateLead(It.IsAny<int>(), It.IsAny<LeadUpdateRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateLead_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var leadRequest = LeadControllerTestData.GetUpdateModel();
            var leadId = 42;
            var expected = $"Lead entiy with ID = {leadId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            _leadService
                .Setup(m => m.UpdateLead(leadId, It.IsAny<LeadModel>()))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.UpdateLead(leadId, leadRequest))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.UpdateLead(leadId, It.IsAny<LeadModel>()), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Received a request to update lead with ID = 42.");
        }
        #endregion

        #region ChangeRoleLeadTests
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
            await _sut.ChangeRoleLead(leadId, role);

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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.ChangeRoleLead(It.IsAny<int>(), It.IsAny<Role>()))!
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.ChangeRoleLead(It.IsAny<int>(), It.IsAny<Role>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void ChangeRoleLead_RoleIsAdmin_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var role = Role.Admin;
            var expected = $"Authorisation error. The role can be changed to Regular or VIP.";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            _leadService
                .Setup(m => m.ChangeRoleLead(leadId, role))
                .Callback(() => throw new IncorrectRoleException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<IncorrectRoleException>(async () => await _sut.ChangeRoleLead(leadId, role))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.ChangeRoleLead(leadId, role), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to update the role of the lead with ID = {leadId}.");
        }

        [Test]
        public void ChangeRoleLead_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var role = Role.Regular;
            var expected = $"Lead entiy with ID = {leadId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            _leadService
                .Setup(m => m.ChangeRoleLead(leadId, role))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.ChangeRoleLead(leadId, role))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.ChangeRoleLead(leadId, role), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to update the role of the lead with ID = {leadId}.");
        }
        #endregion

        #region DeleteByIdTests
        [Test]
        public async Task DeleteByIdTest_ShouldDeleteLead()
        {
            //given
            var token = "token";
            var leadId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            AddContext(token);

            //when
            await _sut.DeleteById(leadId);

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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.DeleteById(It.IsAny<int>()))!
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.DeleteById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void DeleteById_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var expected = $"Lead entiy with ID = {leadId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            _leadService
                .Setup(m => m.DeleteById(leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.DeleteById(leadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.DeleteById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Received a request to delete lead with ID = 42.");
        }

        [Test]
        public void DeleteById_LeadIsBanned_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var expected = $"Lead witd ID {leadId} is already banned.";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            _leadService
                .Setup(m => m.DeleteById(leadId))
                .Callback(() => throw new BannedException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<BannedException>(async () => await _sut.DeleteById(leadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.DeleteById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Received a request to delete lead with ID = 42.");
        }
        #endregion

        #region RestoreByIdTests
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
            await _sut.RestoreById(leadId);

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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.RestoreById(It.IsAny<int>()))!
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.RestoreById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void RestoreById_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var expected = $"Lead entiy with ID = {leadId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            _leadService
                .Setup(m => m.RestoreById(leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.RestoreById(leadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.RestoreById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Received a request to restore lead with ID = 42.");
        }

        [Test]
        public void RestoreById_LeadIsNotBanned_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var expected = $"Lead witd ID {leadId} is already banned.";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            _leadService
                .Setup(m => m.RestoreById(leadId))
                .Callback(() => throw new BannedException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<BannedException>(async () => await _sut.RestoreById(leadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.RestoreById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Received a request to restore lead with ID = 42.");
        }
        #endregion

        #region GetAllTests
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
            await _sut.GetAll();

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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetAll())!
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetAll())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }
        #endregion

        #region GetAllToAuthTests
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
            await _sut.GetAllToAuth();

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
            await _sut.GetAllToAuth();

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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetAllToAuth())!
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetAllToAuth())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }
        #endregion

        #region GetByIdTests
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
            await _sut.GetById(leadId);

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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetById_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var expected = $"Lead entiy with ID = {leadId} not found";
            var identity = new IdentityResponseModel { Id = 1, Role = "Admin" };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(identity);
            _leadService
                .Setup(m => m.GetById(leadId, identity))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.GetById(leadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.GetById(leadId, identity), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received to get an lead with an ID {leadId}.");
        }

        [Test]
        public void GetById_LeadIsNotAdmin_ShouldThrowAuthorizationException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var expected = $"Authorization error. Lead with ID 1 doesn't have acces.";
            var identity = new IdentityResponseModel { Id = 1, Role = "Regular" };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(identity);
            _leadService
                .Setup(m => m.GetById(leadId, identity))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.GetById(leadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadService.Verify(m => m.GetById(leadId, identity), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received to get an lead with an ID 42.");
        }
        #endregion

        #region ChangePasswordTests
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
            await _sut.ChangePassword(leadRequest);

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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.ChangePassword(It.IsAny<LeadChangePasswordRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }
        #endregion
    }
}