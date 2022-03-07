using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Security;

namespace CRM.BusinessLayer
{
    public static class ExceptionsHelper
    {
        public static void ThrowIfEntityNotFound<T>(int id, T entity)
        {
            if (entity is null)
                throw new NotFoundException($"{typeof(T).Name} с id = {id} не найден");
        }
        public static void ThrowIfLeadWasBanned<T>(int id, T entity)
        {
            if (entity is null)
                throw new BannedException($"Lead с id = {id} забанен");
        }

        public static void ThrowIfPasswordIsIncorrected(string pass, string hashPassFromBd)
        {
            if (!PasswordHash.ValidatePassword(pass, hashPassFromBd))
                throw new IncorrectPasswordException("Неверный пароль");
        }

    }
}
