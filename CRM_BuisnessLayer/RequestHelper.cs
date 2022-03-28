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
            var client = new RestClient(url);
            var request = new RestRequest($"api/Transactions/{path}/", method);
            request.AddBody(requestModel);
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Content == null)
            {
                _logger.Error($"Oshibka na storone Transaction.Store.");
                throw new BadRequestException(response.ErrorException.Message);
            }

            return response;
        }

        public async Task<RestResponse> SendGetRequest(string url, string path, int id)
        {
            var client = new RestClient(url);
            var request = new RestRequest($"api/Transactions/{path}{id}/", Method.Get);
            request.AddParameter("id", id);
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Content == null)
            {
                _logger.Error($"Oshibka na storone Transaction.Store.");
                throw new BadRequestException(response.ErrorException.Message);
            }

            return response;
        }
    }
}
