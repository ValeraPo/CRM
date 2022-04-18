using Marvelous.Contracts.Enums;
using RestSharp;

namespace CRM.BusinessLayer.Helpers
{

    public class MarvelousRestClient : RestClient, IRestClient
    {

        public RestClient AddMicroservice(Microservice service) 
            => this.AddDefaultHeader(nameof(Microservice), service.ToString());


        public Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = default) =>
            RestClientExtensions.ExecuteAsync<T>(this, request, cancellationToken);

        public new Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default(CancellationToken)) =>

            base.ExecuteAsync(request, cancellationToken);
    }
}
