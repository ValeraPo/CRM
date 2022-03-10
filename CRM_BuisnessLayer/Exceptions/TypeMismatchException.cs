namespace CRM.BusinessLayer.Exceptions
{
    public class TypeMismatchException : BadRequestException
    {
        public TypeMismatchException(string message) : base(message)
        {
        }
    }
}
