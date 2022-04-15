using FluentValidation;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ResponseModels;

namespace CRM.APILayer.Extensions
{
    public interface IAdvancedController
    {
        bool CheckMicroservice(params Microservice[] microservices);
        void CheckRole(params Role[] roles);
        IdentityResponseModel GetIdentity();
        void Validate<T>(T requestModel, IValidator<T> validator);
    }
}