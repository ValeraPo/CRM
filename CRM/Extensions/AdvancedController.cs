using CRM.BusinessLayer;
using CRM.BusinessLayer.Exceptions;
using FluentValidation;
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
        private readonly ILogger<AdvancedController> _logger;


        public AdvancedController(IConfiguration configuration, 
            IRequestHelper requestHelper,
             ILogger<AdvancedController> logger)
        {
            _configuration = configuration;
            _requestHelper = requestHelper;
            _logger = logger;
        }

        protected async Task CheckRole(params Role[] roles)
        {
            var identity = await GetIdentity();

            if (identity.Role == null)
            {
                var ex = new ForbiddenException($"Invalid token");
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
            if (!roles.Select(r => r.ToString()).Contains(identity.Role))
            {
                var ex = new ForbiddenException($"Lead id = {identity.Id} doesn't have access to this endpiont");
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
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
            {
                var ex = new ForbiddenException($"Anonimus doesn't have access to this endpiont");
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
            var identity = await _requestHelper.GetLeadIdentityByToken(token);
            return identity.Data;
        }

        protected void Validation<T>(T requestModel, IValidator<T> validator)
        {
            if (requestModel == null)
            {
                var ex = new BadRequestException("You must specify the table details in the request body");
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
            var validationResult = validator.Validate(requestModel);
            if (!validationResult.IsValid)
            {
                var ex = new ValidationException(validationResult.Errors);
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
