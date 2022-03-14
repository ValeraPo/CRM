using RestSharp;

namespace CRM.BusinessLayer
{
    public class RequestHelper<T> where T : class
    {
        public Task<RestResponse> GenerateRequest(string url, string path, Method method, T requestModel)
        {
            var client = new RestClient(url);
            var request = new RestRequest(path, method);
            request.AddJsonBody(requestModel);
            var response = client.ExecuteAsync(request);

            return response;
        }
    }
}
