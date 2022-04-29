namespace CRM.BusinessLayer.Exceptions
{
    public class IncorrectPin2FAException : Exception
    {
        public IncorrectPin2FAException(string message) : base(message)
        { }
    }
}
