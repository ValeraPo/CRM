using System;
using System.IO;
using System.Threading.Tasks;
using Marvelous.Contracts.ResponseModels;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using FluentValidation;
using NUnit.Framework;
using CRM.APILayer.Infrastructure;
using CRM.BusinessLayer.Exceptions;

namespace CRM.ApiLayer.Tests.InfrastructureTests
{

    public class ErrorExceptionMiddlewareTests
    {
        private DefaultHttpContext _defaultContext;
        private const string ExceptionMassage = "Exception massage";

        [SetUp]
        public void SetUp()
        {
            _defaultContext = new DefaultHttpContext
            {
                Response = { Body = new MemoryStream() },
                Request = { Path = "/" }
            };
        }

        #region Invoke

        [Test]
        public void Invoke_ValidRequestReceived_ShouldResponse()
        {
            //given
            const string expectedOutput = "Request handed over to next request delegate";
            var middlewareInstance = new ErrorExceptionMiddleware(innerHttpContext =>
            {
                innerHttpContext.Response.WriteAsync(expectedOutput);
                return Task.CompletedTask;
            });

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expectedOutput, actual);
        }

        [Test]
        public void Invoke_WhenThrowForbiddenException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(403);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new ForbiddenException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowAuthorizationException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(403);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new AuthorizationException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowNotFoundException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(404);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new NotFoundException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowBadRequestException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(400);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new BadRequestException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowBannedException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(400);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new BannedException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowIncorrectPasswordException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(400);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new IncorrectPasswordException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowIncorrectRoleException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(400);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new IncorrectRoleException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowTypeMismatchException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(400);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new TypeMismatchException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowValidationException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(422);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new ValidationException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowDuplicationException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(409);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new DuplicationException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowRequestTimeoutException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(504);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new RequestTimeoutException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowServiceUnavailableException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(503);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new ServiceUnavailableException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowBadGatewayException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(502);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new BadGatewayException(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowInternalServerError_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(500);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new InternalServerError(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Invoke_WhenThrowException_ShouldExceptionResponseModel()
        {
            //given
            var expected = GetJsonExceptionResponseModel(400);
            var middlewareInstance = new ErrorExceptionMiddleware(_ => throw new Exception(ExceptionMassage));

            //when
            middlewareInstance.Invoke(_defaultContext);

            //then
            var actual = GetResponseBody();
            Assert.AreEqual(expected, actual);
        }

        #endregion

        private static string GetJsonExceptionResponseModel(int statusCode) =>
            JsonSerializer.Serialize(new ExceptionResponseModel { Code = statusCode, Message = ExceptionMassage });

        private string GetResponseBody()
        {
            _defaultContext.Response.Body.Seek(0, SeekOrigin.Begin);
            return new StreamReader(_defaultContext.Response.Body).ReadToEnd();
        }
    }
}
