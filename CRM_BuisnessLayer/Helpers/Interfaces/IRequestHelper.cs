using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.ResponseModels;
using RestSharp;

namespace CRM.BusinessLayer
{
    public interface IRequestHelper
    {
        Task<RestResponse> SendRequest<T>(string url, string path, Method method, T requestModel);
        Task<RestResponse> GetBalance(string url, List<int> accountIds, Currency currency);
        Task<RestResponse> GenerateRequest(RestRequest request, string url);
        Task<RestResponse> SendGetRequest(string url, string path, int id);
        Task<RestResponse> GetTransactions(string url, string path, int id);
        Task<RestResponse> GetToken(AuthRequestModel auth);
        Task<RestResponse<IdentityResponseModel>> GetLeadIdentityByToken(string url, string path, string token);
        Task<RestResponse<T>> SendRequestForConfigs<T>(string url, string path, string jwtToken = "null");
        //Task<bool> CheckTokenMicroservice(string token);
    }
}