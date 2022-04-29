namespace CRM.BusinessLayer.Exceptions
{
    public class RequestTimeoutException: Exception
    {
        public RequestTimeoutException(string message) : base(message)
        { }
    }
}
