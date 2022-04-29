using CRM.BusinessLayer.Exceptions;
using CRM.DataLayer.Extensions;
using Marvelous.Contracts.Autentificator;
using Marvelous.Contracts.Client;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.ResponseModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;

namespace CRM.BusinessLayer
{
    public class RequestHelper : IRequestHelper
    {
        private readonly ILogger<RequestHelper> _logger;
        private readonly IConfiguration _config;
        private readonly IRestClient _client;

        public RequestHelper(ILogger<RequestHelper> logger,
            IConfiguration config,
            IRestClient client)
        {
            _logger = logger;
            _config = config;
            _client = client;
            _client.AddMicroservice(Microservice.MarvelousCrm);
        }

        public async Task<int> SendTransactionPostRequest<T>(string path, T requestModel)
        {
            _logger.LogInformation("Try send request to TransactionService and added new transaction.");
            _client.Authenticator = new JwtAuthenticator(await GetTokenForMicroservice());
            var request = new RestRequest($"{_config[Microservice.MarvelousTransactionStore.ToString() + "Url"]}{TransactionEndpoints.ApiTransactions}{path}", Method.Post);
            request.AddBody(requestModel!);
            var response = Convert.ToInt32((await ExecuteRequest(request)).Content);
            return response;
        }

        public async Task<decimal> GetBalance(List<int> accountIds, Currency currency)
        {
            _logger.LogInformation($"Try get balance from Transaction Service for account ids = {string.Join(", ", accountIds.ToArray())}.");
            _client.Authenticator = new JwtAuthenticator(await GetTokenForMicroservice());
            var request = new RestRequest($"{_config[Microservice.MarvelousTransactionStore.ToString()+"Url"]}{TransactionEndpoints.ApiBalance}", Method.Get);
            foreach (var id in accountIds)
            {
                request.AddParameter("id", id);
            }
            request.AddParameter("currency", (int)currency);
            var response = Convert.ToDecimal((await ExecuteRequest(request)).Content);
            return response;
        }

        public async Task<decimal> GetBalance(int accountId, Currency currency)
            => await GetBalance(new List<int> { accountId }, currency);

        public async Task<RestResponse> ExecuteRequest(RestRequest request)
        {
            var response = await _client.ExecuteAsync(request);
            _logger.LogInformation("Response received.");
            CheckTransactionError(response);
            return response;
        }

        public async Task<string> GetTransactions(int id)
        {
            _logger.LogInformation($"Try get transactions by acount id = {id} from Transaction Service.");
            _client.Authenticator = new JwtAuthenticator(await GetTokenForMicroservice());
            var request = new RestRequest($"{_config[Microservice.MarvelousTransactionStore.ToString() + "Url"]}{TransactionEndpoints.ApiTransactions}by-accountIds", Method.Get);
            request.AddParameter("accountIds", id);
            return (await ExecuteRequest(request)).Content;
        }

        public async Task<string> GetToken(AuthRequestModel auth)
        {
            _logger.LogInformation($"Try get token from Auth Service for email = {auth.Email.Encryptor()}.");
            var request = new RestRequest($"{_config[Microservice.MarvelousAuth.ToString()]}{AuthEndpoints.ApiAuth}{AuthEndpoints.Login}", Method.Post);
            request.AddBody(auth);
            var response = await _client.ExecuteAsync<string>(request);
            CheckTransactionError(response);
            return response.Data;
        }

        public async Task<IdentityResponseModel> GetLeadIdentityByToken(string token)
        {
            _logger.LogInformation($"Send token {token}");
            _client.Authenticator = new MarvelousAuthenticator(token);
            var request = new RestRequest($"{_config[Microservice.MarvelousAuth.ToString()]}{AuthEndpoints.ApiAuth}{AuthEndpoints.DoubleValidation}");
            var response = await _client.ExecuteAsync<IdentityResponseModel>(request);
            CheckTransactionError(response);
            return response.Data;
        }

        public async Task<string> HashPassword(string password)
        {
            _logger.LogInformation($"Send password for hashing");
            var request = new RestRequest($"{_config[Microservice.MarvelousAuth.ToString()]}{AuthEndpoints.ApiAuth}{AuthEndpoints.Hash}", Method.Post);
            request.AddBody(password);
            var response = await _client.ExecuteAsync<string>(request);
            CheckTransactionError(response);
            return response.Data;
        }

        public async Task<RestResponse<T>> SendRequestForConfigs<T>(string url, string path, string jwtToken = "null")
        {
            _logger.LogInformation($"Try get configs from Config Service");
            var request = new RestRequest($"{url}{path}");
            _client.Authenticator = new JwtAuthenticator(jwtToken);
            var response = await _client.ExecuteAsync<T>(request);
            CheckTransactionError(response);

            return response;
        }

        private async Task<string> GetTokenForMicroservice()
        {
            _logger.LogInformation($"Try get token.");
            var request = new RestRequest($"{_config[Microservice.MarvelousAuth.ToString()]}{AuthEndpoints.ApiAuth}{AuthEndpoints.TokenForMicroservice}", Method.Get);
            var response = await _client.ExecuteAsync<string>(request);
            CheckTransactionError(response);
            return response.Data;
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
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogError($"Try to login. Incorrected password.");
                throw new IncorrectPasswordException("Try to login. Incorrected password.");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogError($"Forbidden {response.ErrorException.Message}");
                throw new ForbiddenException(response.ErrorException.Message);
            }
            if (response.Content == null)
            {
                _logger.LogError($"Content equal's null {response.ErrorException.Message}");
                throw new BadGatewayException(response.ErrorException.Message);
            }
            _logger.LogError($"Error Other Service {response.ErrorException.Message}");
            throw new InternalServerError(response.ErrorException.Message);
        }

    }
}
