using AutoMapper;
using CRM.APILayer.Configuration;
using CRM.APILayer.Controllers;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Services;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CRM.ApiLayer.Tests
{
    public class TransactionControllerTests
    {
        private Mock<ITransactionService> _transactionService;
        private Mock<ILogger<TransactionsController>> _logger;
        private Mock<IRequestHelper> _requestHelper;
        private TransactionsController _sut;
        private readonly IMapper _autoMapper;
        private Mock<ICRMProducers> _crmProducers;

        public TransactionControllerTests()
        {
            _autoMapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperFromApi>()));
        }

        [SetUp]
        public void Setup()
        {
            _crmProducers = new Mock<ICRMProducers>();
            _transactionService = new Mock<ITransactionService>();
            _logger = new Mock<ILogger<TransactionsController>>();
            _requestHelper = new Mock<IRequestHelper>();
            _sut = new TransactionsController(_transactionService.Object,
                _logger.Object,
                _autoMapper,
                _requestHelper.Object,
                _crmProducers.Object);
        }

        private void AddContext(string token)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _sut.ControllerContext.HttpContext = context;
        }

        #region AddDepositTests
        [Test]
        public async Task AddDepositTest_ShouldAddDeposit()
        {
            //given
            var token = "token";
            var leadId = 42;
            var model = new TransactionShortRequest { AccountId = 1};
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.AddDeposit(model);

            //then
            _transactionService.Verify(m => m.AddDeposit(It.IsAny<TransactionRequestModel>(), leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void AddDeposit_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.AddDeposit(new TransactionShortRequest()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddDeposit_RoleIsAdmin_ShouldThrowForbiddenException()
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.AddDeposit(new TransactionShortRequest()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddDeposit_AccountNotFound_ShouldThrowException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransactionShortRequest { AccountId = accountId };
            var expected = $"Account entiy with ID = {accountId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.AddDeposit(It.IsAny<TransactionRequestModel>(), leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.AddDeposit(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.AddDeposit(It.IsAny<TransactionRequestModel>(), leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to add a deposit to an account with ID = {accountId}.");
        }

        [Test]
        public void AddDeposit_LeadHasNoAccess_ShouldThrowAuthorizationExceptio()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransactionShortRequest { AccountId = accountId };
            var expected = $"Authorization error. Lead with ID {leadId} doesn't have acces.";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.AddDeposit(It.IsAny<TransactionRequestModel>(), leadId))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.AddDeposit(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.AddDeposit(It.IsAny<TransactionRequestModel>(), leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received a request to add a deposit to an account with ID = {accountId}.");
        }
        #endregion

        #region AddTransferTests
        [Test]
        public async Task AddTransferTest_ShouldAddTransfer()
        {
            //given
            var token = "token";
            var leadId = 42;
            var model = new TransferShortRequest { AccountIdTo = 1, AccountIdFrom = 2 };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.AddTransfer(model);

            //then
            _transactionService.Verify(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void AddTransfer_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.AddTransfer(new TransferShortRequest()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddTransfer_RoleIsAdmin_ShouldThrowForbiddenException()
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.AddTransfer(new TransferShortRequest()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void AddTransfer_AccountToNotFound_ShouldThrowException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransferShortRequest { AccountIdTo = 1, AccountIdFrom = 2 };
            var expected = $"Account entiy with ID = {accountId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.AddTransfer(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Transfer request received from account with ID 2 to account with ID 1.");
        }

        [Test]
        public void AddTransfer_AccountFromNotFound_ShouldThrowException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransferShortRequest { AccountIdTo = 1, AccountIdFrom = 2 };
            var expected = $"Authorization error. Lead with ID {leadId} doesn't have acces.";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.AddTransfer(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Transfer request received from account with ID 2 to account with ID 1.");
        }

        [Test]
        public void AddTransfer_LeadHasNoAccessAccountTo_ShouldThrowAuthorizationExceptio()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransferShortRequest { AccountIdTo = 1, AccountIdFrom = 2 };
            var expected = $"Account entiy with ID = {accountId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.AddTransfer(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Transfer request received from account with ID 2 to account with ID 1.");
        }

        [Test]
        public void AddTransfer_LeadHasNoAccessAccountFrom_ShouldThrowAuthorizationExceptio()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransferShortRequest { AccountIdTo = 1, AccountIdFrom = 2 };
            var expected = $"Authorization error. Lead with ID {leadId} doesn't have acces.";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.AddTransfer(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.AddTransfer(It.IsAny<TransferRequestModel>(), leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Transfer request received from account with ID 2 to account with ID 1.");
        }
        #endregion

        #region WithdrawTests
        [Test]
        public async Task WithdrawTest_ShouldWithdraw()
        {
            //given
            var token = "token";
            var leadId = 42;
            var model = new TransactionShortRequest { AccountId = 1 };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.Withdraw(model);

            //then
            _transactionService.Verify(m => m.Withdraw(It.IsAny<TransactionRequestModel>(), leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            _crmProducers.Verify(m => m.NotifyWhithdraw(leadId, It.IsAny<TransactionRequestModel>()), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void Withdraw_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.Withdraw(new TransactionShortRequest()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void Withdraw_RoleIsAdmin_ShouldThrowForbiddenException()
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.Withdraw(new TransactionShortRequest()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void Withdraw_AccountNotFound_ShouldThrowException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransactionShortRequest { AccountId = accountId };
            var expected = $"Account entiy with ID = {accountId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.Withdraw(It.IsAny<TransactionRequestModel>(), leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.Withdraw(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.Withdraw(It.IsAny<TransactionRequestModel>(), leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received withdrawal request from account with ID = {accountId}.");
        }

        [Test]
        public void Withdraw_LeadHasNoAccess_ShouldThrowAuthorizationExceptio()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransactionShortRequest { AccountId = accountId };
            var expected = $"Authorization error. Lead with ID {leadId} doesn't have acces.";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.Withdraw(It.IsAny<TransactionRequestModel>(), leadId))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.Withdraw(model))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.Withdraw(It.IsAny<TransactionRequestModel>(), leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Received withdrawal request from account with ID = {accountId}.");
        }
        #endregion

        #region GetTransactionsByAccountIdTests
        [Test]
        public async Task GetTransactionsByAccountIdTest_ShouldGetTransactionsByAccountId()
        {
            //given
            var token = "token";
            var accountId = 42;
            var leadId = 21;
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _sut.GetTransactionsByAccountId(accountId);

            //then
            _transactionService.Verify(m => m.GetTransactionsByAccountId(accountId, leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        [Test]
        public void GetTransactionsByAccountId_TokenIsNull_ShouldThrowForbiddenException()
        {
            // given
            var token = (string)null;
            AddContext(token);
            var expected = $"Anonimus doesn't have access to this endpiont";

            //when
            var actual = Assert
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetTransactionsByAccountId(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetTransactionsByAccountId_RoleIsAdmin_ShouldThrowForbiddenException()
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
                .ThrowsAsync<ForbiddenException>(async () => await _sut.GetTransactionsByAccountId(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public void GetTransactionsByAccountId_AccountNotFound_ShouldThrowException()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransactionRequestModel { AccountId = accountId };
            var expected = $"Account entiy with ID = {accountId} not found";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.GetTransactionsByAccountId(accountId, leadId))
                .Callback(() => throw new NotFoundException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<NotFoundException>(async () => await _sut.GetTransactionsByAccountId(accountId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.GetTransactionsByAccountId(accountId, leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Poluchen zapros na poluchenie transakcii c accounta id = 1");
        }

        [Test]
        public void GetTransactionsByAccountId_LeadHasNoAccess_ShouldThrowAuthorizationExceptio()
        {
            //given
            var token = "token";
            var leadId = 42;
            var accountId = 1;
            var model = new TransactionRequestModel { AccountId = accountId };
            var expected = $"Authorization error. Lead with ID {leadId} doesn't have acces.";
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            _transactionService
                .Setup(m => m.GetTransactionsByAccountId(accountId, leadId))
                .Callback(() => throw new AuthorizationException(expected));
            AddContext(token);

            //when
            var actual = Assert
                .ThrowsAsync<AuthorizationException>(async () => await _sut.GetTransactionsByAccountId(accountId))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _transactionService.Verify(m => m.GetTransactionsByAccountId(accountId, leadId), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, $"Poluchen zapros na poluchenie transakcii c accounta id = 1");
        }
        #endregion
    }
}