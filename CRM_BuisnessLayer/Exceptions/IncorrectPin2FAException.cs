namespace CRM.BusinessLayer.Exceptions
{
    public class IncorrectPin2FAException : BadRequestException
    {
        public IncorrectPin2FAException(string message) : base(message)
        { }
    }
}
