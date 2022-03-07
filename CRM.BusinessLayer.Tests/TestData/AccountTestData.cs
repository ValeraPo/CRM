using CRM.BusinessLayer.Models;
using CRM.DataLayer.Entities;
using System.Collections.Generic;

namespace CRM.BusinessLayer.Tests.TestData
{
    public class AccountTestData
    {
        public AccountModel GetAccountModelForTests()
        {
            AccountModel accountModel = new AccountModel
            {
                Id = 1,
                CurrencyType = CurrencyEnum.Currency.USD,
                IsBlocked = false,
                LockDate = null,
                Name = "MyAccount",
                Lead = new LeadModel
                {
                    Id = 1,
                    Name = "Василий",
                    LastName = "Иванов",
                    BirthDate = System.DateTime.Today,
                    Email = "ivanov@mail.com",
                    Password = "qert123",
                    Phone = "+79567436745",
                    Role = Role.Regular,
                    Accounts = new List<Account> { 
                        new AccountModel
                        {
                            CurrencyType = CurrencyEnum.Currency.RUB
                        }
                    }
                }
            };
            return accountModel;
        }

        public AccountModel GetAccountModelAdminForTests()
        {
            AccountModel accountModel = new AccountModel
            {
                Id = 1,
                CurrencyType = CurrencyEnum.Currency.USD,
                IsBlocked = false,
                LockDate = null,
                Name = "MyAccount",
                Lead = new LeadModel
                {
                    Id = 1,
                    Name = "Василий",
                    LastName = "Иванов",
                    BirthDate = System.DateTime.Today,
                    Email = "ivanov@mail.com",
                    Password = "qert123",
                    Phone = "+79567436745",
                    Role = Role.Admin,
                }
            };
            return accountModel;
        }

        public AccountModel GetAccountModelRegularForTests()
        {
            AccountModel accountModel = new AccountModel
            {
                Id = 1,
                CurrencyType = CurrencyEnum.Currency.ANG,
                IsBlocked = false,
                LockDate = null,
                Name = "MyAccount",
                Lead = new LeadModel
                {
                    Id = 1,
                    Name = "Василий",
                    LastName = "Иванов",
                    BirthDate = System.DateTime.Today,
                    Email = "ivanov@mail.com",
                    Password = "qert123",
                    Phone = "+79567436745",
                    Role = Role.Admin,
                    Accounts = new List<Account> {
                        new AccountModel
                        {
                            CurrencyType = CurrencyEnum.Currency.RUB
                        }
                    }
                }
            };
            return accountModel;
        }
    }
}
