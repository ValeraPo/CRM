using AutoMapper;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionsController(ITransactionService transactionService, IMapper autoMapper)
        {
            _transactionService = transactionService;
            _mapper = autoMapper;
        }

        // api/transaction/
        [HttpPost("deposit")]
        [SwaggerOperation(Summary = "Add deposit")]
        [SwaggerResponse(201, "Deposit added")]
        public ActionResult AddDeposit([FromBody] TransactionRequest transaction)
        {
            var transactionModel = _mapper.Map<TransactionModel>(transaction);
            var transactionId = _transactionService.AddDeposit(transactionModel);

            return StatusCode(201, transactionId);
        }

        // api/transaction/
        [HttpPost("transfer-to-{accountId}-in-{currencyTo}")]
        [SwaggerOperation(Summary = "Add transfer")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        public ActionResult AddTransfer([FromBody] TransactionRequest transaction, int accountId, int currencyTo)
        {
            var transactionModel = _mapper.Map<TransactionModel>(transaction);
            var transactionId = _transactionService.AddTransfer(transactionModel, accountId, currencyTo);

            return StatusCode(201, transactionId);
        }

        [HttpPost("withdraw")]
        [SwaggerOperation(Summary = "Withdraw")]
        [SwaggerResponse(201, "Withdraw successful")]
        public ActionResult Withdraw([FromBody] TransactionRequest transaction)
        {
            var transactionModel = _mapper.Map<TransactionModel>(transaction);
            var transactionId = _transactionService.Withdraw(transactionModel);

            return StatusCode(201, transactionId);
        }
    }
}
