using CRM.BusinessLayer;
using CRM.BusinessLayer.Exceptions;
using FluentValidation;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace CRM.APILayer.Extensions
{
    public class AdvancedController : Controller
    {
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<AdvancedController> _logger;


        public AdvancedController(IRequestHelper requestHelper,
             ILogger<AdvancedController> logger)
        {
            _requestHelper = requestHelper;
            _logger = logger;
        }

        protected void CheckRole(IdentityResponseModel identity, params Role[] roles)
        {
            if (identity.Role == null)
            {
                var ex = new ForbiddenException($"Invalid token");
                _logger.LogError(ex.Message);
                throw ex;
            }
            if (!roles.Select(r => r.ToString()).Contains(identity.Role))
            {
                var ex = new ForbiddenException($"Lead id = {identity.Id} doesn't have access to this endpiont");
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        protected bool CheckMicroservice(IdentityResponseModel identity, params Microservice[] microservices)
        {
            if (!microservices.Select(r => r.ToString()).Contains(identity.IssuerMicroservice))
                return false;
            return true;
        }

        protected IdentityResponseModel GetIdentity()
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (token == null)
            {
                var ex = new ForbiddenException($"Anonimus doesn't have access to this endpiont");
                _logger.LogError(ex.Message);
                throw ex;
            }
            var identity = _requestHelper.GetLeadIdentityByToken(token).Result;
            return identity;
        }

        protected void Validate<T>(T requestModel, IValidator<T> validator)
        {
            if (requestModel == null)
            {
                var ex = new BadRequestException("You must specify the table details in the request body");
                _logger.LogError(ex.Message);
                throw ex;
            }
            var validationResult = validator.Validate(requestModel);
            if (!validationResult.IsValid)
            {
                var ex = new ValidationException(validationResult.Errors);
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}
