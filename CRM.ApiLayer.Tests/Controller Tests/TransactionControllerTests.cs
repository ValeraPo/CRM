using CRM.APILayer.Controllers;
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
        private TransactionsController _controller;


        [SetUp]
        public void Setup()
        {
            _transactionService = new Mock<ITransactionService>();
            _logger = new Mock<ILogger<TransactionsController>>();
            _requestHelper = new Mock<IRequestHelper>();
            _controller = new TransactionsController(_transactionService.Object,
                _logger.Object,
                _requestHelper.Object);
        }

        private void AddContext(string token)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            _controller.ControllerContext.HttpContext = context;
        }

        [Test]
        public async Task AddDepositTest_ShouldAddDeposit()
        {
            //given
            var token = "token";
            var leadId = 42;
            var model = new TransactionRequestModel { AccountId = 1};
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _controller.AddDeposit(model);

            //then
            _transactionService.Verify(m => m.AddDeposit(model, leadId), Times.Once());
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
                .ThrowsAsync<ForbiddenException>(async () => await _controller.AddDeposit(new TransactionRequestModel()))!
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
                .ThrowsAsync<ForbiddenException>(async () => await _controller.AddDeposit(new TransactionRequestModel()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task AddTransferTest_ShouldAddTransfer()
        {
            //given
            var token = "token";
            var leadId = 42;
            var model = new TransferRequestModel { AccountIdTo = 1, AccountIdFrom = 2 };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _controller.AddTransfer(model);

            //then
            _transactionService.Verify(m => m.AddTransfer(model, leadId), Times.Once());
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
                .ThrowsAsync<ForbiddenException>(async () => await _controller.AddTransfer(new TransferRequestModel()))!
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
                .ThrowsAsync<ForbiddenException>(async () => await _controller.AddTransfer(new TransferRequestModel()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

        [Test]
        public async Task WithdrawTest_ShouldWithdraw()
        {
            //given
            var token = "token";
            var leadId = 42;
            var model = new TransactionRequestModel { AccountId = 1 };
            _requestHelper
                .Setup(m => m.GetLeadIdentityByToken(token))
                .ReturnsAsync(new IdentityResponseModel { Id = leadId, Role = "Regular" });
            AddContext(token);

            //when
            await _controller.Withdraw(model);

            //then
            _transactionService.Verify(m => m.Withdraw(model, leadId), Times.Once());
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
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
                .ThrowsAsync<ForbiddenException>(async () => await _controller.Withdraw(new TransactionRequestModel()))!
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
                .ThrowsAsync<ForbiddenException>(async () => await _controller.Withdraw(new TransactionRequestModel()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

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
            await _controller.GetTransactionsByAccountId(accountId);

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
                .ThrowsAsync<ForbiddenException>(async () => await _controller.GetTransactionsByAccountId(It.IsAny<int>()))!
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
                .ThrowsAsync<ForbiddenException>(async () => await _controller.GetTransactionsByAccountId(It.IsAny<int>()))!
                .Message;

            //then
            Assert.AreEqual(expected, actual);
            _requestHelper.Verify(m => m.GetLeadIdentityByToken(token), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, expected);
        }

    }
}