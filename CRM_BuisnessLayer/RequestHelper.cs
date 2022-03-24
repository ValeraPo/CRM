using CRM.BusinessLayer.Exceptions;
using RestSharp;

namespace CRM.BusinessLayer
{
    public class RequestHelper : IRequestHelper
    {
        public async Task<RestResponse> SendRequest<T>(string url, string path, Method method, T requestModel)
        {
            var client = new RestClient(url);
            var request = new RestRequest($"api/Transactions/{path}/", method);
            request.AddBody(requestModel);
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Content == null)
                throw new BadRequestException(response.ErrorException.Message);

            return response;
        }
    }
}
