namespace CRM.BusinessLayer.Exceptions
{
    public class BannedException : ForbiddenException
    {
        public BannedException(string message) : base(message)
        { }
    }
}
