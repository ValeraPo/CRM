using CRM.BusinessLayer.Exceptions;
using NLog;
using RestSharp;

namespace CRM.BusinessLayer
{
    public class RequestHelper : IRequestHelper
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<RestResponse> SendRequest<T>(string url, string path, Method method, T requestModel)
        {
            var request = new RestRequest($"api/Transactions/{path}/", method);
            request.AddBody(requestModel);
            return await GenerateRequest(request, url);
        }

        public async Task<RestResponse> SendGetRequest(string url, string path, int id)
        {
            var request = new RestRequest($"api/Transactions/{path}{id}/", Method.Get);
            request.AddParameter("id", id);
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
                _logger.Error("408 Request Timeout");
                throw new BadRequestException(response.ErrorException.Message);
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Content == null)
            {
                _logger.Error($"Oshibka na storone Transaction.Store.");
                throw new BadRequestException(response.ErrorException.Message);
            }
        }
    }
}
