﻿using RestSharp;

namespace CRM.BusinessLayer
{
    public interface IRequestHelper
    {
        Task<RestResponse> SendRequest<T>(string url, string path, Method method, T requestModel);
    }
}