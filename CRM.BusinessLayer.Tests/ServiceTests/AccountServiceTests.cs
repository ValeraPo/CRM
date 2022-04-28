using AutoMapper;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Tests.ServiceTests
{
    public class AccountServiceTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ILeadRepository> _leadRepositoryMock;
        private Mock<ILogger<AccountService>> _logger;
        private readonly IMapper _autoMapper;
        private readonly Mock<IRequestHelper> _requestHelper;
        private AccountService sut;

        public AccountServiceTests()
        {
            _autoMapper = new Mapper(
                new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));
            _requestHelper = new Mock<IRequestHelper>();

        }

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _leadRepositoryMock = new Mock<ILeadRepository>();
            _logger = new Mock<ILogger<AccountService>>();
            sut =  new AccountService(_autoMapper,
               _accountRepositoryMock.Object,
               _leadRepositoryMock.Object,
               _logger.Object,
               _requestHelper.Object);

        }

        #region AddAccountTests
        [Test]
        public async Task AddAccountTest_ShouldAddedAccount()
        {
            //given
            var leadId = 42;
            var accountModel = new AccountModel { CurrencyType = Currency.USD, Lead = new LeadModel() };
            accountModel.Lead.Id = leadId;
            var accounts = new List<Account> { new Account { CurrencyType = Currency.RUB } };
            _accountRepositoryMock.Setup(m => m.GetByLead(leadId)).ReturnsAsync(accounts);
           

            //when
            await sut.AddAccount(Role.Vip, accountModel);

            //then
            _accountRepositoryMock.Verify(m => m.AddAccount(It.IsAny<Account>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.GetByLead(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Request was received to add an account.");
        }

        [TestCase(Currency.RUB)]
        [TestCase(Currency.USD)]
        public void AddAccount_AccountHasDuplication_ShouldThrowException(Currency currency)
        {
            //given
            var leadId = 42;
            var accountModel = new AccountModel { CurrencyType = currency, Lead = new LeadModel() };
            accountModel.Lead.Id = leadId;
            var accounts = new List<Account> { new Account { CurrencyType = currency } };
            _accountRepositoryMock.Setup(m => m.GetByLead(leadId)).ReturnsAsync(accounts);
            var expected = "Error: an account with this currency already exists.";

            //when
            var actual = Assert
                .ThrowsAsync<DuplicationException>(async () => await sut.AddAccount(It.IsAny<Role>(), accountModel))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepositoryMock.Verify(m => m.GetByLead(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, "Error: an account with this currency already exists.");
        }

        [Test]
        public void AddAccount_RoleIsRegularAndCurrensyIsNotUsd_ShouldThrowException()
        {
            //given
            var leadId = 42;
            var accountModel = new AccountModel { CurrencyType = Currency.RUB, Lead = new LeadModel() };
            accountModel.Lead.Id = leadId;
            var accounts = new List<Account> { new Account { CurrencyType = Currency.TRL } };
            _accountRepositoryMock.Setup(m => m.GetByLead(leadId)).ReturnsAsync(accounts);
            var expected = "Authorization error. The lead role does not allow you to create accounts other than dollar.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.AddAccount(Role.Regular, accountModel))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepositoryMock.Verify(m => m.GetByLead(leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, "Authorisation error. The lead role does not allow you to create accounts other than dollar.");
        }
        #endregion

        #region UpdateAccountTests
        [Test]
        public async Task UpdateAccountTest_ShouldUpdatedAccount()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            var account = new Account { Id = accountId, Name = "Ne vazno", Lead = new Lead() };
            account.Lead.Id = leadId;
            _accountRepositoryMock.Setup(m => m.GetById(accountId)).ReturnsAsync(account);

            //when
            await sut.UpdateAccount(leadId, new AccountModel { Id = accountId });

            //then
            _accountRepositoryMock.Verify(m => m.UpdateAccountById(It.IsAny<Account>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Request was received to update an account ID = {accountId}.");
        }

        [Test]
        public void UpdateAccount_AccountNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            _accountRepositoryMock.Setup(m => m.GetById(accountId)).ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.UpdateAccount(leadId, new AccountModel { Id = accountId }))!
                .Message;

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once());
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateAccount_LeadDontHaveAccesToAccount_ShouldThrowAuthorizationException()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            var authorizathionLeadId = 1;
            var account = new Account { Id = accountId, Name = "Ne vazno", Lead = new Lead() };
            account.Lead.Id = leadId;
            _accountRepositoryMock.Setup(m => m.GetById(accountId)).ReturnsAsync(account);
            var expected = $"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.UpdateAccount(authorizathionLeadId, new AccountModel { Id = accountId }))!
                .Message;

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once());
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region LockByIdTests
        [Test]
        public async Task LockByIdTest_ShouldLockAccount()
        {
            //given
            var accountId = 21;
            var account = new Account { Id = accountId };
            _accountRepositoryMock.Setup(m => m.GetById(accountId)).ReturnsAsync(account);

            //when
            await sut.LockById(accountId);

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once());
            _accountRepositoryMock.Verify(m => m.LockById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Request was received to lock an account ID =  {accountId}.");
        }

        [Test]
        public void LockById_AccountNotFound_ShouldThrowNotFoundException()
        {
            //given
            var accountId = 21;
            _accountRepositoryMock.Setup(m => m.GetById(accountId)).ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.LockById(accountId))!
                .Message;

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once());
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LockById_CurrencyIsRub_ShouldThrowBadRequestException()
        {
            //given
            var accountId = 42;
            _accountRepositoryMock
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Id = accountId, CurrencyType = Currency.RUB });
            var expected = "Error: it is forbidden to block ruble accounts.";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await sut.LockById(accountId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, "Error: it is forbidden to block ruble accounts.");
        }
        #endregion

        #region UnlockByIdTests
        [Test]
        public async Task UnlockByIdTest_ShouldUnlockAccount()
        {
            //given
            var accountId = 21;
            _accountRepositoryMock
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Id = accountId });

            //when
            await sut.UnlockById(accountId);

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once());
            _accountRepositoryMock.Verify(m => m.UnlockById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Request was received to unlock an account ID =  {accountId}.");
        }

        [Test]
        public void UnlockById_AccountNotFound_ShouldThrowNotFoundException()
        {
            //given
            var accountId = 21;
            _accountRepositoryMock.Setup(m => m.GetById(accountId)).ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.UnlockById(accountId))!
                .Message;

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once());
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region GetByLeadTests
        [Test]
        public async Task GetByLeadTest_ShouldReturnListOfAccounts()
        {
            //given
            var leadId = 24;
            _accountRepositoryMock
                .Setup(m => m.GetByLead(leadId))
                .ReturnsAsync(new List<Account> { new Account() });
            _leadRepositoryMock.Setup(m => m.GetById(leadId)).ReturnsAsync(new Lead());

            //when
            var actual = await sut.GetByLead(leadId);


            //then
            _accountRepositoryMock.Verify(m => m.GetByLead(leadId), Times.Once);
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Request to get all accounts.");
        }

        [Test]
        public void GetByLead_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 24;
            _leadRepositoryMock.Setup(m => m.GetById(leadId)).ReturnsAsync((Lead)null!);
            var expected = $"Lead entiy with ID = {leadId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.GetByLead(leadId))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region GetByIdTests
        [Test]
        public async Task GetByIdTest_ShouldReturnAccount()
        {
            //given
            var accountId = 24;
            _accountRepositoryMock
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account());

            //when
            var actual = await sut.GetById(accountId);

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Request for an account with an ID = {accountId}.");
        }

        [Test]
        public void GetById_AccountNotFound_ShouldThrowNotFoundException()
        {
            //given
            var accountId = 24;
            _accountRepositoryMock.Setup(m => m.GetById(accountId)).ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.GetById(accountId))!
                .Message;

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region GetByIdTests_WithLeadId
        [Test]
        public async Task GetByIdTest_WithLeadId_ShouldReturnAccount()
        {
            //given
            var accountId = 24;
            var leadId = 42;
            _accountRepositoryMock
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account
                {
                    Lead = new Lead { Id = leadId },
                });

            //when
            var actual = await sut.GetById(accountId, leadId);

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Request for an account with an ID {accountId} lead with an ID {leadId}.");
        }

        [Test]
        public void GetById_WithLeadId_AccountNotFound_ShouldThrowNotFoundException()
        {
            //given
            var accountId = 24;
            var leadId = 42;
            _accountRepositoryMock.Setup(m => m.GetById(accountId)).ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.GetById(accountId, leadId))!
                .Message;

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetById_WithLeadId_LeadHasNoAccess_ShouldThrowAuthorizationException()
        {
            //given
            var accountId = 24;
            var leadId = 42;
            var leadAutorizationId = 1;
            _accountRepositoryMock
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            var expected = "Authorisation Error. No access to someone else's account.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.GetById(accountId, leadAutorizationId))!
                .Message;

            //then
            _accountRepositoryMock.Verify(m => m.GetById(accountId), Times.Once);
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Authorisation Error. No access to someone else's account.");
        }
        #endregion

        #region GetBalanceTests
        [Test]
        public async Task GetBalanceTest_ShouldReturnBalance()
        {
            //given
            var accountId = 24;
            var leadId = 42;
            var currency = Currency.RUB;
            _accountRepositoryMock
                .Setup(m => m.GetByLead(leadId))
                .ReturnsAsync(new List<Account> { new Account { Id = accountId, CurrencyType = currency } });
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead());

            //when
            var actual = await sut.GetBalance(leadId, currency);

            //then
            _accountRepositoryMock.Verify(m => m.GetByLead(leadId), Times.Once);
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Balance was received.");
        }

        [Test]
        public void GetBalance_LeadNotFound_ShouldThrowNotFoundException()
        {
            //given
            var leadId = 24;
            _leadRepositoryMock.Setup(m => m.GetById(leadId)).ReturnsAsync((Lead)null!);
            var expected = $"Lead entiy with ID = {leadId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.GetBalance(leadId, It.IsAny<Currency>()))!
                .Message;

            //then
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once);
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Request to get all accounts.");
        }

        [Test]
        public void GetBalance_CurrencyIsNotAmongAccounts_ShouldThrowBadRequestException()
        {
            //given
            var accountId = 24;
            var leadId = 42;
            var currency = Currency.RUB;
            _accountRepositoryMock
                .Setup(m => m.GetByLead(leadId))
                .ReturnsAsync(new List<Account> { new Account { Id = accountId, CurrencyType = currency } });
            _leadRepositoryMock
                .Setup(m => m.GetById(leadId))
                .ReturnsAsync(new Lead());
            var expected = "Currency type should be among accounts.";

            //when
            var actual = Assert
                .ThrowsAsync<BadRequestException>(async () => await sut.GetBalance(leadId, Currency.AFN))!
                .Message;

            //then
            _accountRepositoryMock.Verify(m => m.GetByLead(leadId), Times.Once);
            _leadRepositoryMock.Verify(m => m.GetById(leadId), Times.Once);
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, "Balance receipt error. Currency type should be among accounts."); 
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Request to get all accounts.");
        }
        #endregion
    }
}
