using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Services;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Tests.ServiceTests
{
    public class TransactionServiceTests
    {

        private Mock<ILogger<TransactionService>> _logger;
        private Mock<IAccountRepository> _accountRepository;
        private readonly Mock<IRequestHelper> _requestHelper;
        private TransactionService sut;

        public TransactionServiceTests()
        {
            _requestHelper = new Mock<IRequestHelper>();
        }

        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _logger = new Mock<ILogger<TransactionService>>();
            sut = new TransactionService(_accountRepository.Object,
                _requestHelper.Object,
                _logger.Object);
        }

        [Test]
        public async Task AddDepositTest_ShouldSentDeposit()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            var transactionRequestModel = new TransactionRequestModel { AccountId = accountId};
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Lead = new Lead {Id = leadId } });
            _requestHelper
                .Setup(m => m.SendTransactionPostRequest(TransactionEndpoints.Deposit, transactionRequestModel))
                .ReturnsAsync(1);
            

            //when
            await sut.AddDeposit(transactionRequestModel, leadId);

            //then
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            _requestHelper.Verify(m => m.SendTransactionPostRequest(TransactionEndpoints.Deposit, transactionRequestModel), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 3);
        }

        [Test]
        public void AddDeposit_AccountNotFound_ShouldThrowException()
        {
            //given
            var accountId = 21;
            var transactionRequestModel = new TransactionRequestModel { AccountId = accountId };
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.AddDeposit(transactionRequestModel, It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to add a deposit to an account with ID =  {accountId}.");
        }

        [Test]
        public void AddDeposit_LeadHasNoAccess_ShouldThrowAuthorizationExceptio()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            var authorizathionLeadId = 1;
            var transactionRequestModel = new TransactionRequestModel { AccountId = accountId };
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _requestHelper
                .Setup(m => m.SendTransactionPostRequest(TransactionEndpoints.Deposit, transactionRequestModel))
                .ReturnsAsync(1);
            var expected = $"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.AddDeposit(transactionRequestModel, authorizathionLeadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to add a deposit to an account with ID =  {accountId}.");
        }

        [Test]
        public async Task AddTransferTest_ShouldSentTransfer()
        {
            //given
            var leadId = 42;
            var accountIdTo = 21;
            var accountIdFrom = 42;
            var transactionRequestModel = new TransferRequestModel { AccountIdTo = accountIdTo, AccountIdFrom = accountIdFrom };
            _accountRepository
                .Setup(m => m.GetById(accountIdTo))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _accountRepository
                .Setup(m => m.GetById(accountIdFrom))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _requestHelper
                .Setup(m => m.SendTransactionPostRequest(TransactionEndpoints.Transfer, transactionRequestModel))
                .ReturnsAsync(1);

            //when
            await sut.AddTransfer(transactionRequestModel, leadId);

            //then
            _accountRepository.Verify(m => m.GetById(accountIdTo), Times.Once());
            _accountRepository.Verify(m => m.GetById(accountIdFrom), Times.Once());
            _requestHelper.Verify(m => m.SendTransactionPostRequest(TransactionEndpoints.Transfer, transactionRequestModel), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 3);
        }

        [Test]
        public void AddTransfer_AccountToNotFound_ShouldThrowException()
        {
            //given
            var accountIdTo = 21;
            var accountIdFrom = 42;
            var leadId = 10;
            var transactionRequestModel = new TransferRequestModel { AccountIdTo = accountIdTo, AccountIdFrom = accountIdFrom };
            _accountRepository
                .Setup(m => m.GetById(accountIdFrom))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _accountRepository
                .Setup(m => m.GetById(accountIdTo))
                .ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountIdTo} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.AddTransfer(transactionRequestModel, leadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountIdFrom), Times.Once());
            _accountRepository.Verify(m => m.GetById(accountIdTo), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Transfer request received from account with ID {accountIdFrom} to account with ID {accountIdTo}.");
        }

        [Test]
        public void AddTransfer_AccountFromNotFound_ShouldThrowException()
        {
            //given
            var accountIdTo = 21;
            var accountIdFrom = 42;
            var transactionRequestModel = new TransferRequestModel { AccountIdTo = accountIdTo, AccountIdFrom = accountIdFrom };
            _accountRepository
                .Setup(m => m.GetById(accountIdFrom))
                .ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountIdFrom} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.AddTransfer(transactionRequestModel, It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountIdFrom), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Transfer request received from account with ID {accountIdFrom} to account with ID {accountIdTo}.");
        }

        [Test]
        public void AddTransfer_LeadHasNoAccessAccountTo_ShouldThrowAuthorizationExceptio()
        {
            //given
            var leadId = 42;
            var authorizathionLeadId = 1;
            var accountIdTo = 21;
            var accountIdFrom = 42;
            var transactionRequestModel = new TransferRequestModel { AccountIdTo = accountIdTo, AccountIdFrom = accountIdFrom };
            _accountRepository
                .Setup(m => m.GetById(accountIdFrom))
                .ReturnsAsync(new Account { Lead = new Lead { Id = authorizathionLeadId } });
            _accountRepository
                .Setup(m => m.GetById(accountIdTo))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _requestHelper
                .Setup(m => m.SendTransactionPostRequest(TransactionEndpoints.Transfer, transactionRequestModel))
                .ReturnsAsync(1);
            var expected = $"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.AddTransfer(transactionRequestModel, authorizathionLeadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountIdTo), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Transfer request received from account with ID {accountIdFrom} to account with ID {accountIdTo}.");
        }

        [Test]
        public void AddTransfer_LeadHasNoAccessAccountFrom_ShouldThrowAuthorizationExceptio()
        {
            //given
            var leadId = 42;
            var authorizathionLeadId = 1;
            var accountIdTo = 21;
            var accountIdFrom = 42;
            var transactionRequestModel = new TransferRequestModel { AccountIdTo = accountIdTo, AccountIdFrom = accountIdFrom };
            _accountRepository
                .Setup(m => m.GetById(accountIdFrom))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _requestHelper
                .Setup(m => m.SendTransactionPostRequest(TransactionEndpoints.Transfer, transactionRequestModel))
                .ReturnsAsync(1);
            var expected = $"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.AddTransfer(transactionRequestModel, authorizathionLeadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountIdFrom), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Transfer request received from account with ID {accountIdFrom} to account with ID {accountIdTo}.");
        }

        [Test]
        public async Task WithdrawTest_ShouldSentWithdraw()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            var transactionRequestModel = new TransactionRequestModel { AccountId = accountId };
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _requestHelper
                .Setup(m => m.SendTransactionPostRequest(TransactionEndpoints.Withdraw, transactionRequestModel))
                .ReturnsAsync(1);

            //when
            await sut.Withdraw(transactionRequestModel, leadId);

            //then
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            _requestHelper.Verify(m => m.SendTransactionPostRequest(TransactionEndpoints.Withdraw, transactionRequestModel), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 3);
        }

        [Test]
        public void Withdraw_AccountNotFound_ShouldThrowException()
        {
            //given
            var accountId = 21;
            var transactionRequestModel = new TransactionRequestModel { AccountId = accountId };
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.Withdraw(transactionRequestModel, It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received withdraw request from account with ID = {accountId}.");
        }

        [Test]
        public void Withdraw_LeadHasNoAccess_ShouldThrowAuthorizationExceptio()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            var authorizathionLeadId = 1;
            var transactionRequestModel = new TransactionRequestModel { AccountId = accountId };
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _requestHelper
                .Setup(m => m.SendTransactionPostRequest(TransactionEndpoints.Withdraw, transactionRequestModel))
                .ReturnsAsync(1);
            var expected = $"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.Withdraw(transactionRequestModel, authorizathionLeadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received withdraw request from account with ID = {accountId}.");
        }

        [Test]
        public async Task GetTransactionsByAccountIdTest_ShouldReturnTransactions()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _requestHelper
                .Setup(m => m.GetTransactions(accountId))
                .ReturnsAsync("fff");

            //when
            await sut.GetTransactionsByAccountId(accountId, leadId);

            //then
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            _requestHelper.Verify(m => m.GetTransactions(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 3);
        }

        [Test]
        public void GetTransactionsByAccountId_AccountNotFound_ShouldThrowException()
        {
            //given
            var accountId = 21;
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync((Account)null!);
            var expected = $"Account entiy with ID = {accountId} not found";

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await sut.GetTransactionsByAccountId(accountId, It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Popytka polucheniia transakcii accounta id = {accountId}.");
        }

        [Test]
        public void GetTransactionsByAccountId_LeadHasNoAccess_ShouldThrowAuthorizationExceptio()
        {
            //given
            var leadId = 42;
            var accountId = 21;
            var authorizathionLeadId = 1;
            _accountRepository
                .Setup(m => m.GetById(accountId))
                .ReturnsAsync(new Account { Lead = new Lead { Id = leadId } });
            _requestHelper
                .Setup(m => m.GetTransactions(accountId))
                .ReturnsAsync("fff");
            var expected = $"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.";

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await sut.GetTransactionsByAccountId(accountId, authorizathionLeadId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _accountRepository.Verify(m => m.GetById(accountId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Popytka polucheniia transakcii accounta id = {accountId}.");
        }

    }
}
