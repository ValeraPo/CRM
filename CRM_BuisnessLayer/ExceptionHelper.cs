using CRM.BusinessLayer.Exceptions;

namespace CRM.BusinessLayer
{
    public static class ExceptionsHelper
    {
        public static void ThrowIfEntityNotFound<T>(int id, T entity)
        {
            if (entity is null)
                throw new NotFoundException($"{typeof(T).Name} с id = {id} не найден");
        }
    }
}
