using CRM.BusinessLayer.Models;
using CRM.DataLayer.Entities;
using Marvelous.Contracts;
using System.Collections.Generic;

namespace CRM.BusinessLayer.Tests.TestData
{
    public class LeadTestData
    {
        public LeadModel GetLeadModelForTests()
        {
            LeadModel leadModel = new LeadModel
            {

                Id = 1,
                Name = "Василий",
                LastName = "Иванов",
                BirthDate = System.DateTime.Today,
                Email = "ivanov@mail.com",
                Password = "1000:8S83WAEVgLwRh+2W0UjHQzjSP6wMsKbJ:tSKgueB+sLXOYioRB0Ozd6uu/ww=",
                Phone = "+79567436745",
                Role = Role.Regular,
                Accounts = new List<AccountModel> {
                        new AccountModel
                        {
                            CurrencyType = Currency.RUB
                        }
                    }
            };
            return leadModel;
        }

        public Lead GetLeadForTests()
        {
            Lead leadModel = new Lead
            {

                Id = 1,
                Name = "Василий",
                LastName = "Иванов",
                BirthDate = System.DateTime.Today,
                Email = "ivanov@mail.com",
                Password = "1000:8S83WAEVgLwRh+2W0UjHQzjSP6wMsKbJ:tSKgueB+sLXOYioRB0Ozd6uu/ww=",
                Phone = "+79567436745",
                Role = Role.Regular,
                Accounts = new List<Account> {
                        new Account
                        {
                            CurrencyType = Currency.RUB
                        }
                    }
            };
            return leadModel;
        }

        public List<Lead> GetListOfLeadsForTests()
        {
            return new List<Lead>{
               new Lead
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
                       new Account
                       {
                           CurrencyType = Currency.RUB
                       }
                   }
               },

               new Lead
               {
                   Id = 2,
                   Name = "Михаил",
                   LastName = "Петров",
                   BirthDate = System.DateTime.Today,
                   Email = "petrovv@mail.com",
                   Password = "qert123",
                   Phone = "+79567436745",
                   Role = Role.Vip,
                   Accounts = new List<Account> {
                       new Account
                       {
                           CurrencyType = Currency.USD
                       }
                   }
               }
            };
        }
    }
}
