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
                _logger.Error($"{typeof(T).Name} entity with ID = {id} not found");
                throw new NotFoundException($"{typeof(T).Name} entiy with ID = {id} not found");
            }
        }

        public static void ThrowIfEmailNotFound(string email, Lead lead)
        {
            if (lead is null)
            {
                _logger.Error($"Entity with e-mail = {email} not foun");
                throw new NotFoundException($"Entity with e-mail = {email} not found");
            }
        }

        public static void ThrowIfLeadWasBanned(int id, Lead lead)
        {
            if (lead is null)
            {
                _logger.Error($"Lead with ID = {id} is banned");
                throw new BannedException($"Lead with ID = {id} is banned");
            }
        }

        public static void ThrowIfPasswordIsIncorrected(string pass, string hashPassFromBd)
        {
            if (!PasswordHash.ValidatePassword(pass, hashPassFromBd))
            {
                _logger.Error($"Try to login. Incorrected password.");
                throw new IncorrectPasswordException("Try to login. Incorrected password.");
            }
        }

        public static void ThrowIfEmailRepeat(Lead lead, string email)
        {

            if (lead != null)
            {
                _logger.Error($"Try to singup. Email {email} is already exists.");
                throw new DuplicationException($"Try to singup. Email {email} is already exists.");
            }
        }

        public static void ThrowIfLeadDontHaveAccesToAccount(int accountLeadId, int authorizathionLeadId)
        {
            if (accountLeadId != authorizathionLeadId)
            {
                _logger.Error($"Authorization error. Lead with ID {authorizathionLeadId} dont have acces to accoutn with ID {accountLeadId}.");
                throw new AuthorizationException($"Authorization error. Lead with ID {authorizathionLeadId} dont have acces to accoutn with ID {accountLeadId}.");
            }
        }

    }
}
