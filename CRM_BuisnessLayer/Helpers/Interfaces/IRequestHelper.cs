﻿using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.ResponseModels;
using RestSharp;

namespace CRM.BusinessLayer
{
    public interface IRequestHelper
    {
        Task<RestResponse> SendTransactionPostRequest<T>(string path, T requestModel);
        Task<decimal> GetBalance(List<int> accountIds, Currency currency);
        Task<decimal> GetBalance(int accountId, Currency currency);
        Task<RestResponse> GenerateRequest(RestRequest request);
        Task<RestResponse> GetTransactions(int id);
        Task<string> GetToken(AuthRequestModel auth);
        Task<IdentityResponseModel> GetLeadIdentityByToken(string token);
        Task<string> HashPassword(string password);
        Task<RestResponse<T>> SendRequestForConfigs<T>(string url, string path, string jwtToken = "null");
    }
}