﻿using CRM.DataLayer.Entities;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface ILeadRepository
    {
        Task<int> AddLead(Lead lead);
        Task UpdateLeadById(Lead lead);
        Task ChangeRoleLead(Lead lead);
        Task DeleteById(int id);
        Task RestoreById(int id);
        Task ChangePassword(int id, string hashPassword);
        Task<List<Lead>> GetAll();
        Task<Lead> GetById(int id);
        Task<Lead> GetByEmail(string email);
    }
}
