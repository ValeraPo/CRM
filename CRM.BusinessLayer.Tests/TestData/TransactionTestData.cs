using CRM.DataLayer.Entities;
using Marvelous.Contracts;



namespace CRM.BusinessLayer.Tests.TestData
{
    public class TransactionTestData
    {
        public TransactionRequestModel GetTransactionRequestModel()
        {
            return new TransactionRequestModel()
            {
                AccountId = 1,
                Amount = 10,
                Currency = Currency.AFN
            };
        }

        public TransferRequestModel GetTransferRequestModel()
        {
            return new()
            {
                Amount = 10,
                AccountIdFrom = 1,
                AccountIdTo = 2,
                CurrencyFrom = Currency.AWG,
                CurrencyTo = Currency.ARS,
            };
        }

        public Account GetAccount()
        {
            return new Account()
            {
                Id = 1
            };
        }
    }
}
