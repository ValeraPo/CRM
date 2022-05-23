using CRM.BusinessLayer.Exceptions;
using NLog;

namespace CRM.BusinessLayer
{
    public static class ExceptionsHelper
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
       
        // Checking if entity is null
        public static void ThrowIfEntityNotFound<T>(int id, T entity)
        {
            if (entity is null)
            {
                _logger.Error($"{typeof(T).Name} entity with ID = {id} not found");
                throw new NotFoundException($"{typeof(T).Name} entiy with ID = {id} not found");
            }
        }

        // Checking if lead id is not id from token
        public static void ThrowIfLeadDontHaveAcces(int leadId, int authorizathionLeadId)
        {
            if (leadId != authorizathionLeadId)
            {
                _logger.Error($"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.");
                throw new AuthorizationException($"Authorization error. Lead with ID {authorizathionLeadId} doesn't have acces.");
            }
        }

    }
}
