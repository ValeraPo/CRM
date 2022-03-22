namespace CRM.BusinessLayer.Exceptions
{
    public class IncorrectRoleException : BadRequestException
    {
        public IncorrectRoleException(string message) : base(message)
        { }
    }
}
