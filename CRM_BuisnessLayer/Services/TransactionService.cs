﻿using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Logging;
using NLog;
using RestSharp;
using System.Collections;

namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<TransactionService> _logger;
        private const string _url = "https://piter-education.ru:6060";

        public TransactionService(IAccountRepository accountRepository, IRequestHelper requestHelper, ILogger<TransactionService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        public async Task<RestResponse> AddDeposit(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Popytka tranzakcii с аккаунта id = {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            _logger.LogInformation($"Otpravka zaprosa na tranzakciu c accounta id = {transactionModel.AccountId}.");
            var response = await _requestHelper.SendRequest<TransactionRequestModel>(_url, UrlTransaction.Deposit, Method.Post, transactionModel);
            _logger.LogInformation($"Poluchen otvet na tranzakciu c accounta id = {transactionModel.AccountId}.");

            return response;
        }

        public async Task<RestResponse> AddTransfer(TransferRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Popytka transfera c accounta id = {transactionModel.AccountIdFrom} na account id = {transactionModel.AccountIdTo}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountIdFrom);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdFrom, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            var accountTo = await _accountRepository.GetById(transactionModel.AccountIdTo);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdTo, accountTo);
            _logger.LogInformation($"Otpravka zaprosa transfera c accounta id = {transactionModel.AccountIdFrom} na account id = {transactionModel.AccountIdTo}.");
            var response = await _requestHelper.SendRequest<TransferRequestModel>(_url, UrlTransaction.Transfer, Method.Post, transactionModel);
            _logger.LogInformation($"Poluchen otvet na transfer c accounta id = {transactionModel.AccountIdFrom} na account id = {transactionModel.AccountIdTo}.");

            return response;
        }

        public async Task<RestResponse> Withdraw(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Popytka vyvoda sredstv c accounta id = {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            _logger.LogInformation($"Otpravka zaprosa na vyvod sredstv c accounta id = {transactionModel.AccountId}.");
            var response = await _requestHelper.SendRequest<TransactionRequestModel>(_url, UrlTransaction.Withdraw, Method.Post, transactionModel);
            _logger.LogInformation($"Poluchen otvet na vyvod sredstv c accounta id = {transactionModel.AccountId}.");

            return response;
        }

        public async Task<decimal> GetBalance(int id)
        {
            _logger.LogInformation($"Popytka poluchenia balansa  accounta id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _logger.LogInformation($"Otpravka zaprosa na poluchenie balansa accounta id = {id}.");
            var response = await _requestHelper.SendGetRequest(_url, "balanse-by-", id);
            _logger.LogInformation($"Poluchen otvet na poluchenie balansa accounta id = {id}.");

            return Convert.ToDecimal(response.Content);
        }

        public async Task<RestResponse> GetTransactionsByAccountId(int id)
        {
            _logger.LogInformation($"Popytka polucheniia transakcii accounta id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _logger.LogInformation($"Otpravka zaprosa na poluchenie transakcii accounta id = {id}.");
            var response = await _requestHelper.SendGetRequest(_url, "transaction/", id);
            _logger.LogInformation($"Poluchen otvet na poluchenie transakcii accounta id = {id}.");

            return response;
        }
    }
}
