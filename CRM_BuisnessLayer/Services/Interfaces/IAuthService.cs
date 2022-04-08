using Marvelous.Contracts.RequestModels;

namespace CRM.BusinessLayer.Services
{
    public interface IAuthService
    {
        Task<string> GetToken(AuthRequestModel auth);
    }
}