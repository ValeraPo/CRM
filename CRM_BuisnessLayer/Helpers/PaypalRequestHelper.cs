using CRM.BusinessLayer.Models.Paypal;
using CRM.BusinessLayer.Models.Paypal.Request;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json;

namespace CRM.BusinessLayer.Helpers
{
    public class PaypalRequestHelper : IPaypalRequestHelper
    {
        private const string _authTokenParam = "PAYPAL_AUTH_TOKEN";
        private const string _clientIdParam = "PAYPAL_CLIENT_ID";
        private const string _clientSecretParam = "PAYPAL_CLIENT_SECRET";

        private const string _authUrl = "https://api-m.sandbox.paypal.com/v1/oauth2/token";
        private const string _generateInvoiceNumberUrl = "https://api-m.sandbox.paypal.com/v2/invoicing/generate-next-invoice-number";
        private const string _listInvoicesUrl = "https://api-m.sandbox.paypal.com/v2/invoicing/invoices";
        private const string _createDraftInvoideUrl = "https://api-m.sandbox.paypal.com/v2/invoicing/invoices";
        private const string _sendInvoiceUrl = "https://api.sandbox.paypal.com/v2/invoicing/invoices/.InvoiceId./send";
        private const string _getInvoiceUrl = "https://api.sandbox.paypal.com/v2/invoicing/invoices/.InvoiceId.";
        private const string _getPaymentUrl = "https://api.sandbox.paypal.com/v2/payments/captures/.PaymentId.";

        private const string _invoiceIdInUrl = ".InvoiceId.";
        private const string _paymentIdInUrl = ".PaymentId.";


        private IConfiguration _configuration;

        public PaypalRequestHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ListInvoicesResponse> ListInvoices()
        {
            var response = await Request(_listInvoicesUrl, Method.Get);
            var responseContentObject = JsonSerializer.Deserialize<ListInvoicesResponse>(response.Content);
            return responseContentObject;
        }


        public async Task<InvoiceNumberResponse> GetNextInvoiceNumber()
        {
            var response = await Request(_generateInvoiceNumberUrl, Method.Post);
            var responseContentObject = JsonSerializer.Deserialize<InvoiceNumberResponse>(response.Content);
            return responseContentObject;
        }

        public async Task<Link> CreateDraftInvoice(InvoiceRequest request)
        {
            var requestJson = JsonSerializer.Serialize(request);
            var response = await Request(_createDraftInvoideUrl, Method.Post, requestJson);
            var responseContentObject = JsonSerializer.Deserialize<Link>(response.Content);
            return responseContentObject;
        }

        public async Task<Link> SendInvoice(string invoiceId, SendInvoiceRequest request)
        {
            var requestJson = JsonSerializer.Serialize(request);
            var urlToSendInvoice = _sendInvoiceUrl.Replace(_invoiceIdInUrl, invoiceId);
            var response = await Request(urlToSendInvoice, Method.Post, requestJson);
            var responseContentObject = JsonSerializer.Deserialize<Link>(response.Content);
            return responseContentObject;
        }

        public async Task<InvoiceResponse> GetInvoice(string invoiceId)
        {
            var urlToSendInvoice = _getInvoiceUrl.Replace(_invoiceIdInUrl, invoiceId);
            var response = await Request(urlToSendInvoice, Method.Get);
            var responseContentObject = JsonSerializer.Deserialize<InvoiceResponse>(response.Content);
            return responseContentObject;
        }


        public async Task<PaymentResponse> GetPayment(string paymentId)
        {
            var urlToReceivePayment = _getPaymentUrl.Replace(_paymentIdInUrl, paymentId);
            var response = await Request(urlToReceivePayment, Method.Get);
            var responseContentObject = JsonSerializer.Deserialize<PaymentResponse>(response.Content);
            return responseContentObject;
        }


        public string GetInvoiceIdFromLink(Link link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));
            return link.Href.Split('/').Last();
        }


        private async Task GetTokenAndSaveToConfiguration()
        {
            var _paypalClientId = _configuration[_clientIdParam];
            var _paypalSecret = _configuration[_clientSecretParam];

            var client = new RestClient();

            client.Authenticator = new HttpBasicAuthenticator(_paypalClientId, _paypalSecret);
            var request = new RestRequest(_authUrl, Method.Post);
            request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);

            var response = await client.PostAsync(request);
            var authResponse = JsonSerializer.Deserialize<PaypalAuthResponse>(response.Content);
            _configuration[_authTokenParam] = authResponse.AccessToken;
        }

        private async Task<RestResponse?> Request(string url, Method method, string body = null)
        {
            var client = new RestClient();
            var request = new RestRequest(url, method);
            if (body != null)
                request.AddStringBody(body, DataFormat.Json);

            var _authToken = _configuration[_authTokenParam];
            if (string.IsNullOrWhiteSpace(_authToken))
            {
                await GetTokenAndSaveToConfiguration();
                _authToken = _configuration[_authTokenParam];
            }
            client.Authenticator = new JwtAuthenticator(_authToken);

            try
            {
                var response = await client.ExecuteAsync(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("Unauthorized"))
                    await GetTokenAndSaveToConfiguration();
                else
                    throw ex;
            }

            _authToken = _configuration[_authTokenParam];
            client.Authenticator = new JwtAuthenticator(_authToken);
            var responseRetry = await client.ExecuteAsync(request);
            return responseRetry;
        }

    }
}
