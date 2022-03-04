﻿using CRM.DataLayer.Entities;

namespace CRM_APILayer.Models
{
    public class LeadOutputModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Role Role { get; set; }
        public List<AccountOutputModel> Accounts { get; set; }
    }
}
