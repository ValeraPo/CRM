using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ResponseModels;
using Microsoft.Extensions.Configuration;

namespace CRM.BusinessLayer
{
    public class InitializationHelper : IInitializationHelper
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IConfiguration _configuration;
        public InitializationHelper(IRequestHelper requestHelper, IConfiguration configuration)
        {
            _requestHelper = requestHelper;
            _configuration = configuration;
        }

        // Getting configs from cofige service
        public async Task InitializeConfigs()
        {
            var token = await _requestHelper.SendRequestForConfigs<string>(
                _configuration[Microservice.MarvelousAuth.ToString()],
                AuthEndpoints.ApiAuth + AuthEndpoints.TokenForMicroservice);

            var configs = await _requestHelper
                .SendRequestForConfigs<IEnumerable<ConfigResponseModel>>(
                _configuration[Microservice.MarvelousConfigs.ToString()],
                ConfigsEndpoints.Configs, token!.Data);

            foreach (var c in configs!.Data)
            {
                _configuration[c.Key] = c.Value;
            }
        }
    }
}
