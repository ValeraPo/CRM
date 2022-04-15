using Castle.Core.Configuration;
using Moq;
using NUnit.Framework;
using RestSharp;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Tests
{
    public class InitializationHelperTests
    {

        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IRequestHelper> _requestHelper;

        public InitializationHelperTests()
        {
            _requestHelper = new Mock<IRequestHelper>();
            _configuration = new Mock<IConfiguration>();
        }


        //[Test]
        //public async Task InitializeConfigs_ShouldGetConfigs()
        //{
        //    //given
        //    _requestHelper
        //        .Setup(m => m.SendRequestForConfigs<string>(It.IsAny<string>(), It.IsAny<string>()))
        //        .Returns((RestResponse)null!);
        //    var sut = new InitializationHelper(_requestHelper.Object,
        //        _configuration.Object);

        //    //when
        //    await sut.InitializeConfigs();

        //    //then
        //    _requestHelper.Verify(m => m.SendRequestForConfigs<T>(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        //}
    }
}
