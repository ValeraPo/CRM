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

        public static void ThrowIfLeadDontHaveAcces(int leadId, int authorizathionLeadId)
        {
            if (leadId != authorizathionLeadId)
            {
                _logger.Error($"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.");
                throw new AuthorizationException($"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.");
            }
        }

        public static void ThrowIfPin2FAIsIncorrected(int pin, int leadId, string password)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            if (tfa.ValidateTwoFactorPIN(Convert.ToString(leadId), password))
            {
                throw new IncorrectPin2FAException("Try to WithDraw. Incorrected pin 2FA.");
            }
        }
    }
}
