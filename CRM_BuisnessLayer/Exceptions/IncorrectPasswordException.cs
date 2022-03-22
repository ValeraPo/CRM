namespace CRM.BusinessLayer.Exceptions
{
    public class IncorrectPasswordException : BadRequestException
    {
        public IncorrectPasswordException(string message) : base(message)
        { }
    }
}
