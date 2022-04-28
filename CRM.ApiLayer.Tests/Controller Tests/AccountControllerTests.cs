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
    public class AccountControllerTests
    {
        private Mock<IAccountService> _accountService;
        private readonly IMapper _autoMapper;
        private Mock<ILogger<AccountsController>> _logger;
        private Mock<ICRMProducers> _crmProducers;
        private Mock<IRequestHelper> _requestHelper;
        private readonly IValidator<AccountInsertRequest> _validatorAccountInsertRequest;
        private readonly IValidator<AccountUpdateRequest> _validatorAccountUpdateRequest;
        private AccountsController _sut;


        public AccountControllerTests()
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
            _sut = new AccountsController(_accountService.Object,
                _autoMapper,
                _logger.Object,
                _crmProducers.Object,
                _requestHelper.Object,
                _validatorAccountInsertRequest,
                _validatorAccountUpdateRequest);
        }

        private void AddContext(string token)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _sut.ControllerContext.HttpContext = context;
        }

        #region AddAccountTests
        [Test]
        public async Task AddAccountTest_ShouldAddAccount()
        {
            //given
            var token = "token";
            var accountRequest = new AccountInsertRequest { Name = "Money", CurrencyType = 85 };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync( new IdentityResponseModel { Id = 1, Role = "Regular" } );
            AddContext(token);

            //when
            await _sut.AddAccount(accountRequest);

            //then
            _accountService.Verify(m => m.AddAccount(Role.Regular, It.IsAny<AccountModel>()), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyAccountAdded(It.IsAny<AccountModel>()), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void AddAccount_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            string token = default;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.AddAccount(It.IsAny<AccountInsertRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddAccount_AccountHasDuplication_ShouldThrowException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountRequest = new AccountInsertRequest { Name = "Money", CurrencyType = 85 };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            _accountService
                .Setup(m => m.AddAccount(Role.Regular, It.IsAny<AccountModel>()))
                .Callback(() => throw new DuplicationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<DuplicationException>(async () => await _sut.AddAccount(accountRequest))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.AddAccount(Role.Regular, It.IsAny<AccountModel>()), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }

        [Test]
        public void AddAccount_RoleIsRegularAndCurrensyIsNotUsd_ShouldThrowException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountRequest = new AccountInsertRequest { Name = "Money", CurrencyType = 103 };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            _accountService
                .Setup(m => m.AddAccount(Role.Regular, It.IsAny<AccountModel>()))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.AddAccount(accountRequest))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.AddAccount(Role.Regular, It.IsAny<AccountModel>()), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }
        #endregion

        #region UpdateAccountTests
        [Test]
        public async Task UpdateAccountTest_ShouldUpdateAccount()
        {
            //given
            var token = "token";
            var accountId = 42;
            var accountRequest = new AccountUpdateRequest { Name = "Money" };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.UpdateAccount(accountId, accountRequest);

            //then
            _accountService.Verify(m => m.UpdateAccount(It.IsAny<int>(), It.IsAny<AccountModel>()), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyAccountAdded(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void UpdateAccount_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.UpdateAccount(It.IsAny<int>(), It.IsAny<AccountUpdateRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateAccount_InvalidToken_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel());
            AddContext(token);
            var expected = "Invalid token";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.UpdateAccount(It.IsAny<int>(), It.IsAny<AccountUpdateRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateAccount_RoleIsAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" }) ;
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.UpdateAccount(It.IsAny<int>(), It.IsAny<AccountUpdateRequest>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UpdateAccount_AccountNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountId = 42;
            var accountRequest = new AccountUpdateRequest { Name = "Money" };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            _accountService
                .Setup(m => m.UpdateAccount(It.IsAny<int>(), It.IsAny<AccountModel>()))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAccount(accountId, accountRequest))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.UpdateAccount(It.IsAny<int>(), It.IsAny<AccountModel>()), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }

        [Test]
        public void UpdateAccount_LeadDontHaveAccesToAccount_ShouldThrowAuthorizationException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountId = 42;
            var accountRequest = new AccountUpdateRequest { Name = "Money" };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Regular" });
            _accountService
                .Setup(m => m.UpdateAccount(It.IsAny<int>(), It.IsAny<AccountModel>()))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.UpdateAccount(accountId, accountRequest))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.UpdateAccount(It.IsAny<int>(), It.IsAny<AccountModel>()), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }
        #endregion

        #region LockByIdTests
        [Test]
        public async Task LockByIdTest_ShouldLockAccount()
        {
            //given
            var token = "token";
            var accountId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" }) ;
            AddContext(token);

            //when
            await _sut.LockById(accountId);

            //then
            _accountService.Verify(m => m.LockById(accountId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyAccountAdded(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void LockById_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.LockById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void LockById_RoleIsNotAdmin_ShouldThrowForbiddenException()
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.LockById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void LockById_AccountNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            AddContext(token);
            _accountService
                .Setup(m => m.LockById(accountId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.LockById(accountId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.LockById(accountId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }

        [Test]
        public void LockById_CurrencyIsRub_ShouldThrowBadRequestException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            AddContext(token);
            _accountService
                .Setup(m => m.LockById(accountId))
                .Callback(() => throw new BadRequestException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await _sut.LockById(accountId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.LockById(accountId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }
        #endregion

        #region UnlockByIdTests
        [Test]
        public async Task UnlockByIdTest_ShouldUnlockAccount()
        {
            //given
            var token = "token";
            var accountId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            AddContext(token);

            //when
            await _sut.UnlockById(accountId);

            //then
            _accountService.Verify(m => m.UnlockById(accountId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyAccountAdded(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void UnlockById_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.UnlockById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UnockById_RoleIsNotAdmin_ShouldThrowForbiddenException()
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.UnlockById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void UnlockById_AccountNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            AddContext(token);
            _accountService
                .Setup(m => m.UnlockById(accountId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.UnlockById(accountId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.UnlockById(accountId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }
        #endregion

        #region GetByLeadTests
        [Test]
        public async Task GetByLeadTest_ShouldGetListOfAccounts()
        {
            //given
            var token = "token";
            var leadId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.GetByLead();

            //then
            _accountService.Verify(m => m.GetByLead(leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void GetByLead_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetByLead())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetByLead_RoleIsAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetByLead())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetByLead_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var leadId = 42;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            _accountService
                .Setup(m => m.GetByLead(leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.GetByLead())!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.GetByLead(leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }
        #endregion

        #region GetByIdTests
        [Test]
        public async Task GetByIdTest_ShouldGetfAccounts()
        {
            //given
            var token = "token";
            var accountId = 42;
            var leadId = 1;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.GetById(accountId);

            //then
            _accountService.Verify(m => m.GetById(accountId, leadId), Times.Once());
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
        public void GetById_RoleIsAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetById(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetById_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountId = 42;
            var leadId = 1;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            _accountService
                .Setup(m => m.GetById(accountId, leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.GetById(accountId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.GetById(accountId, leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }

        [Test]
        public void GetById_LeadHasNoAccess_ShouldThrowAuthorizationException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var accountId = 42;
            var leadId = 1;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);
            _accountService
                .Setup(m => m.GetById(accountId, leadId))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.GetById(accountId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.GetById(accountId, leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }
        #endregion

        #region GetBalanceTests
        [Test]
        public async Task GetBalanceTest_ShouldGetfAccounts()
        {
            //given
            var token = "token";
            var leadId = 1;
            var currency = Currency.RUB;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.GetBalance(currency);

            //then
            _accountService.Verify(m => m.GetBalance(leadId, currency), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void GetBalance_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetBalance(It.IsAny<Currency>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetBalance_RoleIsAdmin_ShouldThrowForbiddenException()
        {
            // given
            var token = "token";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = 1, Role = "Admin" });
            AddContext(token);
            var expected = $"Lead id = 1 doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetBalance(It.IsAny<Currency>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetBalance_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var leadId = 1;
            var currency = Currency.RUB;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _accountService
                .Setup(m => m.GetBalance(leadId, currency))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.GetBalance(currency))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.GetBalance(leadId, currency), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }

        [Test]
        public void GetBalance_CurrencyIsNotAmongAccounts_ShouldThrowBadRequestException()
        {
            //given
            var token = "token";
            var expected = "Test message";
            var leadId = 1;
            var currency = Currency.THB;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _accountService
                .Setup(m => m.GetBalance(leadId, currency))
                .Callback(() => throw new BadRequestException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await _sut.GetBalance(currency))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountService.Verify(m => m.GetBalance(leadId, currency), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
        }
        #endregion
    }
}