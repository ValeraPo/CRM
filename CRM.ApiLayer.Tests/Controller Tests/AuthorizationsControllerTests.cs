using CRM.APILayer.Controllers;
using CRM.BusinessLayer;
using Marvelous.Contracts.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CRM.ApiLayer.Tests
{
    public class AuthorizationsControllerTests
    {
        private Mock<ILogger<AuthorizationsController>> _logger;
        private Mock<IRequestHelper> _requestHelper;
        private AuthorizationsController _controller;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<AuthorizationsController>>();
            _requestHelper = new Mock<IRequestHelper>();
            _controller = new AuthorizationsController(_logger.Object, _requestHelper.Object);
        }

        [Test]
        public async Task LoginTest_ShouldLogin()
        {
            //given
            var expected = "token";
            var model = new AuthRequestModel { Email = "email", Password = "pass"};
            _requestHelper
                .Setup(m => m.GetToken(model))
                .ReturnsAsync(expected);
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = expected;
            _controller.ControllerContext.HttpContext = context;

            //when
            var actual = (await _controller.Login(model)).Result as OkObjectResult;

            //then
            Assert.AreEqual(StatusCodes.Status200OK, actual!.StatusCode);
            Assert.AreEqual(expected, actual.Value);
            _requestHelper.Verify(m => m.GetToken(model), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }
    }
}
