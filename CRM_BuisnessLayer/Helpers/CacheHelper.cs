using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.BusinessLayer.Helpers
{
    public class CacheHelper
    {
        private readonly IMemoryCache _memoryCache;
        public async Task<int> SetChacheTransactionModel(TransactionRequestModel transactionModel)
        {
            int key = transactionModel.AccountId + Convert.ToInt32(DateTime.Now.ToString());
            _memoryCache.Set(key, transactionModel, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            return key;
        }

        public async Task<TransactionRequestModel> GetChacheTransactionModel(int tmpId)
        {
            return (TransactionRequestModel)_memoryCache.Get(tmpId);
        }
    }
}
