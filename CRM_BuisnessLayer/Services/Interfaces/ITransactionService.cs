﻿using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using RestSharp;

namespace CRM.BusinessLayer.Services
{
    public interface ITransactionService
    {
        Task<RestResponse> AddDeposit(TransactionRequestModel transactionModel, int leadId);
        Task<RestResponse> AddTransfer(TransferRequestModel transactionModel, int leadId);
        Task<RestResponse> Withdraw(TransactionRequestModel transactionModel, int leadId);
        Task<RestResponse> GetTransactionsByAccountId(int id, int leadId);
    }

}