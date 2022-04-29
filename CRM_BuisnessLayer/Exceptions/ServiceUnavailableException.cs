namespace CRM.BusinessLayer.Exceptions
{
    public class ServiceUnavailableException: Exception
    {
        public ServiceUnavailableException(string message) : base(message)
        { }
    }
}
