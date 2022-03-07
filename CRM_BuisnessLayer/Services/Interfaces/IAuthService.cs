namespace CRM.BusinessLayer.Services
{
    public interface IAuthService
    {
        string GetToken(string email, string pass);
    }
}