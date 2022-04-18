﻿using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Helpers;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.ResponseModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Tests
{
    public class RequestHelperTests
    {
        private Mock<IRestClient> _client;
        private IConfiguration _config;
        private Mock<ILogger<RequestHelper>> _logger;
        private RequestHelper _requestHelper;
        private static readonly List<ConfigResponseModel> ListConfigs = new()
        {
            new ConfigResponseModel { Key = "BaseAddress", Value = "80.78.240.4" },
            new ConfigResponseModel { Key = "Address", Value = "::1:4589" }
        };

        private const string _message = "Exceptions test";
        private const Microservice Service = Microservice.MarvelousConfigs;


        [SetUp]
        public void SetUp()
        {
            _client = new Mock<IRestClient>();
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").AddInMemoryCollection(new Dictionary<string, string>()).Build();
            _logger = new Mock<ILogger<RequestHelper>>();
            _requestHelper = new RequestHelper(_logger.Object, _config, _client.Object);
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task SendTransactionPostRequestTests_ShouldSentRequest()
        {
            //given
            var expected = 42;
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse { Content = expected.ToString(), StatusCode = HttpStatusCode.OK });

            //when
            var actual = await _requestHelper.SendTransactionPostRequest("path", new TransactionRequestModel());

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);

        }

        [Test]
        public void SendTransactionPostRequest_WasReturnTimeout_ShouldThrowRequestTimeoutException()
        {
            //given
            var expected = "One or more errors occurred. (Exceptions test)";
            var response = Mock.Of<RestResponse>(m =>  m.StatusCode == HttpStatusCode.RequestTimeout && m.ErrorMessage == _message && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<AggregateException>(() => _requestHelper.SendTransactionPostRequest("path", new TransactionRequestModel()))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Request Timeout {_message}");
        }

        [Test]
        public void SendTransactionPostRequest_WasReturnServiceUnavailable_ShouldThrowServiceUnavailableException()
        {
            //given
            var expected = "One or more errors occurred. (Exceptions test)";
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.ServiceUnavailable && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<AggregateException>(() => _requestHelper.SendTransactionPostRequest("path", new TransactionRequestModel()))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Service Unavailable {_message}");
        }

        [Test]
        public void SendTransactionPostRequest_WasReturnBadRequest_ShouldThrowBadGatewayException()
        {
            //given
            var expected = "One or more errors occurred. (Exceptions test)";
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.BadRequest && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<AggregateException>(() => _requestHelper.SendTransactionPostRequest("path", new TransactionRequestModel()))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Bad Gatеway {_message}");
        }

        [Test]
        public void SendTransactionPostRequest_WasReturnForbiddent_ShouldThrowForbiddenException()
        {
            //given
            var expected = "One or more errors occurred. (Exceptions test)";
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.Forbidden && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<AggregateException>(() => _requestHelper.SendTransactionPostRequest("path", new TransactionRequestModel()))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Forbidden {_message}");
        }

        [Test]
        public void SendTransactionPostRequest_WasReturnNull_ShouldThrowBadGatewayException()
        {
            //given
            var expected = "One or more errors occurred. (Exceptions test)";
            var response = Mock.Of<RestResponse>(m => m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<AggregateException>(() => _requestHelper.SendTransactionPostRequest("path", new TransactionRequestModel()))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Content equal's null {_message}");
        }

        [Test]
        public void SendTransactionPostRequest_WasNotReturnOk_ShouldThrowInternalServerError()
        {
            //given
            var expected = "One or more errors occurred. (Exceptions test)";
            var response = Mock.Of<RestResponse>(m => m.Content == "something" && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<AggregateException>(() => _requestHelper.SendTransactionPostRequest("path", new TransactionRequestModel()))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Error Other Service {_message}");
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task GetBalanceTests_WithList_ShouldReturnBalance()
        {
            //given
            var expected = 100.5;
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse { Content = expected.ToString(), StatusCode = System.Net.HttpStatusCode.OK });

            //when
            var actual = await _requestHelper.GetBalance(new List<int> { 1, 2 }, Currency.AFN);

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task GetBalanceTests_ShouldReturnBalance()
        {
            //given
            var expected = 100.5;
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse { Content = expected.ToString(), StatusCode = System.Net.HttpStatusCode.OK });

            //when
            var actual = await _requestHelper.GetBalance(1, Currency.AFN);

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }

        

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task GenerateRequestTests_ShouldReturnResponse()
        {
            //given
            var expected = new RestResponse { Content = "something", StatusCode = System.Net.HttpStatusCode.OK };
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            //when
            var actual = await _requestHelper.GenerateRequest(It.IsAny<RestRequest>());

            //then
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, actual.StatusCode);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Response received.");
        }

        

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task GetTransactionsTests_ShouldReturnTransactions()
        {
            //given
            var expected = "transactions";
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse { Content = expected, StatusCode = System.Net.HttpStatusCode.OK });

            //when
            var actual = await _requestHelper.GetTransactions(42);

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Response received.");
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Try get transactions by acount id = 42 from Transaction Service.");
        }

        

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task GetTokenTests_ShouldReturnToken()
        {
            //given
            var authModel = new AuthRequestModel { Email = "test@mail.ru", Password = "pass" };
            var expected = "token";
            var response = Mock.Of<RestResponse<string>>(_ => _.Data == expected && _.StatusCode == HttpStatusCode.OK);

            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = await _requestHelper.GetToken(authModel);

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Try get token from Auth Service for email = t**********u.");
        }

        [Test]
        public void GetToken_WasReturnConflict_ShouldThrowIncorrectPasswordException()
        {
            //given
            var expected = $"One or more errors occurred. (Try to login. Incorrected password.)";
            _client
                .Setup(s => s.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse { ErrorException = new Exception(_message), StatusCode = HttpStatusCode.Conflict });

            //when
            var actual = Assert.ThrowsAsync<AggregateException>(() => _requestHelper.SendTransactionPostRequest("path", new TransactionRequestModel()))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync(It.IsAny<RestRequest>(), default), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Try to login. Incorrected password.");
        }

        

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task GetLeadIdentityByTokenTests_ShouldReturnLeadIdentity()
        {
            //given
            var expected = new IdentityResponseModel { Id = 42, Role = "Regular" };
            var response = Mock.Of<RestResponse<IdentityResponseModel>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.OK);

            _client
                .Setup(s => s.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = await _requestHelper.GetLeadIdentityByToken("token");

            //then
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Role, actual.Role);
            _client.Verify(v => v.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Send token token");
        }


        [Test]
        public void GetLeadIdentityByToken_WasReturnTimeout_ShouldThrowRequestTimeoutException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<IdentityResponseModel>>(m => m.Data == new IdentityResponseModel() && m.StatusCode == HttpStatusCode.RequestTimeout && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<RequestTimeoutException>(() => _requestHelper.GetLeadIdentityByToken("token"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Request Timeout {_message}");
        }

        [Test]
        public void GetLeadIdentityByToken_WasReturnServiceUnavailable_ShouldThrowServiceUnavailableException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<IdentityResponseModel>>(m => m.Data == new IdentityResponseModel() && m.StatusCode == HttpStatusCode.ServiceUnavailable && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<ServiceUnavailableException>(() => _requestHelper.GetLeadIdentityByToken("token"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Service Unavailable {_message}");
        }

        [Test]
        public void GetLeadIdentityByToken_WasReturnBadRequest_ShouldThrowBadGatewayException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<IdentityResponseModel>>(m => m.Data == new IdentityResponseModel() && m.StatusCode == HttpStatusCode.BadRequest && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<BadGatewayException>(() => _requestHelper.GetLeadIdentityByToken("token"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Bad Gatеway {_message}");
        }

        [Test]
        public void GetLeadIdentityByToken_WasReturnForbiddent_ShouldThrowForbiddenException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<IdentityResponseModel>>(m => m.Data == new IdentityResponseModel() && m.StatusCode == HttpStatusCode.Forbidden && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<ForbiddenException>(() => _requestHelper.GetLeadIdentityByToken("token"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Forbidden {_message}");
        }

        [Test]
        public void GetLeadIdentityByToken_WasReturnNull_ShouldThrowBadGatewayException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<IdentityResponseModel>>(m => m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<BadGatewayException>(() => _requestHelper.GetLeadIdentityByToken("token"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Content equal's null {_message}");
        }

        [Test]
        public void GetLeadIdentityByToken_WasNotReturnOk_ShouldThrowInternalServerError()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<IdentityResponseModel>>(m => m.Content == "something" && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<InternalServerError>(() => _requestHelper.GetLeadIdentityByToken("token"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<IdentityResponseModel>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Error Other Service {_message}");
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task HashPasswordTests_ShouldReturnHashPassword()
        {
            //given
            var expected = "HashPassword";
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.OK);
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = await _requestHelper.HashPassword("password");

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Send password for hashing");
        }

        [Test]
        public void HashPassword_WasReturnTimeout_ShouldThrowRequestTimeoutException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.RequestTimeout && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<RequestTimeoutException>(() => _requestHelper.HashPassword("password"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Request Timeout {_message}");
        }

        [Test]
        public void HashPassword_WasReturnServiceUnavailable_ShouldThrowServiceUnavailableException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.ServiceUnavailable && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<ServiceUnavailableException>(() => _requestHelper.HashPassword("password"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Service Unavailable {_message}");
        }

        [Test]
        public void HashPassword_WasReturnBadRequest_ShouldThrowBadGatewayException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.BadRequest && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<BadGatewayException>(() => _requestHelper.HashPassword("password"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Bad Gatеway {_message}");
        }

        [Test]
        public void HashPassword_WasReturnForbiddent_ShouldThrowForbiddenException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.Forbidden && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<ForbiddenException>(() => _requestHelper.HashPassword("password"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Forbidden {_message}");
        }

        [Test]
        public void HashPassword_WasReturnNull_ShouldThrowBadGatewayException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<BadGatewayException>(() => _requestHelper.HashPassword("password"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Content equal's null {_message}");
        }

        [Test]
        public void HashPassword_WasNotReturnOk_ShouldThrowInternalServerError()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Content == "something" && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<InternalServerError>(() => _requestHelper.HashPassword("password"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Error Other Service {_message}");
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////
        [Test]
        public async Task SendRequestForConfigsTests_ShouldReturnConfigs()
        {
            //given
            var expected = Mock.Of<RestResponse<string>>(m => m.Data == "configs" && m.StatusCode == HttpStatusCode.OK);

            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            //when
            var actual = await _requestHelper.SendRequestForConfigs<string>("url", "path");

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, "Try get configs from Config Service");
        }

        [Test]
        public void SendRequestForConfigs_WasReturnTimeout_ShouldThrowRequestTimeoutException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.RequestTimeout && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<RequestTimeoutException>(() => _requestHelper.SendRequestForConfigs<string>("url", "path"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Request Timeout {_message}");
        }

        [Test]
        public void SendRequestForConfigs_WasReturnServiceUnavailable_ShouldThrowServiceUnavailableException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.ServiceUnavailable && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<ServiceUnavailableException>(() => _requestHelper.SendRequestForConfigs<string>("url", "path"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Service Unavailable {_message}");
        }

        [Test]
        public void SendRequestForConfigs_WasReturnBadRequest_ShouldThrowBadGatewayException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.BadRequest && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<BadGatewayException>(() => _requestHelper.SendRequestForConfigs<string>("url", "path"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Bad Gatеway {_message}");
        }

        [Test]
        public void SendRequestForConfigs_WasReturnForbiddent_ShouldThrowForbiddenException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Data == expected && m.StatusCode == HttpStatusCode.Forbidden && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<ForbiddenException>(() => _requestHelper.SendRequestForConfigs<string>("url", "path"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Forbidden {_message}");
        }

        [Test]
        public void SendRequestForConfigs_WasReturnNull_ShouldThrowBadGatewayException()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<BadGatewayException>(() => _requestHelper.SendRequestForConfigs<string>("url", "path"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Content equal's null {_message}");
        }

        [Test]
        public void SendRequestForConfigs_WasNotReturnOk_ShouldThrowInternalServerError()
        {
            //given
            var expected = _message;
            var response = Mock.Of<RestResponse<string>>(m => m.Content == "something" && m.ErrorException == new Exception(_message));
            _client
                .Setup(s => s.ExecuteAsync<string>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //when
            var actual = Assert.ThrowsAsync<InternalServerError>(() => _requestHelper.SendRequestForConfigs<string>("url", "path"))!.Message;

            //then
            Assert.AreEqual(expected, actual);
            _client.Verify(v => v.ExecuteAsync<string>(It.IsAny<RestRequest>(), default), Times.Once);
            _client.Verify(v => v.AddMicroservice(Microservice.MarvelousCrm), Times.Once);
            VerifyHelper.VerifyLogger(_logger, LogLevel.Error, $"Error Other Service {_message}");
        }


    }
}