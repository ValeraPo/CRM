using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Security;
using CRM.DataLayer.Entities;
using NLog;

namespace CRM.BusinessLayer
{
    public static class ExceptionsHelper
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void ThrowIfEntityNotFound<T>(int id, T entity)
        {
            if (entity is null)
            {
                _logger.Error($"Ошибка поиска. {typeof(T).Name} с id = {id} не найден");
                throw new NotFoundException($"{typeof(T).Name} с id = {id} не найден");
            }
        }

        public static void ThrowIfEmailNotFound(string email, Lead lead)
        {
            if (lead is null)
            {
                _logger.Error($"Ошибка поиска. Lead с {email} не найден");
                throw new NotFoundException($"Lead с {email} не найден");
            }
        }

        public static void ThrowIfLeadWasBanned(int id, Lead lead)
        {
            if (lead is null)
            {
                _logger.Error($"Ошибка запроса. Lead с id = {id} забанен");
                throw new BannedException($"Lead с id = {id} забанен");
            }
        }

        public static void ThrowIfPasswordIsIncorrected(string pass, string hashPassFromBd)
        {
            if (!PasswordHash.ValidatePassword(pass, hashPassFromBd))
            {
                _logger.Error($"Ошибка ввода пароля. Введен неверный пароль.");
                throw new IncorrectPasswordException("Неверный пароль");
            }
        }

    }
}
