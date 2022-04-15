using CRM.BusinessLayer.Exceptions;
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

        public static void ThrowIfLeadDontHaveAccesToAccount(int accountLeadId, int authorizathionLeadId)
        {
            if (accountLeadId != authorizathionLeadId)
            {
                _logger.Error($"Authorization error. Lead with ID {authorizathionLeadId} dont have acces to accoutn.");
                throw new AuthorizationException($"Authorization error. Lead with ID {authorizathionLeadId} dont have acces to accoutn.");
            }
        }

    }
}
