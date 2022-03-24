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
                _logger.Error($"Oshibka poiska. {typeof(T).Name} c id = {id} ne naiden");
                throw new NotFoundException($"{typeof(T).Name} с id = {id} не найден");
            }
        }

        public static void ThrowIfEmailNotFound(string email, Lead lead)
        {
            if (lead is null)
            {
                _logger.Error($"Oshibka поиска. Lead с {email} не найден");
                throw new NotFoundException($"Lead с {email} не найден");
            }
        }

        public static void ThrowIfLeadWasBanned(int id, Lead lead)
        {
            if (lead is null)
            {
                _logger.Error($"Oshibka zaprosa. Lead c id = {id} zabanen");
                throw new BannedException($"Lead с id = {id} забанен");
            }
        }

        public static void ThrowIfPasswordIsIncorrected(string pass, string hashPassFromBd)
        {
            if (!PasswordHash.ValidatePassword(pass, hashPassFromBd))
            {
                _logger.Error($"Oshibka vvoda parolya. Vveden nevernyi parol'.");
                throw new IncorrectPasswordException("Неверный пароль");
            }
        }

        public static void ThrowIfEmailRepeat(Lead lead, string email)
        {

            if (lead != null)
            {
                _logger.Error($"Oshibka dobavleniya leada. Pol'zovatel' {email} uze suchestvuet.");
                throw new DuplicationException($"Пользователь {email} уже существует.");
            }
        }

        public static void ThrowIfLeadDontHaveAccesToAccount(int accountLeadId, int authorizathionLeadId)
        {
            if (accountLeadId != authorizathionLeadId)
            {
                _logger.Error($"Oshibka manipulyacii c accountom. Pol'zovatel' c id {authorizathionLeadId} ne imeet dostupa k accountu c id {accountLeadId}.");
                throw new AuthorizationException($"Пользователь с Id {authorizathionLeadId} не имеет доступа к аккаунту с Id {accountLeadId}.");
            }
        }

    }
}
