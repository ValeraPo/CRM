using RestSharp;
using RestSharp.Authenticators;

namespace CRM.BusinessLayer;

public class MarvelousAuthenticator : AuthenticatorBase
{
    public MarvelousAuthenticator(string token) : base(token) { }

    protected override ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
        => new(new HeaderParameter(KnownHeaders.Authorization, accessToken));
}