using Moq;
using NUnit.Framework;
using AutoMapper;
using CRM.BusinessLayer.Services;
using Microsoft.Extensions.Logging;
using CRM.BusinessLayer.Configurations;
using System.Threading.Tasks;
using CRM.BusinessLayer.Tests.TestData;
using CRM.DataLayer.Repositories.Interfaces;
using CRM.DataLayer.Entities;
using RestSharp;
using CRM.BusinessLayer.Exceptions;
using Marvelous.Contracts;

namespace CRM.BusinessLayer.Tests.ServiceTests
{
    public class TransactionServiceTests
    {

        private readonly IMapper _autoMapper;
        private readonly Mock<ILogger<TransactionService>> _logger;
        private readonly Mock<ITransactionService> _transactionService;
        private Mock<IAccountRepository> _accountRepository;
        private readonly Mock<IRequestHelper> _requestHelper;
        private readonly TransactionTestData _transactionTestData;

        public TransactionServiceTests()
        {
            _logger = new Mock<ILogger<TransactionService>>();
            _autoMapper = new Mapper(
                new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperToData>()));
            _transactionTestData = new TransactionTestData();
            _requestHelper = new Mock<IRequestHelper>();
        }

        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IAccountRepository>();
        }

        [Test]
        public async Task AddDepositTests()
        {

            //given
            var transactionRequestModel = _transactionTestData.GetTransactionRequestModel();
            var account = _transactionTestData.GetAccount();
            _accountRepository.Setup(m => m.GetById(1)).ReturnsAsync(account);
            _requestHelper.Setup(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transactionRequestModel)).ReturnsAsync((RestResponse)null);
            var sut = new TransactionService(_accountRepository.Object, _requestHelper.Object, _logger.Object);

            //when
            sut.AddDeposit(transactionRequestModel, 1);

            //then
            _accountRepository.Verify(m => m.GetById(1), Times.Once());
            _requestHelper.Verify(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transactionRequestModel), Times.Once());
        }
        
        [Test]
        public async Task AddDepositNegativeTests()
        {
            //given
            var transactionRequestModel = _transactionTestData.GetTransactionRequestModel();
            var account = _transactionTestData.GetAccount();
            _accountRepository.Setup(m => m.GetById(2));
            _requestHelper.Setup(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transactionRequestModel)).ReturnsAsync((RestResponse)null);
            var sut = new TransactionService(_accountRepository.Object, _requestHelper.Object, _logger.Object);

            //when

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.AddDeposit(transactionRequestModel, (It.IsAny<int>())));
        }

        [Test]
        public async Task AddTransferTests()
        {
            //given 
            var accountFrom = _transactionTestData.GetAccount();
            var accontTo = _transactionTestData.GetAccount();
            accontTo.Id = 2;
            var transferRequestModel = _transactionTestData.GetTransferRequestModel();
            _accountRepository.Setup(m => m.GetById(1)).ReturnsAsync(accountFrom);
            _accountRepository.Setup(m => m.GetById(2)).ReturnsAsync(accontTo);
            _requestHelper.Setup(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transferRequestModel)).ReturnsAsync((RestResponse)null);
            var sut = new TransactionService(_accountRepository.Object, _requestHelper.Object, _logger.Object);

            //when
            sut.AddTransfer(transferRequestModel, 1);

            //then
            _accountRepository.Verify(m => m.GetById(It.IsAny<int>()), Times.Exactly(2));
            _requestHelper.Verify(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transferRequestModel), Times.Once());
        }
        
        [Test]
        public async Task AddTransferNegativeTests()
        {
            //given 
            var accountFrom = _transactionTestData.GetAccount();
            var transferRequestModel = _transactionTestData.GetTransferRequestModel();
            _accountRepository.Setup(m => m.GetById(1));
            _requestHelper.Setup(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transferRequestModel)).ReturnsAsync((RestResponse)null);
            var sut = new TransactionService(_accountRepository.Object, _requestHelper.Object, _logger.Object);

            //when
            sut.AddTransfer(transferRequestModel, (It.IsAny<int>()));

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.AddTransfer(transferRequestModel, (It.IsAny<int>())));
        }

        [Test]
        public async Task WithdrawTest()
        {
            //given
            var account = _transactionTestData.GetAccount();
            var transactionRequestModel = _transactionTestData.GetTransactionRequestModel();
            _accountRepository.Setup(m => m.GetById(1)).ReturnsAsync(account);
            _requestHelper.Setup(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transactionRequestModel)).ReturnsAsync((RestResponse)null);
            var sut = new TransactionService(_accountRepository.Object, _requestHelper.Object, _logger.Object);

            //when
            sut.Withdraw(transactionRequestModel, 1);

            //then
            _accountRepository.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _requestHelper.Verify(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transactionRequestModel), Times.Once());
        }
        
        [Test]
        public async Task WithdrawNegativeTest()
        {
            //given
            var account = _transactionTestData.GetAccount();
            var transactionRequestModel = _transactionTestData.GetTransactionRequestModel();
            _accountRepository.Setup(m => m.GetById(1));
            _requestHelper.Setup(m => m.SendRequest(It.IsAny<string>(), It.IsAny<string>(), RestSharp.Method.Post, transactionRequestModel)).ReturnsAsync((RestResponse)null);
            var sut = new TransactionService(_accountRepository.Object, _requestHelper.Object, _logger.Object);

            //when

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.Withdraw(transactionRequestModel, (It.IsAny<int>())));
        }

        [Test]
        public async Task GetBalanceTests()
        {
            //given
            var account = _transactionTestData.GetAccount();
            _accountRepository.Setup(m => m.GetById(1)).ReturnsAsync(account);
            _requestHelper.Setup(m => m.SendGetRequest(It.IsAny<string>(), 1)).ReturnsAsync((RestResponse)null);
            var sut = new TransactionService(_accountRepository.Object, _requestHelper.Object, _logger.Object);

            //when
            var balance = sut.GetBalance(1);

            //then
            _accountRepository.Verify(m => m.GetById(It.IsAny<int>()), Times.Once());
            _requestHelper.Verify(m => m.SendGetRequest(It.IsAny<string>(), 1), Times.Once());
        }
        
        [Test]
        public async Task GetBalanceNegativeTests()
        {
            //given
            var account = _transactionTestData.GetAccount();
            _accountRepository.Setup(m => m.GetById(1));
            _requestHelper.Setup(m => m.SendGetRequest(It.IsAny<string>(), 1)).ReturnsAsync((RestResponse)null);
            var sut = new TransactionService(_accountRepository.Object, _requestHelper.Object, _logger.Object);

            //when

            //then
            Assert.ThrowsAsync<NotFoundException>(async () => await sut.GetBalance(1));
        }

    }
}
