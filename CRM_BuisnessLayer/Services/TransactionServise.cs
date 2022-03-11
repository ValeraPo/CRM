using AutoMapper;
using CRM.BusinessLayer.Models;
using CRM.DataLayer.Repositories.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Services
{
    public class TransactionServise : ITransactionServise
    {
        private readonly IAccountRepository _accountRepository;


        public TransactionServise(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Task<RestResponse> AddDeposit(TransactionModel transactionModel)
        {
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var client = new RestClient("https://api.myorg.com");
            var request = new RestRequest("/transaction/", Method.Post);
            request.AddJsonBody(transactionModel);
            var response = client.ExecuteAsync(request);

            return response;
        }

        public Task<RestResponse> AddTransfer(TransactionModel transactionModel, int accountIdTo, int currencyTo)
        {
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var accountTo = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, accountTo);

            var client = new RestClient("https://api.myorg.com");
            var request = new RestRequest($"/transfer-to-{accountIdTo}-in-{currencyTo}/", Method.Post);
            request.AddJsonBody(transactionModel);
            var response = client.ExecuteAsync(request);
            
            return response;
        }

        public Task<RestResponse> Withdraw(TransactionModel transactionModel)
        {
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var client = new RestClient("https://api.myorg.com");
            var request = new RestRequest("/withdraw/", Method.Post);
            request.AddJsonBody(transactionModel);
            var response = client.ExecuteAsync(request);
            return response;
        }
    }
}
