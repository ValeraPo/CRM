using CRM.APILayer.Models;
using System;

namespace CRM.ApiLayer.Tests
{
    public static class LeadControllerTestData
    {
        public static LeadInsertRequest GetInsertModel()
            => new LeadInsertRequest
            {
                Password = "12345678",
                Email = "erty@mail.po",
                Name = "Vasya",
                LastName = "Popoc",
                BirthDate = DateTime.Parse("2022-04-15")
            };

        public static LeadUpdateRequest GetUpdateModel()
            => new LeadUpdateRequest
            {
                Name = "Vasya",
                LastName = "Popoc",
                BirthDate = DateTime.Parse("2022-04-15")
            };

        public static LeadChangePasswordRequest GetChangePasswordModel()
            => new LeadChangePasswordRequest
            {
                NewPassword = "12345678",
                OldPassword = "qwe123qwe",
            };
    }
}
