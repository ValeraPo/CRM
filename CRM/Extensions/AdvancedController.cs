using CRM.BusinessLayer;
using CRM.BusinessLayer.Exceptions;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace CRM.APILayer.Extensions
{
    public class AdvancedController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IRequestHelper _requestHelper;
        public AdvancedController(IConfiguration configuration, IRequestHelper requestHelper)
        {
            _configuration = configuration;
            _requestHelper = requestHelper;
        }

        protected async Task CheckRole(params Role[] roles)
        {
            var identity = await GetIdentity();

            if (identity.Role == null)
                throw new ForbiddenException($"Invalid token");
            if (!roles.Select(r => r.ToString()).Contains(identity.Role))
                throw new ForbiddenException($"Lead id = {identity.Id} doesn't have access to this endpiont");
        }

        protected async Task<bool> CheckMicroservice(params Microservice[] microservices)
        {
            var identity = await GetIdentity();

            if (!microservices.Select(r => r.ToString()).Contains(identity.IssuerMicroservice))
                return false;
            return true;
        }

        protected async Task<IdentityResponseModel> GetIdentity()
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (token == null) 
                throw new ForbiddenException($"Anonimus doesn't have access to this endpiont");
            var identity = await _requestHelper.GetLeadIdentityByToken(_configuration[Microservice.MarvelousAuth.ToString()],
                "check-double-validate-token/", token);
            return identity.Data;
        }
    }
}
