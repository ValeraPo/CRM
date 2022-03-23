using AutoMapper;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Tests.TestData;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts;
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
        private readonly AccountTestData _accountTestData;
        private readonly IMapper _autoMapper;
        private readonly Mock<ILogger<AccountService>> _logger;

        public AccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _leadRepositoryMock = new Mock<ILeadRepository>();
            _accountTestData = new AccountTestData();
            _autoMapper = new Mapper(
                new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));
            _logger = new Mock<ILogger<AccountService>>();

        }

        [SetUp]
        public async Task Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
        }

        [Test]
        public async Task AddAccountTest()
        {
            //given
            var accountModel = _accountTestData.GetAccountModelVipForTests();
            var accounts = new List<Account>();
            _accountRepositoryMock.Setup(m => m.GetByLead(It.IsAny<int>())).ReturnsAsync(accounts);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //when
            sut.AddAccount((int)Role.Vip, accountModel);

            //then
            _accountRepositoryMock.Verify(m => m.AddAccount(It.IsAny<Account>()), Times.Once());
        }

        [Test]
        public async Task AddAccountNegativeTest_DuplicationException()
        {
            //given
            var accountModel = _accountTestData.GetAccountModelRegularForTests();
            var accounts = _accountTestData.GetListOfAccountsForTests();
            _accountRepositoryMock.Setup(m => m.GetByLead(It.IsAny<int>())).ReturnsAsync(accounts);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<DuplicationException>(async () => await sut.AddAccount(It.IsAny<int>(), accountModel));
        }

        [Test]
        public async Task AddAccountNegativeTest_AuthorizationExceptionRegular()
        {
            //given
            var accountModel = _accountTestData.GetAccountModelForTests();
            var accounts = new List<Account>();
            _accountRepositoryMock.Setup(m => m.GetByLead(It.IsAny<int>())).ReturnsAsync(accounts);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<AuthorizationException>(async () => await sut.AddAccount((int)Role.Regular, accountModel));
        }

        [Test]
        public async Task UpdateAccountTest()
        {
            //given
            var account = _accountTestData.GetAccountForTests();
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(account);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //when
            sut.UpdateAccount(1, new AccountModel());

            //then
            _accountRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.UpdateAccountById(It.IsAny<Account>()), Times.Once());
        }

        [Test]
        public async Task UpdateAccountNegativeTest_NotFoundException()
        {
            //given
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Account)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.UpdateAccount(It.IsAny<int>(), new AccountModel()));
        }

        [Test]
        public async Task UpdateAccountNegativeTest_AuthorizationException()
        {
            //given
            var account = _accountTestData.GetAccountForTests();
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(account);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<AuthorizationException>(async () => await sut.UpdateAccount(42, new AccountModel()));
        }

        [Test]
        public async Task LockByIdTest()
        {
            //given
            var account = new Account();
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(account);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //when
            sut.LockById(It.IsAny<int>());

            //then
            _accountRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.LockById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task LockByIdNegativeTest()
        {
            //given
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Account)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.LockById(It.IsAny<int>()));
        }

        [Test]
        public async Task UnlockByIdTest()
        {
            //given
            var account = new Account();
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(account);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //when
            sut.UnlockById(It.IsAny<int>());

            //then
            _accountRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.UnlockById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task UnlockByIdNegativeTest()
        {
            //given
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Account)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.UnlockById(It.IsAny<int>()));
        }

        [Test]
        public async Task GetByLeadTest()
        {
            //given
            var accounts = _accountTestData.GetListOfAccountsForTests();
            var lead = new Lead();
            _accountRepositoryMock.Setup(m => m.GetByLead(It.IsAny<int>())).ReturnsAsync(accounts);
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(lead);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //when
            var actual = sut.GetByLead(It.IsAny<int>()).Result;

            //then
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.IsNotNull(actual[i].Id);
                Assert.IsNotNull(actual[i].Name);
                Assert.IsNotNull(actual[i].CurrencyType);
                Assert.IsNotNull(actual[i].Lead);
            }
        }

        [Test]
        public async Task GetByLeadNegativeTest_()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Lead)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.GetByLead(It.IsAny<int>()));
        }

        [Test]
        public async Task GetByIdTest()
        {
            //given
            var account = _accountTestData.GetAccountForTests();
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(account);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //when
            var actual = sut.GetById(1, 1).Result;

            //then
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Id);
            Assert.IsNotNull(actual.Name);
            Assert.IsNotNull(actual.CurrencyType);
            Assert.IsNotNull(actual.Lead);
        }

        [Test]
        public async Task GetByIdNegativeTest_AuthorizationException()
        {
            //given
            var accounts = _accountTestData.GetAccountForTests();
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync(accounts);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<AuthorizationException>(async () => await sut.GetById(100, 100));
        }

        [Test]
        public async Task GetByIdNegativeTest_NotFoundException()
        {
            //given
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).ReturnsAsync((Account)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object, _logger.Object);

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.GetById(It.IsAny<int>(), It.IsAny<int>()));
        }
    }
}
