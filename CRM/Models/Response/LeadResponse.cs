﻿using CRM.DataLayer.Entities;

namespace CRM.APILayer.Models
{
    public class LeadResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Role Role { get; set; }
        public List<AccountResponse> Accounts { get; set; }
    }
}