namespace CRM.BusinessLayer.Services
{
    public interface IAuthService
    {
        Task<string> GetToken(string email, string pass);
    }
}