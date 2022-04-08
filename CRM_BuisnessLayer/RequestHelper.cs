using CRM.BusinessLayer.Exceptions;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.Urls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CRM.BusinessLayer
{
    public class RequestHelper : IRequestHelper
    {
        private readonly ILogger<RequestHelper> _logger;
        private readonly IConfiguration _config;

        public RequestHelper(ILogger<RequestHelper> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<RestResponse> SendRequest<T>(string url, string path, Method method, T requestModel)
        {
            var request = new RestRequest($"{TransactionUrls.ApiTransactions}{path}/", method);
            request.AddBody(requestModel!);
            return await GenerateRequest(request, url);
        }

        public async Task<RestResponse> GetBalance(string url, List<int> accountIds, Currency currency)
        {
            var request = new RestRequest("api/balance", Method.Get);
            foreach (var id in accountIds)
            {
                request.AddParameter("id", id);
            }
            request.AddParameter("currency", (int)currency);

            return await GenerateRequest(request, url);
        }

        public async Task<RestResponse> GenerateRequest(RestRequest request, string url)
        {
            var client = new RestClient(url);
            var response = await client.ExecuteAsync(request);
            CheckTransactionError(response);
            return response;
        }

        public async Task<RestResponse> SendGetRequest(string url, string path, int id)
        {
            var request = new RestRequest($"{TransactionUrls.ApiTransactions}{path}{id}/", Method.Get);
            request.AddParameter("id", id);
            return await GenerateRequest(request, url);
        }

        public async Task<RestResponse> GetTransactions(string url, string path, int id)
        {
            var c = TransactionUrls.Url;
            var request = new RestRequest($"{TransactionUrls.ApiTransactions}{path}", Method.Get);
            request.AddParameter("accountIds", id);
            return await GenerateRequest(request, url);
        }

        public async Task<RestResponse> GetToken(AuthRequestModel auth)
        {
            var cc = _config["identityUrl"];
            var client = new RestClient(cc);
            client.AddDefaultHeader(nameof(Microservice), Microservice.MarvelousCrm.ToString());
            var request = new RestRequest($"{AuthUrls.ApiAuth}/{AuthUrls.Login}", Method.Post);
            request.AddBody(auth);
            var response = await client.ExecuteAsync(request);
            CheckTransactionError(response);
            return response;
        }

        public async Task<bool> CheckToken(string token)
        {
            var client = new RestClient(_config["identityUrl"]);
            client.Authenticator = new MarvelousAuthenticator(token);
            client.AddDefaultHeader(nameof(Microservice), Microservice.MarvelousCrm.ToString());
            var request = new RestRequest($"{AuthUrls.ApiAuth}/{AuthUrls.ValidationFront}");
            var response = await client.ExecuteAsync(request);
            return CheckTransactionErrorForToken(response);
        }

        public async Task<bool> CheckTokenMicroservice(string token)
        {
            var client = new RestClient(_config["identityUrl"]);
            client.Authenticator = new MarvelousAuthenticator(token);
            client.AddDefaultHeader(nameof(Microservice), Microservice.MarvelousCrm.ToString());
            var request = new RestRequest($"{AuthUrls.ApiAuth}/{AuthUrls.ValidationMicroservice}");
            var response = await client.ExecuteAsync(request);
            return CheckTransactionErrorForToken(response);
        }

        void CheckTransactionError(RestResponse response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return;
            if (response.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
            {
                _logger.LogError($"Request Timeout {response.ErrorException.Message}");
                throw new RequestTimeoutException(response.ErrorException.Message);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogError($"Service Unavailable {response.ErrorException.Message}");
                throw new ServiceUnavailableException(response.ErrorException.Message);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _logger.LogError($"Bad Gatеway {response.ErrorException.Message}");
                throw new BadGatewayException(response.ErrorException.Message);
            }
            if (response.Content == null)
            {
                _logger.LogError($"Transaction content equal's null {response.ErrorException.Message}");
                throw new BadGatewayException(response.ErrorException.Message);

            }
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogError($"Bad Gatеway {response.ErrorException.Message}");
                throw new ForbiddenException(response.ErrorException.Message);
            }
            _logger.LogError($"Error Transaction.Store. {response.ErrorException.Message}");
            throw new InternalServerError(response.ErrorException.Message);
        }

        bool CheckTransactionErrorForToken(RestResponse response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            
            _logger.LogError($"Validation failed {response.ErrorException.Message}");
            return false;
        }
    }
}
