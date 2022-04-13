namespace CRM.BusinessLayer.Exceptions
{
    public class BannedException : BadRequestException
    {
        public BannedException(string message) : base(message)
        { }
    }
}
