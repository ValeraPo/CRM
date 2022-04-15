using CRM.APILayer.Controllers;
using CRM.BusinessLayer;
using Marvelous.Contracts.RequestModels;
using Microsoft.AspNetCore.Http;
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
        private AuthorizationsController controller;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<AuthorizationsController>>();
            _requestHelper = new Mock<IRequestHelper>();
            controller = new AuthorizationsController(_logger.Object, _requestHelper.Object);
        }

        [Test]
        public async Task LoginTest_ShouldLogin()
        {
            //given
            var token = "token";
            var model = new AuthRequestModel { Email = "email", Password = "pass"};
            _requestHelper
                .Setup(m => m.GetToken(model))
                .ReturnsAsync(token);
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = token;
            controller.ControllerContext.HttpContext = context;

            //when
            await controller.Login(model);

            //then
            _requestHelper.Verify(m => m.GetToken(model), Times.Once());
            VerifyHelper.VerifyLogger(_logger, LogLevel.Information, 2);
        }
    }
}
