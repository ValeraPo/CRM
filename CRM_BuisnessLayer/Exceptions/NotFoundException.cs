namespace CRM.BusinessLayer.Exceptions
{
    public class NotFoundException : ForbiddenException
    {
        public NotFoundException(string message) : base(message)
        {}
    }
}
