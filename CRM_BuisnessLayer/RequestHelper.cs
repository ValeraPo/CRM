using CRM.BusinessLayer.Exceptions;
using Marvelous.Contracts;
using NLog;
using RestSharp;

namespace CRM.BusinessLayer
{
    public class RequestHelper : IRequestHelper
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<RestResponse> SendRequest<T>(string url, string path, Method method, T requestModel)
        {
            var request = new RestRequest($"{TransactionUrls.Api}{path}/", method);
            request.AddBody(requestModel);
            return await GenerateRequest(request, url);
        }

        public async Task<RestResponse> SendGetRequest(string url, string path, int id)
        {
            var request = new RestRequest($"{TransactionUrls.Api}{path}{id}/", Method.Get);
            request.AddParameter("id", id);
            return await GenerateRequest(request, url);
        }

        public async Task<RestResponse> SendGetRequest(string url, string path, List<int> accountIds)
        {
            var request = new RestRequest($"{TransactionUrls.Api}{path}/", Method.Get);
            foreach (var id in accountIds)
            {
                request.AddParameter("accountIds", id);
            }
            return await GenerateRequest(request, url);
        }

        public async Task<RestResponse> GenerateRequest(RestRequest request, string url)
        {
            var client = new RestClient(url);
            var response = await client.ExecuteAsync(request);
            CheckTransactionError(response);
            return response;
        }

        void CheckTransactionError(RestResponse response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
            {
                _logger.Error($"Request Timeout {response.ErrorException.Message}");
                throw new RequestTimeoutException(response.ErrorException.Message);
            } 
            if(response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                _logger.Error($"Service Unavailable {response.ErrorException.Message}");
                throw new ServiceUnavailableException(response.ErrorException.Message);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _logger.Error($"Bad Gatеway {response.ErrorException.Message}");
                throw new BadGatewayException(response.ErrorException.Message);
            }
            if (response.Content == null)
            {
                _logger.Error($"Transaction content equal's null {response.ErrorException.Message}");
                throw new BadGatewayException(response.ErrorException.Message);

            }
            if (response.StatusCode != System.Net.HttpStatusCode.OK )
            {
                _logger.Error($"Oshibka na storone Transaction.Store. {response.ErrorException.Message}");
                throw new InternalServerError(response.ErrorException.Message);
            }
        }
    }
}
