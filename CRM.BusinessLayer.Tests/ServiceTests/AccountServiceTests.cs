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
        private readonly AccountTestData _accountTestData;
        private readonly IMapper _autoMapper;

        public AccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
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
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object);

            //then
            Assert.Throws<AuthorizationException>(() => sut.AddAccount(accountModel));
        }

        [Test]
        public void AddAccountNegativeTest_AuthorizationExceptionRegular()
        {
            //given
            var accountModel = _accountTestData.GetAccountModelRegularForTests();
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object);

            //then
            Assert.Throws<AuthorizationException>(() => sut.AddAccount(accountModel));
        }

        [Test]
        public void UpdateAccount()
        {
            //given
            var account = new Account();
            _accountRepositoryMock.Setup(m => m.GetById(It.IsAny<int>())).Returns(account);
            _accountRepositoryMock.Setup(m => m.UpdateAccountById(account));
            var sut = new AccountService(_autoMapper, _accountRepositoryMock.Object);

            //when
            sut.UpdateAccount(new AccountModel());

            //then
            _accountRepositoryMock.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _accountRepositoryMock.Verify(m => m.UpdateAccountById(account), Times.Once());
        }


    }
}
