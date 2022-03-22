namespace CRM.BusinessLayer.Exceptions
{
    public class AuthorizationException : ForbiddenException
    {
        public AuthorizationException(string message) : base(message)
        { }
    }
}
