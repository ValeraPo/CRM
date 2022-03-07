using AutoMapper;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Tests.TestData;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace CRM.BusinessLayer.Tests.ServiceTests
{
    public class AccountServiceTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<ILeadRepository> _leadRepositoryMock;
        private readonly AccountTestData _accountTestData;
        private readonly IMapper _autoMapper;

        public AccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _leadRepositoryMock = new Mock<ILeadRepository>();
            _accountTestData = new AccountTestData();
            _autoMapper = new Mapper(
                new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));
        }

        [SetUp]
        public void Setup()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
        }

        //[Test]
        //public void AddAccountTest()
        //{
        //    //given
        //    var accountModel = _accountTestData.GetAccountModelForTests();
        //    _accountRepositoryMock.Setup(a => a.AddAccount(It.IsAny<Account>())).Returns(23);
        //    var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object);

        //    //when
        //    sut.AddAccount(accountModel);

        //    //then
        //    _accountRepositoryMock.Verify(m => m.AddAccount(It.IsAny<Account>()), Times.Once());
        //}

        [Test]
        public void AddAccountNegativeTest_AuthorizationExceptionAdmin()
        {
            //given
            var accountModel = _accountTestData.GetAccountModelAdminForTests();
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //then
            Assert.Throws<AuthorizationException>(() => sut.AddAccount(accountModel));
        }

        [Test]
        public void AddAccountNegativeTest_AuthorizationExceptionRegular()
        {
            //given
            var accountModel = _accountTestData.GetAccountModelRegularForTests();
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //then
            Assert.Throws<AuthorizationException>(() => sut.AddAccount(accountModel));
        }

        [Test]
        public void UpdateAccountTest()
        {
            //given
            var account = new Account();
            _accountRepositoryMock.Setup(m => m.UpdateAccountById(It.IsAny<Account>()));
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(account);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //when
            sut.UpdateAccount(42, new AccountModel());

            //then
            _accountRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.UpdateAccountById(It.IsAny<Account>()), Times.Once());
        }

        [Test]
        public void UpdateAccountNegativeTest()
        {
            //given
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Account)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.UpdateAccount(It.IsAny<int>(), new AccountModel()));
        }

        [Test]
        public void LockByIdTest()
        {
            //given
            var account = new Account();
            _accountRepositoryMock.Setup(m => m.LockById(It.IsAny<int>()));
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(account);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //when
            sut.LockById(It.IsAny<int>());

            //then
            _accountRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.LockById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void LockByIdNegativeTest()
        {
            //given
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Account)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.LockById(It.IsAny<int>()));
        }

        [Test]
        public void UnlockByIdTest()
        {
            //given
            var account = new Account();
            _accountRepositoryMock.Setup(m => m.UnlockById(It.IsAny<int>()));
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(account);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //when
            sut.UnlockById(It.IsAny<int>());

            //then
            _accountRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.UnlockById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void UnlockByIdNegativeTest()
        {
            //given
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Account)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.UnlockById(It.IsAny<int>()));
        }

        [Test]
        public void GetByLeadTest()
        {
            //given
            var accounts = _accountTestData.GetListOfAccountsForTests();
            var lead = new Lead();
            _accountRepositoryMock.Setup(m => m.GetByLead(It.IsAny<int>())).Returns(accounts);
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(lead);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //when
            var actual = sut.GetByLead(It.IsAny<int>());

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
        public void GetByLeadNegativeTest()
        {
            //given
            _leadRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns((Lead)null);
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object, _leadRepositoryMock.Object);

            //then
            Assert.Throws<NotFoundException>(() => sut.GetByLead(It.IsAny<int>()));
        }
    }
}
