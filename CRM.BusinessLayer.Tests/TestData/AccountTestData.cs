using CRM.BusinessLayer.Models;
using CRM.DataLayer.Entities;
using Marvelous.Contracts;
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
                CurrencyType = Currency.GBP,
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
                            CurrencyType = Currency.RUB
                        }
                    }
                }
            };
            return accountModel;
        }

        public AccountModel GetAccountModelVipForTests()
        {
            AccountModel accountModel = new AccountModel
            {
                Id = 1,
                CurrencyType = Currency.USD,
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
                    Role = Role.Vip,
                }
            };
            return accountModel;
        }

        public AccountModel GetAccountModelRegularForTests()
        {
            AccountModel accountModel = new AccountModel
            {
                Id = 1,
                CurrencyType = Currency.USD,
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
                            CurrencyType = Currency.RUB
                        }
                    }
                }
            };
            return accountModel;
        }
        public List<Account> GetListOfAccountsForTests()
        {
            return new List<Account>{
                new Account
                {
                    Id = 1,
                    CurrencyType = Currency.USD,
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
                                CurrencyType = Currency.RUB
                            }
                        }
                    }
                },
                new Account
                {
                    Id = 2,
                    CurrencyType = Currency.USD,
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
                                CurrencyType = Currency.USD
                            }
                        }
                    }
                }
            };
        }

    }
}
