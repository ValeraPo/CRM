using AutoMapper;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.ResponseModels;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Tests.ServiceTests
{
    public class LeadServiceTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ILeadRepository> _leadRepositoryMock;
        private readonly IMapper _autoMapper;
        private Mock<ILogger<LeadService>> _logger;
        private Mock<IRequestHelper> _requestHelper;
        private LeadService sut;

        public LeadServiceTests()
        {
            _autoMapper = new Mapper(
                new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));

        }

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _leadRepositoryMock = new Mock<ILeadRepository>();
            _requestHelper = new Mock<IRequestHelper>();
            _logger = new Mock<ILogger<LeadService>>();
            sut = new LeadService(_autoMapper,
               _leadRepositoryMock.Object,
               _accountRepositoryMock.Object,
               _logger.Object,
               _requestHelper.Object);

        }

        #region AddLeadTests
        [Test]
        public async Task AddLeadTest_ShouldAddLead()
        {
            //given
            var email = "email";
            var password = "password";
            var leadModel = new LeadModel { Email = email, Password = password};
            _leadRepositoryMock
                .Setup(m => m.GetByEmail(email))
                .ReturnsAsync((Lead)null!);
            _requestHelper
                .Setup(m => m.HashPassword(password))
                .ReturnsAsync("hash");

            //when
            await sut.AddLead(leadModel);

            //then
            _leadRepositoryMock.Verify(m => m.AddLead(It.IsAny<Lead>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.AddAccount(It.IsAny<Account>()), Times.Once());
            _requestHelper.Verify(m => m.HashPassword(password), Times.Once());
            _leadRepositoryMock.Verify(m => m.GetByEmail(email), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Received a request to create a new lead.");
        }

        [Test]
        public void AddLeadTest_EmailExists_ShouldThrowDuplicationException()
        {
            //given
            var email = "email";
            var leadModel = new LeadModel { Email = email };
            _leadRepositoryMock
                .Setup(m => m.GetByEmail(email))
                .ReturnsAsync(new Lead());
            var expected = $"Try to singup. Email e***l is already exists.";

            //when
            var actual = Assert
                .ThrowsAsync<DuplicationException>(async () => await sut.AddLead(leadModel))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _leadRepositoryMock.Verify(m => m.GetByEmail(email), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Received a request to create a new lead.");

        }
        #endregion

        #region UpdateLeadTests
        [Test]
        public async Task UpdateLeadTest_ShouldUpdateLead()
        {
            //given
            var leadId = 42;
            var leadModel = new LeadModel { Id = leadId };
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead());

            //when
            await sut.UpdateLead(leadId, leadModel);

            //then
            _leadRepositoryMock.Verify(m => m.UpdateLeadById(It.IsAny<Lead>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to update lead with ID = {leadId}.");
        }

        [Test]
        public void UpdateLead_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync((Lead)null!);
            var expected = $"Lead entiy with ID = {leadId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.UpdateLead(leadId, new LeadModel { Id = leadId }))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ChangeRoleLeadTests
        [Test]
        public async Task ChangeRoleLeadTest_ShouldChangeRoleLead()
        {
            //given
            var leadId = 42;
            var role = Role.Regular;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead());

            //when
            await sut.ChangeRoleLead(leadId, role);

            //then
            _leadRepositoryMock.Verify(m => m.ChangeRoleLead(It.IsAny<Lead>()), Times.Once());
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to update the role of the lead with ID = {leadId}.");
        }

        [Test]
        public void ChangeRoleLead_RoleIsAdmin_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            var role = Role.Admin;
            var expected = $"Authorisation error. The role can be changed to Regular or VIP.";

            //when
            var actual = Assert
                .ThrowsAsync<IncorrectRoleException>(async () => await sut.ChangeRoleLead(leadId, role))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);

        }

        [Test]
        public void ChangeRoleLead_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            var role = Role.Vip;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync((Lead)null!);
            var expected = $"Lead entiy with ID = {leadId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.ChangeRoleLead(leadId, role))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to update the role of the lead with ID = {leadId}.");
        }
        #endregion

        #region DeleteByIdTests
        [Test]
        public async Task DeleteByIdTest_ShouldDeleteLead()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead { IsBanned = false});

            //when
            await sut.DeleteById(leadId);

            //then
            _leadRepositoryMock.Verify(m => m.DeleteById(leadId), Times.Once());
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to delete lead with ID =  {leadId}.");
        }

        [Test]
        public void DeleteById_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync((Lead)null!);
            var expected = $"Lead entiy with ID = {leadId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.DeleteById(leadId))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to delete lead with ID =  {leadId}.");
        }

        [Test]
        public void DeleteById_LeadIsBanned_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead { IsBanned = true, Id = leadId });
            var expected = $"Lead witd ID {leadId} is already banned.";

            //when
            var actual = Assert
                .ThrowsAsync<BannedException>(async () => await sut.DeleteById(leadId))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to delete lead with ID =  {leadId}.");
        }
        #endregion

        #region RestoreByIdTests
        [Test]
        public async Task RestoreByIdTest_ShouldRestoreLead()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead { IsBanned = true });

            //when
            await sut.RestoreById(leadId);

            //then
            _leadRepositoryMock.Verify(m => m.RestoreById(leadId), Times.Once());
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to restore lead with ID =  {leadId}.");
        }

        [Test]
        public void RestoreById_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync((Lead)null!);
            var expected = $"Lead entiy with ID = {leadId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.RestoreById(leadId))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to restore lead with ID =  {leadId}.");
        }

        [Test]
        public void RestoreById_LeadIsNotBanned_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead { IsBanned = false, Id = leadId });
            var expected = $"Lead with ID {leadId} is not banned.";

            //when
            var actual = Assert
                .ThrowsAsync<BannedException>(async () => await sut.RestoreById(leadId))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to restore lead with ID =  {leadId}.");
        }
        #endregion

        #region GetAllTests
        [Test]
        public async Task GetAllTest_ShouldReturnAllLeads()
        {
            //when
            await sut.GetAll();

            //then
            _leadRepositoryMock.Verify(m => m.GetAll(), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to receive all leads.");
        }

        [Test]
        public async Task GetAllToAuth_ShouldReturnAllRealLeads()
        {
            //when
            await sut.GetAllToAuth();

            //then
            _leadRepositoryMock.Verify(m => m.GetAllToAuth(), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to receive all leads for Auth.");
        }
        #endregion

        #region GetByIdTests_WithIdentityModel
        [Test]
        public async Task GetByIdTest_WithIdentityModelNotAdmin_ShouldReturnLead()
        {
            //given
            var leadId = 42;
            var identity = new IdentityResponseModel { Id = leadId, Role = Role.Regular.ToString() };
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead());

            //when
            await sut.GetById(leadId, identity);

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received to get an lead with an ID {leadId}.");
        }

        [Test]
        public async Task GetByIdTest_WithIdentityModelAdmin_ShouldReturnLead()
        {
            //given
            var leadId = 42;
            var identity = new IdentityResponseModel { Id = 1, Role = Role.Admin.ToString() };
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead());

            //when
            await sut.GetById(leadId, identity);

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received to get an lead with an ID {leadId}.");
        }

        [Test]
        public void GetById_LeadIsNotAdmin_ShouldThrowAuthorizationException()
        {
            //given
            var leadId = 42;
            var identity = new IdentityResponseModel { Id = 100, Role = Role.Regular.ToString() };
            var expected = $"Authorization error. Lead with ID 100 doesn't have acces.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.GetById(leadId, identity))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received to get an lead with an ID {leadId}.");
        }

        [Test]
        public void GetById_WhithIdentity_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            var identity = new IdentityResponseModel { Id = leadId, Role = Role.Regular.ToString() };
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync((Lead)null!);
            var expected = $"Lead entiy with ID = {leadId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.GetById(leadId, identity))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received to get an lead with an ID {leadId}.");
        }
        #endregion

        #region GetByIdTests
        [Test]
        public async Task GetByIdTest_ShouldReturnLead()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead());

            //when
            await sut.GetById(leadId);

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
        }

        [Test]
        public void GetById_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync((Lead)null!);
            var expected = $"Lead entiy with ID = {leadId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.GetById(leadId))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once());
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ChangePasswordTests
        [Test]
        public async Task ChangePasswordTest_ShouldChangePassword()
        {
            //given
            var leadId = 42;
            var oldPass = "old";
            var newPass = "new";
            var hash = "hash";
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead { Id = leadId });
            _requestHelper
                .Setup(m => m.GetToken(It.IsAny<AuthRequestModel>()))
                .ReturnsAsync("token");
            _requestHelper
                .Setup(m => m.HashPassword(newPass))
                .ReturnsAsync(hash);

            //when
            await sut.ChangePassword(leadId, oldPass, newPass);

            //then
            _requestHelper.Verify(m => m.GetToken(It.IsAny<AuthRequestModel>()), Times.Once());
            _requestHelper.Verify(m => m.HashPassword(newPass), Times.Once());
            _leadRepositoryMock.Verify(m => m.ChangePassword(leadId, hash), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to change the password of a lead with an ID = {leadId}.");
        }
        #endregion
    }
}
