using Auth.BusinessLayer.Helpers;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ResponseModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace CRM.BusinessLayer.Tests
{
    public class RequestHelperTests
    {
        private Mock<IRestClient> _client;
        private IConfiguration _config;
        private RequestHelper _requestHelper;
        private static readonly List<ConfigResponseModel> ListConfigs = new()
        {
            new ConfigResponseModel { Key = "BaseAddress", Value = "80.78.240.4" },
            new ConfigResponseModel { Key = "Address", Value = "::1:4589" }
        };

        private const string Message = "Exceptions test";
        private const Microservice Service = Microservice.MarvelousConfigs;
        private Mock<ILogger<RequestHelper>> _logger;


        [SetUp]
        public void SetUp()
        {
            _client = new Mock<IRestClient>();
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").AddInMemoryCollection(new Dictionary<string, string>()).Build();
            _logger = new Mock<ILogger<RequestHelper>>();
            //_requestHelper = new RequestHelper(_logger.Object, _config);
        }


    }
}
