﻿using RestSharp;

namespace CRM.BusinessLayer
{
    public class RequestHelper : IRequestHelper
    {
        public async Task<RestResponse> SendRequest<T>(string url, string path, Method method, T requestModel)
        {
            var client = new RestClient(url);
            var request = new RestRequest($"api/Transactions/{path}/", method);
            request.AddBody(requestModel);
            //request.AddJsonBody(requestModel);
            var response = await client.ExecuteAsync(request);

            return response;
        }
    }
}
