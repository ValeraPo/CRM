﻿using Marvelous.Contracts;
using Marvelous.Contracts.Enums;

namespace CRM.BusinessLayer.Models
{
    public class LeadModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public List<AccountModel> Accounts { get; set; }
        public Role Role { get; set; }
        public bool IsBanned { get; set; }

    }
}
