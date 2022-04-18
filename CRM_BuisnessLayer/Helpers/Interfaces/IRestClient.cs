using Marvelous.Contracts.Enums;
using RestSharp;
using RestSharp.Authenticators;


namespace CRM.BusinessLayer.Helpers
{
    public interface IRestClient
    {
        
        public IAuthenticator? Authenticator { get; set; }
        public RestClient AddMicroservice(Microservice service);
        public Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = default);
        public Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
